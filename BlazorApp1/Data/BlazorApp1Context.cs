using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBlazorShopHosted.Libraries.Shared.Milk.Models;
using MyBlazorShopHosted.Libraries.Shared.Order;

namespace BlazorApp1.Data
{
    public class BlazorApp1Context : DbContext
    {
        public BlazorApp1Context (DbContextOptions<BlazorApp1Context> options)
            : base(options)
        {
        }

        public DbSet<MyBlazorShopHosted.Libraries.Shared.Milk.Models.SXProducts> SXProducts { get; set; } = default!;
        public DbSet<MyBlazorShopHosted.Libraries.Shared.Order.OrderModel> SXOrderId { get; set; } = default!;
    }
}
