using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
    [Authorize]
    public class InventoryController : ApiController
    {
        //[Authorize(Roles = "AnOtherRole")] // AnOtherRole AND Admin
        [Authorize(Roles = "Admin")]
        public void Post(InventoryModel item)
        {
            InventoryData data = new InventoryData();
            
            data.SaveInventoryRecord(item);
        }

        [Authorize(Roles ="Manager,Admin")] // Manager OR Admin
        public List<InventoryModel> Get()
        {
            InventoryData data = new InventoryData();

            return data.GetInventory();
        }
    }
}
