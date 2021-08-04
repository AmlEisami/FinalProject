using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class ExchangeController : Controller
    {
        static HttpClient client = new HttpClient();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexAsync()
        {
            var path = "http://data.fixer.io/api/latest?access_key=758b556a8a2bc33fafde24780c31dbea&sympols=USD,GBP,AUD";
            var wether = "";
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                wether = await response.Content.ReadAsStringAsync();
            }
            ViewBag.wether = wether;

            return View();
        }
    }
}
