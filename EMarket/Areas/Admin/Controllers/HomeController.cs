using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IToastifyService _notifyService { get; }
        public HomeController(IToastifyService notifyService)
        {
            _notifyService = notifyService;
        }

        public IActionResult Index()
        {
            _notifyService.Success("Hiện lên nè :v");
            return View();
        }
    }
}
