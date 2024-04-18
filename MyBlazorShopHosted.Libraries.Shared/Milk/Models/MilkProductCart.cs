using MyBlazorShopHosted.Libraries.Shared.Product.Models;
using MyBlazorShopHosted.Libraries.Shared.ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlazorShopHosted.Libraries.Shared.Milk.Models
{
    public class MilkProductCart
    {
        public List<ShoppingCartItem> Items { get; set; }

        public double Total { get; set; }


        public void AddProduct(SXProducts product, int quantity)
        {

            if (Items.Any(i => i.product.Id == product.Id))
            {
                var item = Items.First(i => i.product.Id == product.Id);
                item.Price = product.Price;
                item.Quantity = quantity;
            }
            else
            {
                Items.Add(new ShoppingCartItem
                {
                    product = product,
                    Quantity = quantity
                });
            }
        }

        public List<ShoppingCartItem> GetItems() {  return Items; }

    }
}



