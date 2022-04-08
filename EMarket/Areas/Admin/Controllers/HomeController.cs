using AspNetCoreHero.ToastNotification.Abstractions;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            //string taikhoanID = HttpContext.Session.GetString("AdminAccountId");
            //int? taikhoanRole = HttpContext.Session.GetInt32("AdminRoleId");

            //if ((User.IsInRole("admin") || User.IsInRole("staff")) && taikhoanID != null && taikhoanRole != null)
            //{
            //    if (User.Identity.IsAuthenticated && User.IsInRole("admin"))
            //        return View();
            //}

            //return Redirect("/admin/login.html");
            return View();
        }
    }
}
