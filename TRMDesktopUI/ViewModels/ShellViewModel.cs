using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;

namespace TRMDesktopUI.ViewModels
{
    // Inheriting from "Conductor<object>" allows to display one and only one form on view "ShelView"
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {        
        private IEventAggregator _events;
        private SalesViewModel _salesVM;       

        public ShellViewModel(LoginViewModel loginVM, IEventAggregator events, SalesViewModel salesVM)
        {
            _events = events;            
            _salesVM = salesVM;
            
            // Subscribing to all events of IHandle<A>, IHandle<B>...
            _events.Subscribe(this);

            // Get a brand new instance of LoginViewModel and activate it.            
            ActivateItem(IoC.Get<LoginViewModel>());
        }

        // This listen for the LogOnEvent
        public void Handle(LogOnEvent message)
        {
            // Close out the LoginView and open up the SalesView. This happens because 
            // AcitvateItem is in the Conductor class,
            // and when we have this conductor class, only one item can be active.
            // The old item is not deleted, remains in _loginVM.
            ActivateItem(_salesVM);
        }
    }
}
