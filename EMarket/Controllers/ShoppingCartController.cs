using AspNetCoreHero.ToastNotification.Abstractions;
using EMarket.Data;
using EMarket.Extension;
using EMarket.Models;
using EMarket.ModelViews;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMarket.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly dbMarketsContext _context;
        public INotyfService _notifyService { get; }

        public ShoppingCartController(dbMarketsContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // Đầu tiên, phải khởi tạo 1 giỏ hàng RỖNG
        // CartItem là 1 class có các thuộc tính 
        //    public Product product { get; set; }
        //    public int amount { get; set; }
        //    public double TotalMoney => amount * product.Price.Value;
        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }

        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            List<CartItem> cart = GioHang;
            try
            {
                //Thêm sản phẩm vào gió hàng
                CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null) //đã có --> cập nhật số lượng
                {
                    item.amount = item.amount + amount.Value;
                    //lưu lại session
                    //HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                else
                {
                    Product hh = _context.Products.SingleOrDefault(p => p.ProductId == productID);
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = hh
                    };
                    cart.Add(item); //thêm vào gió
                }
                //lưu lại session
                HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                //_notifyService.Success("Thêm vào giỏ hàng thành công!");
                ViewBag.CurrentNumber = amount.Value;
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }


        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            //Lấy giỏ hàng ra để dxuwr lý
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                    if (item != null && amount.HasValue)//đã có --> cập nhật số lượng
                    {
                        item.amount = amount.Value;
                    }

                    //lưu lại session
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }


        [HttpPost]
        [Route("api/cart/remove")]
        public ActionResult Remove(int productID)
        {
            try
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                //lưu lại session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                _notifyService.Success("Remove product in cart successfully!");

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }


        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
    }
}
