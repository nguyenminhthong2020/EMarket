using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EMarket.ModelViews
{
    public class MuaHangVM
    {
        public int CustomerId
        { get; set; }
        [Required(ErrorMessage = "Please enter fullname")]
        public string FullName { get; set; }

        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter phone number")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Please enter delivery address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please choose province")]
        public int TinhThanh { get; set; }
        [Required(ErrorMessage = "Please choose district")]
        public int QuanHuyen { get; set; }
        [Required(ErrorMessage = "Please choose ward")]
        public int PhuongXa { get; set; }
        public int PaymentID { get; set; }
        public string Note { get; set; }
    }
}
