using EMarket.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMarket.Controllers
{
    public class LocationController : Controller
    {
        private readonly dbMarketsContext _context;

        public LocationController(dbMarketsContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult QuanHuyenList(int LocationId)
        {
            var QuanHuyens = _context.Locations.OrderBy(x => x.LocationId)
            .Where(x => x.Parent == LocationId && x.Levels == 2)
            .OrderBy(x => x.Name)
            .ToList();
            return Json(QuanHuyens);
        }
        public ActionResult PhuongXalist(int LocationId)
        {
            var PhuongXas = _context.Locations.OrderBy(x => x.LocationId)
            .Where(x => x.Parent == LocationId && x.Levels == 3)
            .OrderBy(x => x.Name)
            .ToList();
            return Json(PhuongXas);
        }
    }
}
