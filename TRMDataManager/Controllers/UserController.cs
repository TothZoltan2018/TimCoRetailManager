﻿using Microsoft.AspNet.Identity; // it will get the extension method which provides info about the user
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]    
    public class UserController : ApiController
    {        
        // Find a logged in user's data.
        public List<UserModel> GetById()
        {
            string userId = RequestContext.Principal.Identity.GetUserId();
            UserData data =  new UserData();

            return data.GetUserById(userId);
        } 
    }
}
