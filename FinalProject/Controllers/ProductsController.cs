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
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using Facebook;

namespace FinalProject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly FinalProjectContext _context;

        public ProductsController(FinalProjectContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<ViewResult> Index(string searchString, string price, string categoryNames)
        {
            CombinedModel prodAndCat = new CombinedModel();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            var currentCategory = await _context.Categories.Where(c => c.CategoryName == categoryNames).ToListAsync();
            prodAndCat.CategoryNames = await _context.Categories.Select(c => c.CategoryName).ToListAsync();
            prodAndCat.Products = (from product in _context.Products
                                   from categories in product.Category
                                   select new Products
                                   {
                                       Id = product.Id,
                                       Image = product.Image,
                                       Price = product.Price,
                                       ProductName = product.ProductName,
                                       Video = product.Video,
                                       Description = product.Description,
                                       Category = product.Category
                                   }).AsEnumerable().Where(p => (!String.IsNullOrEmpty(price) ? ((decimal)p.Price) <= (decimal)Convert.ToDouble(price) : true) &&
                                                 (!String.IsNullOrEmpty(searchString) ? p.ProductName.ToLower().Contains(searchString.ToLower()) : true) &&
                                                 (!String.IsNullOrEmpty(categoryNames) ? p.Category.Exists(c => c.CategoryName == categoryNames) : true))
                                                 .GroupBy(p => p.Id).Select(p => p.First());
                                                 
            return View(prodAndCat);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Shopping Cart
        public async Task<IActionResult> ShoppingCart(string searchString, string price)
        {
            if (HttpContext.Session.GetString("cart") == null)
            {
                return View();
            }
            
            JObject dynamicCart = JObject.Parse(HttpContext.Session.GetString("cart"));
            var cart = new int[dynamicCart.Count];
            var i = 0;
            foreach (var property in dynamicCart)
            {
                cart[i] = int.Parse(property.Key);
                i++;
            }
            var products = (from product in _context.Products
                           where cart.Contains(product.Id)
                           select product).Where(p => (!String.IsNullOrEmpty(price) ? ((decimal)p.Price) <= (decimal)Convert.ToDouble(price) : true) &&
                                                      (!String.IsNullOrEmpty(searchString) ? p.ProductName.Contains(searchString) : true));
            ViewData["amount"] = new SelectList(dynamicCart);
            return View(await products.ToListAsync());
        }

        // GET: add item to cart
        public void AddItemToCart(int itemId, int amount)
        {
            JObject dynamicCart = new JObject();
            if (HttpContext.Session.GetString("cart") == null)
            {
                dynamicCart.Add(itemId.ToString(), amount.ToString());

            } else
            {
                dynamicCart = JObject.Parse(HttpContext.Session.GetString("cart"));
                if (dynamicCart[itemId.ToString()] != null)
                {
                    amount = amount + int.Parse(dynamicCart[itemId.ToString()].ToString());
                }
                dynamicCart[itemId.ToString()] = amount.ToString();
            }
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dynamicCart));
        }

        // GET: remove item from cart
        public void RemoveItemFromCart(int itemId)
        {
            JObject dynamicCart = JObject.Parse(HttpContext.Session.GetString("cart"));
            dynamicCart.Remove(itemId.ToString());
            if (dynamicCart.Count == 0)
            {
                HttpContext.Session.Remove("cart");
            }
            else
            {
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dynamicCart));
                var cart = new int[dynamicCart.Count];
                var i = 0;
                foreach (var property in dynamicCart)
                {
                    cart[i] = int.Parse(property.Key);
                    i++;
                }
                var products = from product in _context.Products
                               where cart.Contains(product.Id)
                               select product;
                ViewData["amount"] = new SelectList(dynamicCart);
            }
        }

        // GET: change amount of product in shopping cart
        public void ChangeQuantity(int itemId, int amount)
        {
            JObject dynamicCart = JObject.Parse(HttpContext.Session.GetString("cart"));
            dynamicCart[itemId.ToString()] = amount.ToString();
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dynamicCart));
        }

       // GET: Products/Create
       [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["categories"] = new SelectList(_context.Categories, nameof(Categories.Id), nameof(Categories.CategoryName));
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductName,Price,Description,Image")] Products products, int[] Category)
        {
            if (ModelState.IsValid)
            {
                products.Category = new List<Categories>();
                products.Category.AddRange(_context.Categories.Where(x => Category.Contains(x.Id)));

                _context.Add(products);
                await _context.SaveChangesAsync();
                var pageId = "139699228237162";

                dynamic messagePost = new ExpandoObject();
                messagePost.access_token = "EAAYh1wlHGjkBAKmaWH6NR1T13G6CEzCQAc8zlm3XweblGM51TVLikDV75uF8sVJqNs3b9q2SPqSoGHdyZBKmGT7kPzjzQrZB8hixQ0yTF0rDW91V2hQgmv9deXcmCTAU4SVHhp6XuYFNE8s7iI2afs6jgHVce1OfU4RZB6YlZA808YU0gbdj2KmKte1Kw3MZD";
                messagePost.message = "A new product has been published! Come check out \"" + products.ProductName + "\"";
                messagePost.description = "Yay";

                FacebookClient app = new FacebookClient("EAAYh1wlHGjkBAKmaWH6NR1T13G6CEzCQAc8zlm3XweblGM51TVLikDV75uF8sVJqNs3b9q2SPqSoGHdyZBKmGT7kPzjzQrZB8hixQ0yTF0rDW91V2hQgmv9deXcmCTAU4SVHhp6XuYFNE8s7iI2afs6jgHVce1OfU4RZB6YlZA808YU0gbdj2KmKte1Kw3MZD");
                    var result = app.Post("/" + pageId + "/feed", messagePost);

                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,Price,Description,Image")] Products products)
        {
            if (id != products.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.Id))
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
            return View(products);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.Products.FindAsync(id);
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
