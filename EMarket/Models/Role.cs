using System;
using System.Collections.Generic;
using System.ComponentModel;

#nullable disable

namespace EMarket.Models
{
    public partial class Role
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
        }

        [DisplayName("Id")]
        public int RoleId { get; set; }

        [DisplayName("Tên gọi")]
        public string RoleName { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
