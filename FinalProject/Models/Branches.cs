using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Branches
    {
        public int Id { get; set; }

        [Required]
        public string BranchName { get; set; }

        [Required]
        public string Location { get; set; }

        public int UsersId { get; set; }

        public Users BranchManager { get; set; }
    }
}
