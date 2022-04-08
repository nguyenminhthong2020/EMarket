using EMarket.Data;
using EMarket.Helper;
using EMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly dbMarketsContext _context;
        public SearchController(dbMarketsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return View();

            return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
        }


        [HttpPost]
        public IActionResult FindProduct(string search, int catId, int page)
        {
            List<Product> ls = new List<Product>();
            if (string.IsNullOrEmpty(search) || search.Length < 1)
            {
                return PartialView("ListProductsSearchPartial", null);
            }

            var result = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.ProductName.Contains(search))
                .Where(x => (x.CatId - catId) * catId == 0);

            var count = result.Count();

            ViewBag.CatId = catId;
            ViewBag.Search = search;
            ViewBag.SoLuong = count;
            ViewBag.CurrentPage = page;
            ViewBag.Num = (count - (count % 10)) / 10 + (count % 10 > 0 ? 1 : 0);

            ls = result
               .OrderByDescending(x => x.ProductId)
               .Take(10)
               .ToList();

            if (ls == null)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("ListProductsSearchPartial", ls);
            }
        }
    }
}
