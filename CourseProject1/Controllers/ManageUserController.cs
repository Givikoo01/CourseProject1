using CourseProject1.Data;
using CourseProject1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                var currentUser = _userManager.Users.Include(x => x.Collections).FirstOrDefault(x => x.Id == _userManager.GetUserAsync(User).Result.Id);
                return View(currentUser);
            }
            var profileUser = _userManager.Users.Include(x => x.Collections).FirstOrDefault(x => x.Id == userId);
            return View(profileUser);
        }
    }
}
