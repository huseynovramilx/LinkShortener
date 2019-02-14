﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkShortener.Areas.Admin.Models;
using LinkShortener.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LinkShortener.Services;
using LinkShortener.Models;

namespace LinkShortener.Areas.Admin.Controllers
{
   [Area("Admin")]
    public class HomeController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly PayPalHttpClientFactory _clientFactory;

        private List<UserPayoutVM> _userPayouts;
        public HomeController(ApplicationDbContext context, PayPalHttpClientFactory clientFactory)
        {
            _context = context;
            _clientFactory = clientFactory;
            PayoutBatch lastPayoutBatch = _context.PayoutBatches.OrderBy(p => p.ID).LastOrDefault();
            _userPayouts = _context.Links
            .Include(l => l.Owner)
            .Include(l => l.Clicks)
            .GroupBy(l => l.OwnerId)
            .Where(group => group.First().OwnerId != null)
            .Select(group => new UserPayoutVM
            {
                Email = group.First().Owner.Email,
                Payout = group.
                Sum(l => l.Clicks.Where(
                    c => (c.DateTime <= DateTime.Now.AddDays(-1)&&(lastPayoutBatch==null||c.DateTime >= lastPayoutBatch.ID)))
                .Count())
            }).Where(u => u.Payout !=0).ToList();

        }
        public IActionResult Index()
        {
            return View(_userPayouts);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPayouts(){
            PayoutBatch payoutBatch = await _context.AddPayoutBatch(_userPayouts);
            if (payoutBatch.PayoutPaypalBatchId == null)
            {
                PayPalHttpClient client = _clientFactory.GetClient();
                try
                {
                    await client.AuthorizeAsync();
                    await client.CreatePayouts(payoutBatch);
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return Json(e.Message);
                }
            }

            return View(viewName: "Index");
        }
    }
}