using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
    [Route("api/[controller]")] // Means api/Sale
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase // MVC controller
    {
        // UI|---- Post ---> Insert into Database
        [Authorize(Roles = "Cashier")]
        public void Post(SaleModel sale)
        {
            SaleData data = new SaleData();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);//in framework: RequestContext.Principal.Identity.GetUserId();;

            data.SaveSale(sale, userId);
        }

        // Query Database|---- Get ---> UI
        [Authorize(Roles = "Admin,Manager")]
        [Route("GetSalesReport")]
        public List<SaleReportModel> GetSalesReport()
        {
            //if (RequestContext.Principal.IsInRole("Admin"))
            //{
            //    // Do asmin stuff
            //}

            SaleData data = new SaleData();

            return data.GetSaleReport();
        }
    }
}
