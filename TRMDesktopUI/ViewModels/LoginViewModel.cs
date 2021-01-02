using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;

namespace TRMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName;
        private string _password;
        private IAPIHelper _apiHelper;
        public LoginViewModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

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
        
        public bool IsErrorVisible
        {
            get
            {
                bool output = false;

                if (ErrorMessage?.Length > 0)
                {
                    output = true;
                }
                return output ;            
            }   
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);                
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

        public async Task LogIn()
        {
            try
            {
                ErrorMessage = "";
                var result = await _apiHelper.Authenticate(UserName, Password);

                // Capture more info about the logged in user
                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            
        }

    }
}
