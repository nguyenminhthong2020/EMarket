using EMarket.Extension;
using EMarket.ModelViews;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMarket.Controllers.Components
{
    public class HeaderCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            return View(cart);
        }
    }
}
//public class SampleViewComponent : ViewComponent
//{
//    private readonly DbContext db;
//    public SampleViewComponent(DbContext context)
//    {
//        db = context;
//    }

//    public async Task<IViewComponentResult> InvokeAsync()
//    {
//        // Do somethings
//        return View(Data);
//    }
//}

