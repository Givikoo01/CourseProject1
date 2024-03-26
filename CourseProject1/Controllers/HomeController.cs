using CourseProject1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using CourseProject1.Data;
using CourseProject1.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace UsersApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public HomeController(ILogger<HomeController> logger, AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            var latestItems = await _context.Items
               .OrderByDescending(i => i.DateOfCreation)
               .Select(i => new LatestItemVM
               {
                   Name = i.Name,
                   Collection = i.Collection.Name,
                   CollectionId = i.CollectionId,
                   UserId = i.Collection.UserId,
                   ItemId = i.Id,
                   Author = i.Collection.User.UserName
               })
               .Take(5)
               .ToListAsync();

            var topCollections = await _context.Collections
                .OrderByDescending(c => c.Items.Count)
                .Select(c => new TopCollectionVM
                {
                    CollectionName = c.Name,
                    NumberOfItems = c.Items.Count,
                    CollectionId = c.Id,
                    UserId = c.UserId
                })
                .Take(5)
                .ToListAsync();

            var viewModel = new HomeIndexVM
            {
                LatestItems = latestItems,
                TopCollections = topCollections,
            };

            return View(viewModel);
        }
    }

}