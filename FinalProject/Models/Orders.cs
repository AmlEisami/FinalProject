using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Orders
    {
        public int Id { get; set; }

        public int UsersId { get; set; }

        public Users User { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public double OrderPrice { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public List<OrderDetails> OrderDetails { get; set; }
    }
}
