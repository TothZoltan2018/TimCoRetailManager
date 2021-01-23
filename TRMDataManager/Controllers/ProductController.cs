using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRMDataManager.Library;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
    [Authorize(Roles = "Cashier")] // User must be authorized in role of a Cashier to use this class
    public class ProductController : ApiController
    {
        // GET api/values
        public List<ProductModel> Get()
        {
            ProductData data = new ProductData();

            return data.GetProducts();   
        }
    }
}
