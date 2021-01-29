using Microsoft.AspNet.Identity; // it will get the extension method which provides info about the user
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;
using TRMDataManager.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]    
    public class UserController : ApiController
    {        
        // Find a logged in user's data.
        [HttpGet]
        // I suppose, this name doesn't matter, the [HttpGet] attribute binds this method to 
        // the code section in APIHelper.GetLoggedInUserInfo, where HttpClient.GetAsync("/api/User") is called
        public UserModel GetById() 
        {
            string userId = RequestContext.Principal.Identity.GetUserId();
            UserData data =  new UserData();

            return data.GetUserById(userId).First();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var users = userManager.Users.ToList();// equivalent: context.Users.ToList(), so no need to userStore and userManager
                var roles = context.Roles.ToList(); // it also contains the the userId

                foreach (var user in users)
                {
                    ApplicationUserModel u = new ApplicationUserModel
                    {
                        Id = user.Id,
                        Email = user.Email
                    };

                    // Match rolenames to all the roleids the user has
                    foreach (var userRole in user.Roles)
                    {
                        u.Roles.Add(userRole.RoleId, roles.Where(x => x.Id == userRole.RoleId).First().Name);
                    }

                    output.Add(u);
                }
            }

            return output;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("api/User/Admin/GetAllRoles")]
        public Dictionary<string, string> GetAllRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                // creating a new variable could have some tiny performance issue, but good for debugging
                var roles = context.Roles.ToDictionary(x => x.Id, x => x.Name);

                return roles;
            }                          
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/AddRole")]
        public void AddARole(UserRolePairModel pairing)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.AddToRole(pairing.UserId, pairing.RoleName);
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("api/User/Admin/RemoveRole")]
        public void RemoveARole(UserRolePairModel pairing)
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                userManager.RemoveFromRole(pairing.UserId, pairing.RoleName);
            }
        }
    }
}
