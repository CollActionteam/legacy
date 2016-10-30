using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CollAction.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(IStringLocalizer<HomeController> localizer)
        {
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Title"] = _localizer["About CollAction"];
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public ViewResult FAQ()
        {
            ViewData["Title"] = _localizer["Frequently Asked Questions"];
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
