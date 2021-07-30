using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Categories
    {
        public int Id { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public List<Products> MyProperty { get; set; }
    }
}
