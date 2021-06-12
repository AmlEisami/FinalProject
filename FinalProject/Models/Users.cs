using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{

    public enum UserPermission
    {
        Admin = 1,
        Editor,
        Client
    }

    public class Users
    {
        public int Id { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }

        public int PermissionsId { get; set; } = 3;

        public Permissions Permission { get; set; }

        public List<Orders> Orders { get; set; }
    }
}
