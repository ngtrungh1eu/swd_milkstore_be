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
        public int Id {  get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool? Gender {  get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Image {  get; set; }
        public string? status { get; set; }
        public bool IsDisable { get; set; }
        public string? Email { get; set; }
        
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public Cart Cart { get; set; }
        public Favorite Favorite { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<PreOrder> PreOrders { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set;}
    }
}
