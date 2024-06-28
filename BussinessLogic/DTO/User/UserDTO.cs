using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.DTO.User
{
    public class UserDTO
    {

        [Required]
        [Range(2, 15, ErrorMessage = "Username must contains as least 2 characters")]
        public string UserName { get; set; }
        [Required]
        [Range(8, 50, ErrorMessage = "Password must contains as least 8 characters")]
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? Gender { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool IsDisable { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }
        public int RoleId { get; set; }
    }
}
