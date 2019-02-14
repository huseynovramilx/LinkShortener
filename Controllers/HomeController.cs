﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LinkShortener.Models;
using LinkShortener.Data;

namespace LinkShortener.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [Route("{id:maxlength(4)?}")]
        public IActionResult Index([FromRoute]string id)
        {
            if(id is null)
                return View();
            Link link = _context.Links.FirstOrDefault(l => l.Id == id);
            
            if (link is null)
                return View();
            else
            {
                link.Clicks.Add(new Click
                {
                    DateTime = DateTime.Now,
                    LinkId = link.Id
                });

                _context.SaveChanges();

                return View(viewName: "IndexId", model: link.FullUrl);
            }
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
