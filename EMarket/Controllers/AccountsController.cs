using AspNetCoreHero.ToastNotification.Abstractions;
using EMarket.Data;
using EMarket.Extension;
using EMarket.Helper;
using EMarket.Models;
using EMarket.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EMarket.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly dbMarketsContext _context;
        public INotyfService _notifyService { get; }

        public AccountsController(dbMarketsContext context, INotyfService notyfService)
        {
            _context = context;
            _notifyService = notyfService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("my-account.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    var lsOrder = _context.Orders
                        .Include(x => x.TransactStatus)
                        .AsNoTracking()
                        .Where(x => x.CustomerId == khachhang.CustomerId)
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();
                    ViewBag.lsOrder = lsOrder;
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login", "Accounts");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public IActionResult DangKyTaiKhoan()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public async Task<IActionResult> DangKyTaiKhoan(RegisterVM taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomKey();
                    Customer khachhang = new Customer
                    {
                        FullName = taikhoan.FullName,
                        Phone = taikhoan.Phone.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        Password = (taikhoan.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt,
                        CreateDate = DateTime.Now
                    };
                    try
                    {
                        _context.Add(khachhang);
                        await _context.SaveChangesAsync();
                        //Lưu Session Makh
                        HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
                        var taikhoanID = HttpContext.Session.GetString("CustomerId");
                        //Identity
                        var claims = new List<Claim>
                        {
                             new Claim(ClaimTypes.Name, khachhang.FullName),
                             new Claim("CustomerId", khachhang.CustomerId.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);

                        //Xem thêm:
                        // https://www.red-gate.com/simple-talk/development/dotnet-development/policy-based-authorization-in-asp-net-core-a-deep-dive/#:~:text=The%20Authorize%20attribute%20enables%20you,if%20the%20user%20is%20authenticated.
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    catch (Exception ex)
                    {
                        var m = ex;
                        return RedirectToAction("DangKyTaiKhoan", "Accounts");
                    }
                }
                else
                {
                    return View(taikhoan);
                }
            }
            catch
            {
                return View(taikhoan);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (khachhang != null)
                    return Json(data: "Phone number : " + Phone + " already exist");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (khachhang != null)
                    return Json(data: "Email : " + Email + " already exist");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }

        [AllowAnonymous]
        [Route("login.html", Name = "Login")]
        public IActionResult Login()
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                return RedirectToAction("Dashboard", "Accounts");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login.html", Name = "Login")]
        public async Task<IActionResult> Login(LoginViewModel customer, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);

                    var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.UserName);
                    if (khachhang == null)
                    {
                        return View(customer);
                    };

                    string pass = (customer.Password + khachhang.Salt.Trim()).ToMD5();
                    if (khachhang.Password != pass)
                    {
                        return View(customer);
                    }

                    //Check account disable
                    if (khachhang.Active == false) return RedirectToAction("ThongBao", "Accounts");

                    //Save Session CustomerId
                    HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("CustomerId");
                    //Identity
                    var claims = new List<Claim>
                    {
                        // new Claim(JwtRegisteredClaimNames.Sub, email),
                        // new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                        // https://stackoverflow.com/questions/32584074/whats-the-role-of-the-claimsprincipal-why-does-it-have-multiple-identities
                        // https://www.red-gate.com/simple-talk/development/dotnet-development/policy-based-authorization-in-asp-net-core-a-deep-dive/#:~:text=The%20Authorize%20attribute%20enables%20you,if%20the%20user%20is%20authenticated
                        
                        // nếu hiểu
                        // Principal = User
                        // Identity = Driver's License, Passport, Credit Card, Google Account, Facebook Account, RSA SecurID, Finger print, Facial recognition, etc.
                        // https://andrewlock.net/introduction-to-authentication-with-asp-net-core/
                        new Claim(ClaimTypes.Name, khachhang.FullName),
                        //new Claim(ClaimTypes.Role, "admin"),
                        new Claim("CustomerId", khachhang.CustomerId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);

                    // vd khác:
                    //const string Issuer = "https://gov.uk";
                    //var claims = new List<Claim> {
                    //    new Claim(ClaimTypes.Name, "Andrew", ClaimValueTypes.String, Issuer),
                    //    new Claim(ClaimTypes.Surname, "Lock", ClaimValueTypes.String, Issuer),
                    //    new Claim(ClaimTypes.Country, "UK", ClaimValueTypes.String, Issuer),
                    //    new Claim("ChildhoodHero", "Ronnie James Dio", ClaimValueTypes.String)
                    //};

                    //var userIdentity = new ClaimsIdentity(claims, "Passport");
                    //var userPrincipal = new ClaimsPrincipal(userIdentity);

                    //await HttpContext.Authentication.SignInAsync("Cookie", userPrincipal,
                    //    new AuthenticationProperties
                    //    {
                    //        ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    //        IsPersistent = false,
                    //        AllowRefresh = false
                    //    });

                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            catch
            {
                return RedirectToAction("DangkyTaikhoan", "Accounts");
            }
            return View(customer);
        }


        [HttpGet]
        [Route("logout.html", Name = "Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("login"); // khác với "adminlogin" bên Area Admin
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var taikhoanID = HttpContext.Session.GetString("CustomerId");
                    if (taikhoanID == null)
                    {
                        return RedirectToAction("Login", "Accounts");
                    }
                    var taikhoan = _context.Customers.Find(Convert.ToInt32(taikhoanID));
                    if (taikhoan == null) return RedirectToAction("Login", "Accounts");

                    var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
                    if (pass == taikhoan.Password)
                    {
                        string passnew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
                        taikhoan.Password = passnew;
                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        _notifyService.Success("Đổi mật khẩu thành công !");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                }
            }
            catch
            {
                return RedirectToAction("Dashboard", "Accounts");
            }
            return RedirectToAction("Dashboard", "Accounts");
        }
    }
}
