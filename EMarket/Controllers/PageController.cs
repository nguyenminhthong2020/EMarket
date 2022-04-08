using EMarket.Data;
using EMarket.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMarket.Controllers
{
    public class PageController : Controller
    {
        private readonly dbMarketsContext _context;

        public PageController(dbMarketsContext context)
        {
            _context = context;
        }


        [Route("/page/{Alias}", Name = "PageDetails")]
        public async Task<IActionResult> Details(string Alias)
        {
            if (string.IsNullOrEmpty(Alias))
            {
                return RedirectToAction("Index", "Home");
            }

            var page = await _context.Pages.AsNoTracking().SingleOrDefaultAsync(x => x.Alias == Alias);
            if (page == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(page);
        }
    }
}
