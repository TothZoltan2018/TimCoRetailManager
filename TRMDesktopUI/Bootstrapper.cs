﻿using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TRMDesktopUI.Helpers;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Helpers;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;
using TRMDesktopUI.ViewModels;

namespace TRMDesktopUI
{
    // We setup CaliburnMicro in this class
    // But before:
    // 1. We installed CaliburnMicro nuget package
    // 2. Created Folder structure: Models, Views, ViewModels
    // 3. Created ShellViewModel class and ShellView window
    // 4. Delete "StartupUri="MainWindow.xaml">" from App.xamlm, add <Application.Resources> and delete file MainWindow.xaml

    class Bootstrapper : BootstrapperBase
    {
        // This is a kind of Dependency Injection tool, like Autofac, but CaliburnMicro involve it.
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize(); // Initialize the framework

            // These 4 lines are copied from https://stackoverflow.com/questions/30631522/caliburn-micro-support-for-passwordbox
            ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");
        }

        private IMapper ConfigureAutomapper()
        {
            // At the beginning of our application itt will do reflection. Only once, therefore it's fast.
            // It maps one model to an other.
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductModel, ProductDisplayModel>();
                cfg.CreateMap<CartItemModel, CartItemDisplayModel>();
            });

            var output = config.CreateMapper();

            return output;
        }

        protected override void Configure()
        {
            _container.Instance(ConfigureAutomapper());

            // Whenever we ask for a SimpleContainer instance, it will return it.
            _container.Instance(_container)
                .PerRequest<IProductEndpoint, ProductEndpoint>()
                .PerRequest<IUserEndpoint, UserEndpoint>()
                .PerRequest<ISaleEndpoint, SaleEndpoint>();

            _container
                .Singleton<IWindowManager, WindowManager>()
                // If the ShellWiewModel aks for an IEventAggregator, it gives back the first EventAggregator ever created
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IAPIHelper, APIHelper>()
                .Singleton<IConfigHelper, ConfigHelper>()
                .Singleton<ILoggedInUserModel, LoggedInUserModel>();

            // Connect all of our ViewModels to our Views
            // Reflection: Although this is resource consuming, but it happens only once, at startup.
            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // On startup launch ShellViewModel as our base view. Then the viewmodel will launch the view
            DisplayRootViewFor<ShellViewModel>(); 
        }

        // This creates the instance of type "service" with name "key"
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
