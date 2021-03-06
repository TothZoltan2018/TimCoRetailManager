﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    // Inheriting from "Conductor<object>" allows to display one and only one form on view "ShelView"
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {        
        private IEventAggregator _events;        
        private ILoggedInUserModel _user;
        private IAPIHelper _aPIHelper;

        public ShellViewModel(LoginViewModel loginVM,
                              IEventAggregator events,                              
                              ILoggedInUserModel user,
                              IAPIHelper aPIHelper)
        {
            _events = events;            
            _user = user;
            _aPIHelper = aPIHelper;
            
            // Subscribing to all events of IHandle<A>, IHandle<B>...
            _events.SubscribeOnPublishedThread(this);

            // Get a brand new instance of LoginViewModel and activate it.
            // We do not await it intentionlly.
            ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());
        }

        public bool IsLoggedIn
        {
            get
            {
                bool output = false;

                if (string.IsNullOrEmpty(_user.Token) == false) // So, the user is not logged in
                {
                    output = true;
                }

                return output;
            }
        }

        public void ExitApplication()
        {
            //this.ExitApplication();  // Caused StackOverflow Exception
            TryCloseAsync();
        }

        public async Task UserManagement()
        {
            await ActivateItemAsync(IoC.Get<UserDisplayViewModel>(), new CancellationToken());
        }

        public async Task LogOut()
        {
            _user.ResetUserModel();
            _aPIHelper.LogOffUser();
            await ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        // This listens for the LogOnEvent. (Because this class inhereted from IHandle<LogOnEvent>)
        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {            
            // Close out the LoginView and open up the SalesView. This happens because 
            // AcitvateItem is in the Conductor class,
            // and when we have this conductor class, only one item can be active.
            // The old item is not deleted, remains in _loginVM.
            // Get a brand new instance of SalesViewModel and activate it.
            await ActivateItemAsync(IoC.Get<SalesViewModel>(), cancellationToken);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
