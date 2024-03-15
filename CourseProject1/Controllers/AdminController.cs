using CourseProject1.Data;
using CourseProject1.Models;
using CourseProject1.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UsersApp.Controllers;

namespace CourseProject1.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AdminController(ILogger<HomeController> logger, AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AdminPanel()
        {
            var users = _context.Users.ToList();
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> Block(string userIds)
        {
            if (userIds != null)
            {

                List<string> UserIds = userIds.Split(',').ToList();
                if (UserIds == null || UserIds.Count == 0)
                {
                    return BadRequest("No users selected for blocking.");
                }
                var currentUser = await _userManager.GetUserAsync(User);

                // Perform blocking logic using userIds
                foreach (var userId in UserIds)
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        user.IsActive = false;
                    }
                }
                // Check if the current user is among those being blocked
                if (currentUser != null && UserIds.Contains(currentUser.Id))
                {
                    // Log out the current user
                    await _signInManager.SignOutAsync();

                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminPanel"); // Redirect to appropriate action after blocking
        }

        [HttpPost]
        public async Task<IActionResult> Unblock(string userIds)
        {
            if (userIds != null)
            {
                List<string> UserIds = userIds.Split(',').ToList();
                if (UserIds == null || UserIds.Count == 0)
                {
                    return BadRequest("No users selected for unblocking.");
                }

                // Perform unblocking logic using userIds
                foreach (var userId in UserIds)
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        user.IsActive = true;
                    }
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminPanel"); // Redirect to appropriate action after unblocking
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userIds)
        {
            if (userIds != null)
            {
                List<string> UserIds = userIds.Split(',').ToList();
                if (UserIds == null || UserIds.Count == 0)
                {
                    return BadRequest("No users selected for deletion.");
                }

                // Perform deletion logic using userIds
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null && UserIds.Contains(currentUser.Id))
                {
                    // Delete and Log out the current user
                    _context.Users.Remove(currentUser);
                    await _signInManager.SignOutAsync();
                }
                foreach (var userId in UserIds)
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        _context.Users.Remove(user);
                    }
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("AdminPanel"); // Redirect to appropriate action after deletion
        }

        [HttpPost]
        public async Task<IActionResult> AddAdmin(string userIds)
        {
            if (userIds != null)
            {
                List<string> UserIds = userIds.Split(',').ToList();
                if (UserIds == null || UserIds.Count == 0)
                {
                    return BadRequest("No users selected for adding as Admin.");
                }
                // Perform adding as adming logic using userIds
                foreach (var userId in UserIds)
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        await _userManager.RemoveFromRoleAsync(user, Roles.Member.ToString());
                        await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminPanel"); // Redirect to appropriate action after unblocking
        }
        [HttpPost]
        public async Task<IActionResult> RemoveAdmin(string userIds)
        {
            if (userIds != null)
            {
                List<string> UserIds = userIds.Split(',').ToList();
                if (UserIds == null || UserIds.Count == 0)
                {
                    return BadRequest("No users selected for adding as Admin.");
                }

                // Perform removing as adming logic using userIds
                foreach (var userId in UserIds)
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        await _userManager.RemoveFromRoleAsync(user, Roles.Admin.ToString());
                        await _userManager.AddToRoleAsync(user, Roles.Member.ToString());
                    }
                }
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null && UserIds.Contains(currentUser.Id))
                {
                    //log out and sign in again to change roles
                    await _signInManager.SignOutAsync();
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login", "Account");
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminPanel"); // Redirect to appropriate action after unblocking
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
