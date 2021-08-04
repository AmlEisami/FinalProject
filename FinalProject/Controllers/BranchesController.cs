using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace FinalProject.Controllers
{
    public class BranchesController : Controller
    {
        private readonly FinalProjectContext _context;

        public BranchesController(FinalProjectContext context)
        {
            _context = context;
        }

        // GET: Branches
        public async Task<IActionResult> Index()
        {
            var finalProjectContext = from b in _context.Branch
                                      join u in _context.Users
                                      on b.UsersId equals u.Id
                                      select new Branches
                                      {
                                          Id = b.Id,
                                          BranchName = b.BranchName,
                                          Location = b.Location,
                                          UsersId = b.UsersId,
                                          BranchManager = u
                                      };
            ViewBag.permission = HttpContext.Session.GetString("Permission");
            return View(await finalProjectContext.ToListAsync());
        }


        // GET: Branches/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Username");
            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,BranchName,Location,UsersId")] Branches branches)
        {
            if (ModelState.IsValid)
            {
                _context.Add(branches);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "DisplayName", branches.UsersId);
            return View(branches);
        }

    }
}
