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
        [MinLength(2, ErrorMessage = "Username must contain at least 2 characters")]
        [MaxLength(15, ErrorMessage = "Username must contain at most 15 characters")]
        public string Username { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "Password must contain at least 8 characters")]
        [MaxLength(50, ErrorMessage = "Password must contain at most 50 characters")]
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
