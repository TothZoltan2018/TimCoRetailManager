using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Library.Models
{
    // This list will be sent over the API
    public class SaleModel
    {
        public List<SaleDetailModel> SaleDetails { get; set; } = new List<SaleDetailModel>();
    }
}
