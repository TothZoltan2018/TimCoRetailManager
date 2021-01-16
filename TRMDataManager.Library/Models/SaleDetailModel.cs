using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TRMDataManager.Library.Models
{
    // This Model gets populated from the API
    public class SaleDetailModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}