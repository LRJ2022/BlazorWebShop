using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlazorShopHosted.Libraries.Shared.Milk.Models
{
    public class ShoppingCartItem
    {
        public SXProducts product { get; set; }
        public double? Price { get; set; }
        public int Quantity { get; set; }
    }
}
