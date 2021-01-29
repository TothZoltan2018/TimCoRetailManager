using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TRMDataManager.Models
{
    // Method parameters which are passed in as HttpGet can be multiple. These are sent as url parameters.
    // But those which are passed in as HttpPost needs to be
    //wrapped in a single object (like this model) and not exposed.
    public class UserRolePairModel
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }
    }
}