using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;

namespace FinalProject.Data
{
    public class FinalProjectContext : DbContext
    {
        public FinalProjectContext (DbContextOptions<FinalProjectContext> options)
            : base(options)
        {
        }

        public DbSet<FinalProject.Models.Branches> Branch { get; set; }

        public DbSet<FinalProject.Models.Categories> Categories { get; set; }

        public DbSet<FinalProject.Models.Orders> Orders { get; set; }

        public DbSet<FinalProject.Models.Products> Products { get; set; }

        public DbSet<FinalProject.Models.CategoriesProducts> CategoriesProducts { get; set; }

        public DbSet<FinalProject.Models.OrderDetails> OrderDetails { get; set; }

        public DbSet<FinalProject.Models.Permissions> Permissions { get; set; }

        public DbSet<FinalProject.Models.Users> Users { get; set; }
    }
}
