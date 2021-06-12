using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProject.Data;
using FinalProject.Models;

namespace FinalProject.Controllers
{
    public class UsersController : Controller
    {
        private readonly FinalProjectContext _context;

        public UsersController(FinalProjectContext context)
        {
            _context = context;
        }

        // GET: Users/Login
        public IActionResult Login()
        {
            ViewData["PermissionsId"] = new SelectList(_context.Permissions, "Id", "PermissionName");
            return View();
        }

        // POST: Users/Login
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Id,Username,Password")] Users users)
        {
            var q = from user in _context.Users
                    where user.Username == users.Username && user.Password == users.Password
                    select user;

            if (q.Count() > 0)
            {
                return RedirectToAction(nameof(Index), "Home");
            }
            else
            {
                ViewData["Error"] = "Sorry, username and/or password are incorrect!";
            }
            ViewData["PermissionsId"] = new SelectList(_context.Permissions, "Id", "PermissionName", users.PermissionsId);
            return View(users);
        }

        // GET: Users/Register
        public IActionResult Register()
        {
            ViewData["PermissionsId"] = new SelectList(_context.Permissions, "Id", "PermissionName");
            return View();
        }

        // POST: Users/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Fullname,Username,Password,Email,Birthdate")] Users users)
        {
            if (ModelState.IsValid)
            {
                var username = _context.Users.FirstOrDefault(user => user.Username == users.Username);

                if (username == null)
                {
                    var email = _context.Users.FirstOrDefault(user => user.Email == users.Email);
                    if (email == null)
                    {
                        _context.Add(users);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), "Home");
                    } else
                    {
                        ViewData["Error"] = "Email is not available";
                    }
 
                }
                else
                {
                    ViewData["Error"] = "Username is not available";
                }
            }
            ViewData["PermissionsId"] = new SelectList(_context.Permissions, "Id", "PermissionName", users.PermissionsId);
            return View(users);
        }

        /*
                // GET: Users
                public async Task<IActionResult> Index()
                {
                    var finalProjectContext = _context.Users.Include(u => u.Permission);
                    return View(await finalProjectContext.ToListAsync());
                }

                // GET: Users/Details/5
                public async Task<IActionResult> Details(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var users = await _context.Users
                        .Include(u => u.Permission)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    if (users == null)
                    {
                        return NotFound();
                    }

                    return View(users);
                }

                // GET: Users/Edit/5
                public async Task<IActionResult> Edit(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var users = await _context.Users.FindAsync(id);
                    if (users == null)
                    {
                        return NotFound();
                    }
                    ViewData["PermissionsId"] = new SelectList(_context.Permissions, "Id", "PermissionName", users.PermissionsId);
                    return View(users);
                }

                // POST: Users/Edit/5
                // To protect from overposting attacks, enable the specific properties you want to bind to.
                // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
                [HttpPost]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> Edit(int id, [Bind("Id,Fullname,Username,Password,Email,Birthdate,PermissionsId")] Users users)
                {
                    if (id != users.Id)
                    {
                        return NotFound();
                    }

                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(users);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!UsersExists(users.Id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    ViewData["PermissionsId"] = new SelectList(_context.Permissions, "Id", "PermissionName", users.PermissionsId);
                    return View(users);
                }

                // GET: Users/Delete/5
                public async Task<IActionResult> Delete(int? id)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var users = await _context.Users
                        .Include(u => u.Permission)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    if (users == null)
                    {
                        return NotFound();
                    }

                    return View(users);
                }

                // POST: Users/Delete/5
                [HttpPost, ActionName("Delete")]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> DeleteConfirmed(int id)
                {
                    var users = await _context.Users.FindAsync(id);
                    _context.Users.Remove(users);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                private bool UsersExists(int id)
                {
                    return _context.Users.Any(e => e.Id == id);
                }*/
    }
}
