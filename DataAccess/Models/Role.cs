﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string RoleType { get; set; }

        public virtual ICollection<User> Users { get; set;}
    }
}
