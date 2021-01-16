using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Library.Models
{
    // A list from this will be sent over the API
    public class SaleDetailModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
