using EMarket.Data;
using EMarket.Models;
using EMarket.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EMarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly dbMarketsContext _context;

        public HomeController(ILogger<HomeController> logger, dbMarketsContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewVM model = new HomeViewVM();

            var lsProducts = _context.Products.AsNoTracking()
            .Where(x => x.Active == true && x.HomeFlag == true)
            .OrderByDescending(x => x.DateCreated)
            .ToList();

            List<ProductHomeVM> lsProductViews = new List<ProductHomeVM>();

            var lsCats = _context.Categories
                .AsNoTracking()
                .Where(x => x.Published == true)
                .OrderByDescending(x => x.Ordering)
                .ToList();

            foreach (var item in lsCats)
            {
                ProductHomeVM productHome = new ProductHomeVM();
                productHome.category = item;
                productHome.lsProducts = lsProducts.Where(x => x.CatId == item.CatId).ToList();
                lsProductViews.Add(productHome);
            }

            var quangcao = _context.QuangCaos
                .AsNoTracking()
                .FirstOrDefault(x => x.Active == true);

            var TinTuc = _context.TinDangs
                .AsNoTracking()
                .Where(x => x.Published == true && x.IsNewfeed == true)
                .OrderByDescending(x => x.CreatedDate)
                .Take(3)
                .ToList();

            model.Products = lsProductViews;
            model.quangcao = quangcao;
            model.TinTucs = TinTuc;
            ViewBag.AllProducts = lsProducts;

            return View(model);
        }


        [Route("lien-he.html", Name = "Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [Route("lien-he.html", Name = "Contact")]
        public async Task<IActionResult> Contact(ContactVM model)
        {
            if (ModelState.IsValid)
            {

                //    MailMessage message = new MailMessage(
                //    from: model.Email,
                //    to: "songoku.minhthong@gmail.com",
                //    subject: $"[Customer - Contact- {model.Name}]",
                //    body: model.Message
                //);
                //    message.BodyEncoding = System.Text.Encoding.UTF8;
                //    message.SubjectEncoding = System.Text.Encoding.UTF8;
                //    message.IsBodyHtml = true;
                //    message.ReplyToList.Add(new MailAddress(model.Email));
                //    message.Sender = new MailAddress(model.Email);

                try
                {
                    //await client.SendMailAsync(message);
                    //return View(model);
                }
                catch (Exception ex)
                {
                    return View(model);
                }

            }
            return View(model);
        }

        [Route("gioi-thieu.html", Name = "About")]
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
