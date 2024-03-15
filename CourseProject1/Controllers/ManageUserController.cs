using CourseProject1.Data;
using CourseProject1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject1.Controllers
{
    public class ManageUserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        public ManageUserController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;    
            _context = context;
        }
        public async Task<IActionResult> Index(string userId)
        {
            if (userId == null)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                return View(currentUser);
            }
            var profile = await _userManager.FindByIdAsync(userId);
            return View(profile);
        }
    }
}
