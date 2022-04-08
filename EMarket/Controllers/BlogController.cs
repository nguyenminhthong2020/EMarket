using EMarket.Data;
using EMarket.Helper;
using EMarket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMarket.Controllers
{
    public class BlogController : Controller
    {
        private readonly dbMarketsContext _context;

        public BlogController(dbMarketsContext context)
        {
            _context = context;
        }

        //https://tedu.com.vn/lap-trinh-aspnet-core/attribute-routing-trong-aspnet-core-230.html
        //[Route("blogs.html", Name = "Blog")]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page < 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            var lst = _context.TinDangs
                                       .AsNoTracking()
                                       .OrderByDescending(c => c.CreatedDate);

            var count = lst.Count();
            ViewBag.Num = (count - (count % 10)) / 10 + (count % 10 > 0 ? 1 : 0);


            PagedList<TinDang> models = new PagedList<TinDang>(lst, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        [Route("/tin-tuc/{Alias}-{id}.html", Name = "TinDetails")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tinDang = await _context.TinDangs.AsNoTracking().SingleOrDefaultAsync(x => x.PostId == id);
            if (tinDang == null)
            {
                return NotFound();
            }

            var LsRelatedPost = _context.TinDangs.AsNoTracking()
                                        .Where(x => x.Published == true && x.PostId != id)
                                        .Take(3)
                                        .OrderByDescending(x => x.CreatedDate)
                                        .ToList();
            ViewBag.LsRelatedPost = LsRelatedPost;
            return View(tinDang);
        }
    }
}
