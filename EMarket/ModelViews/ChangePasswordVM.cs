using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMarket.ModelViews
{
    public class ChangePasswordVM
    {
        [Key]
        public int CustomerId { get; set; }
        [Display(Name = "Current password")]
        [DataType(DataType.Password)]
        public string PasswordNow { get; set; }

        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter password")]
        [MinLength(5, ErrorMessage = "You need to set a password of at least 5 characters")]
        public string Password { get; set; }


        [MinLength(5, ErrorMessage = "You need to set a password of at least 5 characters")]
        [Display(Name = "Enter confirm new password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords are not the same")]
        public string ConfirmPassword { get; set; }
    }
}
