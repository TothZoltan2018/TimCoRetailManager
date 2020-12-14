using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity; // it will get the extension method which provides info about the user

namespace TRMDataManager.Controllers
{
    [Authorize] // In order for you to call any of these endpoints in these valuecontrollers, you have to be logged in.
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            string userId = RequestContext.Principal.Identity.GetUserId();
            
            return new string[] { "value1", "value2", userId };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }
        
        // POST api/values
        public void Post([FromBody]string value) // The value field in the swagger form needs to be inside "" to get in here
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
