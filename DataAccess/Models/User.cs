using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public int? UserId {  get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }

        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? Gender {  get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Image {  get; set; }
        public bool IsDisable { get; set; }


        public virtual ICollection<Blog> Blogs { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public Cart Cart { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<PreOrder> PreOrders { get; set; }

    }
}
