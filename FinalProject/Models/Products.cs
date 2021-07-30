using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{

    public class Products
    {
        public int Id { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int Stock { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public string Video { get; set; }

        public List<Categories> Category { get; set; }
    }
    public class CombinedModel 
    {
        public IEnumerable<FinalProject.Models.Products> Products { get; set; }
        public IEnumerable<string> CategoryNames { get; set; }
    }
}
