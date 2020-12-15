using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public Bootstrapper()
        {
            Initialize(); // Initialize the framework
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // On startup launch ShellViewModel as our base view. Then the viewmodel will launch the view
            DisplayRootViewFor<ShellViewModel>(); 
        }
    }
}
