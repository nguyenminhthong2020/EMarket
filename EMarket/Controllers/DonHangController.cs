﻿using EMarket.Data;
using EMarket.ModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMarket.Controllers
{
    public class DonHangController : Controller
    {
        private readonly dbMarketsContext _context;

        public DonHangController(dbMarketsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID)) return RedirectToAction("Login", "Accounts");

                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                if (khachhang == null) return NotFound();


                var donhang = await _context.Orders
                .Include(x => x.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id && Convert.ToInt32(taikhoanID) == m.CustomerId);
                if (donhang == null)
                {
                    return NotFound();
                }

                var chitietdonhang = _context.OrderDetails
                .Include(x => x.Product)
                .AsNoTracking()
                .Where(x => x.OrderId == id)
                .OrderBy(x => x.OrderDetailId)
                .ToList();

                XemDonHang donHang = new XemDonHang();
                donHang.DonHang = donhang;
                donHang.ChiTietDonHang = chitietdonhang;
                return PartialView("Details", donHang);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
