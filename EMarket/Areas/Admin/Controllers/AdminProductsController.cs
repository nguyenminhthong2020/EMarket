using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EMarket.Data;
using EMarket.Models;
using PagedList.Core;
using EMarket.Helper;
using Microsoft.AspNetCore.Http;
using System.IO;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace EMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProductsController : Controller
    {
        private readonly dbMarketsContext _context;
        public IToastifyService _notifyService { get; }

        public AdminProductsController(dbMarketsContext context, IToastifyService toastifyService)
        {
            _context = context;
            _notifyService = toastifyService;
        }

        // GET: Admin/AdminProducts
        public IActionResult Index(int? page, int? catId, string search)
        {
            var pageNumber = page == null || page < 0 ? 1 : page.Value;
            if (catId != null && catId < 0)
            {
                catId = 0;
            }
            var pageSize = Utilities.PAGE_SIZE;

            // https://comdy.vn/entity-framework/dbcontext-va-dbset-trong-entity-framework/
            // Một số phương thức của DbSet:
            // Add, Find(id), Include, Remove,AsNoTracking
            // AsNoTracking: 
            // trả về 1 truy vấn mới, trong đó các thực thể trả về sẽ không lưu trong 
            // bộ nhớ cache trong DbContext (không được DbContext theo dõi)
            // --> tăng hiệu suất cho các thực thể chỉ đọc
            // nói với Entity Framework rằng không cần theo dõi kết quả của truy vấn,
            // và không cần xử lý lưu trữ bổ sung các giá trị được trả về
            var lstProduct = _context.Products
                                       .AsNoTracking()
                                       .Include(c => c.Cat);

            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", catId);
            ViewBag.Search = search;
            ViewBag.CatId = catId;
            ViewBag.CurrentPage = pageNumber;


            //TempData["CatId"] = catId;
            //TempData["Search"] = search;

            var lstProduct1 = lstProduct.Where(p => catId == null ? true : ((p.CatId - catId) * catId == 0))
                                        .Where(p => search == null ? true : p.ProductName.Contains(search))
                                        .OrderByDescending(c => c.ProductId);

            ViewBag.SoLuong = lstProduct1.Count();
            PagedList<Product> models = new PagedList<Product>(lstProduct1, pageNumber, pageSize);
            ViewBag.SoLuong = lstProduct1.Count();
            return View(models);

        }

        //// GET: Admin/AdminProducts/Filter
        //public JsonResult Filter(int catId)
        //{
        //    var pageNumber = 1;
        //    var pageSize = Utilities.PAGE_SIZE;
        //    var lstProduct = _context.Products
        //                               .AsNoTracking()
        //                               .Include(c => c.Cat)
        //                               .OrderBy(c => c.ProductId);

        //    if (catId == 0)
        //    {
        //        PagedList<Product> models = new PagedList<Product>(lstProduct, pageNumber, pageSize);
        //        ViewBag.CurrentPage = pageNumber;
        //        return Json(models);
        //    }

        //    var lstProduct1 = lstProduct.Where(p => p.CatId == catId);
        //    PagedList<Product> models1 = new PagedList<Product>(lstProduct1, pageNumber, pageSize);
        //    ViewBag.CurrentPage = pageNumber;

        //    return Json(models1);

        //}


        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ShortDesc,Description,CatId,Price,Discount,Thumb,Video,DateCreated,DateModified,BestSellers,HomeFlag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitsInStock")] Product product, IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                product.ProductName = Utilities.ToTitleCase(product.ProductName);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(product.ProductName) + extension;
                    product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }

                if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";
                product.Alias = Utilities.SEOUrl(product.ProductName);
                product.DateModified = DateTime.Now;
                product.DateCreated = DateTime.Now;

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ShortDesc,Description,CatId,Price,Discount,Thumb,Video,DateCreated,DateModified,BestSellers,HomeFlag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitsInStock")] Product product, IFormFile fThumb)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.ProductName = Utilities.ToTitleCase(product.ProductName);
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(product.ProductName) + extension;
                        product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                    }

                    if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";
                    product.Alias = Utilities.SEOUrl(product.ProductName);
                    product.DateModified = DateTime.Now;

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
