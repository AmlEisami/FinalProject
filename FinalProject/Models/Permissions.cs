using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Permissions
    {
        public int Id { get; set; }

        [Required]
        public string PermissionName { get; set; }

        public List<Users> Users { get; set; }
    }
}
