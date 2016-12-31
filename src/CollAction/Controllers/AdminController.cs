using CollAction.Data;
using CollAction.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController: Controller
    {
        public AdminController(
            UserManager<ApplicationUser> userManager,
            IStringLocalizer<AccountController> localizer,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _localizer = localizer;
            _context = context;
        }

        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        [HttpGet]
        public IActionResult Index()
            => View();

        [HttpGet]
        public IActionResult ManageProjectsIndex()
            => View();
    }
}
