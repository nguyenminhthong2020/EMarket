using System;
using System.Collections.Generic;
using System.ComponentModel;

#nullable disable

namespace EMarket.Models
{
    public partial class Account
    {
        [DisplayName("Id")]
        public int AccountId { get; set; }

        [DisplayName("Điện thoại")]
        public string Phone { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

        [DisplayName("Trạng thái")]
        public bool Active { get; set; }

        [DisplayName("Họ tên")]
        public string FullName { get; set; }

        public int? RoleId { get; set; }

        [DisplayName("Lần đăng nhập cuối")]
        public DateTime? LastLogin { get; set; }

        public DateTime? CreateDate { get; set; }

        public virtual Role Role { get; set; }
    }
}
