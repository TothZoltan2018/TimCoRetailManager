using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TRMDesktopUI.Models;

namespace TRMDesktopUI.Helpers
{
    // Handles all of our API call interactions so that the Views call this and say that give me 
    // this and this
    public class APIHelper : IAPIHelper
    {
        private HttpClient apiClient;

        public APIHelper()
        {
            InitializeClient();
        }

        private void InitializeClient()
        {
            // We added System.Configuration to References of this Project
            // The value belongs to the key "api" in the Appconfig file
            string api = ConfigurationManager.AppSettings["api"];

            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri(api);
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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

            using (HttpResponseMessage response = await apiClient.PostAsync("/Token", data)) // RequestUri is in the Appconfig
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
    }
}

