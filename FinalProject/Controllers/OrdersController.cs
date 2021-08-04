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
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace FinalProject.Controllers
{
    public class OrdersController : Controller
    {
        static HttpClient client = new HttpClient();
        private readonly FinalProjectContext _context;

        public OrdersController(FinalProjectContext context)
        {
            _context = context;
        }

        // GET: Orders
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string Address, string price)
        {
            var finalProjectContext = _context.Orders.Include(o => o.User)
                .Where( o => (!String.IsNullOrEmpty(price) ? ((decimal)o.OrderPrice) <= (decimal)Convert.ToDouble(price) : true) &&
                             (!String.IsNullOrEmpty(Address) ? o.Address.ToLower().Contains(Address.ToLower()) : true));
            return View(await finalProjectContext.ToListAsync());
        }

        // GET: Orders/Details/5
        [Authorize(Roles = "Admin, Editor, Client")]
        public async Task<IActionResult> MyOrders()
        {
                var path = "http://data.fixer.io/api/latest?access_key=758b556a8a2bc33fafde24780c31dbea&symbols=USD,CAD,JPY";
                var wether = "";
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    wether = response.Content.ReadAsStringAsync().Result;
                    var ssss = Newtonsoft.Json.JsonConvert.DeserializeObject(wether);
                    ViewBag.wether = ssss;
                var a = "";
            }
                
            

            var userId = Convert.ToInt32(HttpContext.Session.GetString("Userid"));

            var myOrders = _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Product).Where(m => m.UsersId == userId);


            return View(await myOrders.ToListAsync());
        }

        public async Task<bool> CreateOrder(string address)
        {
            var orderPrice = 0.0;
            Orders order = new();
            order.UsersId = int.Parse(HttpContext.Session.GetString("Userid"));
            order.Address = address;
            order.OrderDate = DateTime.Now;

            JObject dynamicCart = JObject.Parse(HttpContext.Session.GetString("cart"));
            var numOfOD = 0;
            foreach (var property in dynamicCart)
            {
                if (int.Parse(property.Value.ToString()) > 0)
                {
                    numOfOD++;
                    var product = from p in _context.Products
                                  where p.Id == int.Parse(property.Key)
                                  select p;

                    orderPrice += product.First().Price * int.Parse(property.Value.ToString());
                }
            }
            if (numOfOD > 0) 
            {
                order.OrderPrice = orderPrice;
                _context.Add(order);
                await _context.SaveChangesAsync();
                var orderDetailsRange = new OrderDetails[numOfOD];
                var id = 0;
                foreach (var property in dynamicCart)
                {
                    if (int.Parse(property.Value.ToString()) > 0)
                    {

                        orderDetailsRange[id] = new();
                        orderDetailsRange[id].ProductsId = int.Parse(property.Key);
                        orderDetailsRange[id].Amount = int.Parse(property.Value.ToString());
                        orderDetailsRange[id].OrdersId = order.Id;
                        id++;
                    }
                }
                _context.AddRange(orderDetailsRange);
                await _context.SaveChangesAsync();
            } else
            {
                return false;
            }
            HttpContext.Session.Remove("cart");
            return true;
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id", orders.UsersId);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UsersId,Address,OrderPrice,OrderDate")] Orders orders)
        {
            if (id != orders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersExists(orders.Id))
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
            ViewData["UsersId"] = new SelectList(_context.Users, "Id", "Id", orders.UsersId);
            return View(orders);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(orders);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
