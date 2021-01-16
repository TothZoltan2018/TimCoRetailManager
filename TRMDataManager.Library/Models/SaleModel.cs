using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TRMDataManager.Library.Models
{
    public class SaleModel
    {
        // This Model gets populated from the API
        // We do not initialize this property (unlike the TRMDesktopUI.Library.Models.SaleModel),
        // because we want to know if there was a problem during posting its values on the API.
        public List<SaleDetailModel> SaleDetails { get; set; }
    }
}