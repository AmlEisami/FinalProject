using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }

        public int ProductsId { get; set; }

        public Products ProductName { get; set; }

        [Required]
        public int Amount { get; set; }

        public int OrdersId { get; set; }

        public Orders Order { get; set; }

    }
}
