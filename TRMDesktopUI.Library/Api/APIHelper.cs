using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.Library.Api
{
    // Handles all of our API call interactions so that the Views call this and say that give me 
    // this and this
    public class APIHelper : IAPIHelper
    {
        private HttpClient _apiClient;
        private ILoggedInUserModel _loggedInUser;

        public APIHelper(ILoggedInUserModel loggedInUser)
        {
            InitializeClient();
            _loggedInUser = loggedInUser;
        }

        // Read only property to be used from outside this class.
        //This class is a singleton (created in Bootstrapper), so only one instance of this HttpClient proprty exists.
        public HttpClient ApiClient
        {
            get
            {
                return _apiClient;
            }                
        }

        private void InitializeClient()
        {
            // We added System.Configuration to References of this Project
            // The value belongs to the key "api" in the Appconfig file
            string api = ConfigurationManager.AppSettings["api"];

            _apiClient = new HttpClient();
            _apiClient.BaseAddress = new Uri(api);
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Call out to the API using the ApiClient username and password and get back our token info
        public async Task<AuthenticatedUser> Authenticate(string username, string password)
        {
            // This data as token will be sent to the API endpoint to get back a response
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            using (HttpResponseMessage response = await _apiClient.PostAsync("/Token", data)) // RequestUri is in the Appconfig
            {
                if (response.IsSuccessStatusCode)
                {
                    // Intalled nuget package: Microsoft.AspNet.WebApi.Client
                    // Grab the info from the "Content" and put to an AuthenticatedUser model.
                    var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public void LogOffUser()
        {
            // Without clearing the token, that would be still valid even if we are logged out on the UI --> Security hole
            _apiClient.DefaultRequestHeaders.Clear();        
        }

        public async Task GetLoggedInUserInfo(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            using (HttpResponseMessage response = await _apiClient.GetAsync("/api/User")) // UserController
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<LoggedInUserModel>();
                    // This mapping task could have been done with Automapper
                    // but only for the sake of this one mappping it is not worth, so do it manually
                    _loggedInUser.CreatedDate = result.CreatedDate;
                    _loggedInUser.EmailAddress = result.EmailAddress;
                    _loggedInUser.FirstName = result.FirstName;
                    _loggedInUser.Id = result.Id;
                    _loggedInUser.LastName = result.LastName;
                    _loggedInUser.Token = token;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }            
            }
        }
    }
}