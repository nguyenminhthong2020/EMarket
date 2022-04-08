using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace EMarket.Models
{
    public partial class Product
    {
        public Product()
        {
            AttributesPrices = new HashSet<AttributesPrice>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        [DisplayName("Id")]
        public int ProductId { get; set; }

        [DisplayName("Tên sản phẩm")]
        public string ProductName { get; set; }

        [DisplayName("Mô tả ngắn")]
        public string ShortDesc { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; }


        public int? CatId { get; set; }

        [DisplayName("Giá bán")]
        [DisplayFormat(DataFormatString = @"{0:n0} VNĐ")]
        public int? Price { get; set; }

        [DisplayName("Giảm giá")]
        public int? Discount { get; set; }

        [DisplayName("Ảnh")]
        public string Thumb { get; set; }

        public string Video { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime? DateCreated { get; set; }

        [DisplayName("Ngày sửa")]
        public DateTime? DateModified { get; set; }

        public bool BestSellers { get; set; }
        public bool HomeFlag { get; set; }

        [DisplayName("Trạng thái")]
        public bool Active { get; set; }
        public string Tags { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string MetaDesc { get; set; }
        public string MetaKey { get; set; }

        [DisplayName("Tồn kho")]
        public int? UnitsInStock { get; set; }

        public virtual Category Cat { get; set; }
        public virtual ICollection<AttributesPrice> AttributesPrices { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
