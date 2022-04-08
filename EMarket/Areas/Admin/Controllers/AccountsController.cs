using EMarket.Data;
using EMarket.Extension;
using EMarket.Helper;
using EMarket.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly dbMarketsContext _context;
        public AccountsController(dbMarketsContext context)
        {
            _context = context;
        }

        [Route("admin/login.html")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("admin/login.html")]
        public async Task<IActionResult> Login(LoginViewModel acc1, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(acc1.UserName);
                    if (!isEmail) return View(acc1);

                    var acc = _context.Accounts.Include(a => a.Role)
                                    .AsNoTracking().SingleOrDefault(x => x.Email.Trim() == acc1.UserName);
                    if (acc == null)
                    {
                        return View(acc1);
                    };

                    string pass = (acc1.Password + acc.Salt.Trim()).ToMD5();
                    if (acc.Password != pass)
                    {
                        return View(acc1);
                    }

                    //Check account disable
                    if (acc.Active == false) return View(acc); // phải chuyển sang Trang thông báo bạn không còn là quản trị viên

                    //Save Session AccountId
                    HttpContext.Session.SetString("AdminAccountId", acc.AccountId.ToString());
                    //Save Session AccountId
                    HttpContext.Session.SetInt32("AdminRoleId", acc.RoleId.HasValue ? acc.RoleId.Value : 2);

                    var taikhoanID = HttpContext.Session.GetString("AdminAccountId");
                    var taikhoanRole = HttpContext.Session.GetInt32("AdminRoleId");

                    //Identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, acc.FullName),
                        new Claim(ClaimTypes.Role, taikhoanRole == 1 ? "admin": "staff"),
                        new Claim("AdminAccountId", acc.AccountId.ToString()),
                        new Claim("AdminRoleId", acc.RoleId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "adminlogin");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);


                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "Home", new { Area = "Admin" });
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            catch
            {
                return Redirect("/admin/login.html");
            }
            return View(acc1);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("adminlogin");
            HttpContext.Session.Remove("AdminAccountId");
            HttpContext.Session.Remove("AdminRoleId");

            return Redirect("/admin/login.html");
        }
    }
}
