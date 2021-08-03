using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalProject.Data;
using FinalProject.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

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
            if (HttpContext.Session.GetString("Permission") != null)
            {
                return RedirectToAction(nameof(Index), "Products");
            }
            ViewData["PermissionsId"] = new SelectList(_context.Permissions, "Id", "PermissionName");
            return View();
        }

        public IActionResult AccessDenied()
        {
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
                Signin(q.First());

                return RedirectToAction(nameof(Index), "Products");
            }
            else
            {
                ViewData["Error"] = "Sorry, username and/or password are incorrect!";
            }
            ViewData["PermissionsId"] = new SelectList(_context.Permissions, "Id", "PermissionName", users.PermissionsId);
            return View(users);
        }

        private async void Signin(Users account)
        {
            var permission = from permissions in _context.Permissions
                             where permissions.Id == account.PermissionsId
                             select permissions;

            HttpContext.Session.SetString("Permission", permission.First().PermissionName);
            HttpContext.Session.SetString("Username", account.Username);
            HttpContext.Session.SetString("Userid", account.Id.ToString());
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Role, permission.First().PermissionName),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

        // GET: Users/Register
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("Permission") != null)
            {
                return RedirectToAction(nameof(Index), "Products");
            }
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

                        var newUser = _context.Users.FirstOrDefault(user => user.Username == users.Username && user.Password == users.Password);

                        Signin(newUser);

                        return RedirectToAction(nameof(Index), "Products");
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


        [HttpGet]
        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("Userid") == null)
            {
                return View("Login");
            }

            if (HttpContext.Session.GetString("Permission") != "Admin" &&
                int.Parse(HttpContext.Session.GetString("Userid")) != id)
            {
                return View("AccessDenied");
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

        // GET: Users
        [Authorize(Roles = "Admin")]
        public async Task<ViewResult> Index(string username, string fullname, string email)
        {
            var users = await _context.Users.Where(u => (!String.IsNullOrEmpty(username) ? u.Username.ToLower().Contains(username.ToLower()) : true) &&
                                                  (!String.IsNullOrEmpty(fullname) ? u.Fullname.ToLower().Contains(fullname.ToLower()) : true) &&
                                                  (!String.IsNullOrEmpty(email) ? u.Email.ToLower().Contains(email.ToLower()) : true)).ToListAsync();
            return View( users);
        }


        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("Userid") == null)
            {
                return View("Login");
            }

            if (HttpContext.Session.GetString("Permission") != "Admin" &&
                int.Parse(HttpContext.Session.GetString("Userid")) != id)
            {
                return View("AccessDenied");
            }

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
                        return RedirectToAction(nameof(Index), "Products");
                    }
                    ViewData["PermissionsId"] = new SelectList(_context.Permissions, "Id", "PermissionName", users.PermissionsId);
                    return View(users);
                }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
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
                [Authorize(Roles = "Admin")]
                [HttpPost, ActionName("Delete")]
                [ValidateAntiForgeryToken]
                public async Task<IActionResult> DeleteConfirmed(int id)
                {
                    var users = await _context.Users.FindAsync(id);
                    _context.Users.Remove(users);
                    await _context.SaveChangesAsync();
                    if (int.Parse(HttpContext.Session.GetString("Userid")) == id)
                    {
                        await Signout();
                    }
                    return RedirectToAction(nameof(Index));
                }

                private bool UsersExists(int id)
                {
                    return _context.Users.Any(e => e.Id == id);
                }
    }
}
