using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBlazorShopHosted.Libraries.Shared.Milk.Models;

namespace BlazorApp1.Data
{
    public class BlazorApp1Context : DbContext
    {
        public BlazorApp1Context (DbContextOptions<BlazorApp1Context> options)
            : base(options)
        {
        }

        public DbSet<MyBlazorShopHosted.Libraries.Shared.Milk.Models.SXProducts> SXProducts { get; set; } = default!;
    }
}
