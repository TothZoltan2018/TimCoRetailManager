using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName;
        private string _password ="";

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogIn); // CanLogin is not working if that would be a method              
            }
        }               

        public string Password
        {
            get { return _password; }
            set 
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);                
            }
        }

        // This magically enables/disables the "Log In" button behind the scenes. It must work upon naming conventions by Caliburn.Micro
        public bool CanLogIn
        {
            get 
            {
                bool output = false;

                if (UserName?.Length > 0 && Password?.Length > 0) // "?": If NOT Null
                {
                    output = true;
                }

                return output;
            }
        }

        public void LogIn()
        {
            
        }

    }
}
