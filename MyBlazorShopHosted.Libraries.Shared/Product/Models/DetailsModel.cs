using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlazorShopHosted.Libraries.Shared.Product.Models
{
    public class DetailsModel
    {
        public ProductModel Product { get; set; }
        public int Quatity { get; set; }
        public decimal Total { get; set; }
    }
}
