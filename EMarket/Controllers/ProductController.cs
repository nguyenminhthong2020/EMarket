using EMarket.Data;
using EMarket.Helper;
using EMarket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMarket.Controllers
{
    public class ProductController : Controller
    {
        private readonly dbMarketsContext _context;

        public ProductController(dbMarketsContext context)
        {
            _context = context;
        }

        [Route("shop.html", Name = "ShopProduct")]
        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page == null || page < 0 ? 1 : page.Value;
                var pageSize = Utilities.PAGE_SIZE;
                var lst = _context.Products
                                           .AsNoTracking()
                                           .Include(p => p.Cat)
                                           .OrderByDescending(c => c.DateCreated);

                var count = lst.Count();
                ViewBag.Num = (count - (count % 10)) / 10 + (count % 10 > 0 ? 1 : 0);


                PagedList<Product> models = new PagedList<Product>(lst, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [Route("/danh-sach-san-pham/{Alias}", Name = "ListProduct")]
        public IActionResult List(string Alias, int page = 1)
        {
            try
            {
                var danhmuc = _context.Categories.AsNoTracking().SingleOrDefault(item => item.Alias == Alias);


                var pageSize = Utilities.PAGE_SIZE;
                var lst = _context.Products
                                           .AsNoTracking()
                                           .Include(p => p.Cat)
                                           .Where(x => x.CatId == danhmuc.CatId)
                                           .OrderByDescending(c => c.DateCreated);

                var count = lst.Count();
                ViewBag.Num = (count - (count % 10)) / 10 + (count % 10 > 0 ? 1 : 0);


                PagedList<Product> models = new PagedList<Product>(lst, page, pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = danhmuc;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [Route("/{Alias}-{id}.html", Name = "ProductDetails")]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var p = await _context.Products.Include(p => p.Cat).FirstOrDefaultAsync(p => p.ProductId == id);
                if (p == null)
                {
                    return NotFound();
                }
                var relatedProduct = _context.Products.AsNoTracking().Where(x => x.CatId == p.CatId && x.ProductId != id && x.Active == true)
                                                      .OrderByDescending(_ => _.DateCreated)
                                                      .Take(4).ToList();

                ViewBag.SanPhamLienQuan = relatedProduct;

                return View(p);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
    }
}
