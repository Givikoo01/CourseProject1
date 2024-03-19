using CourseProject1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CourseProject1.Models;
using CourseProject1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace CourseProject1.Controllers
{
    public class CollectionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        public CollectionController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult AddCollection()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCollection(AddCollectionVM model, string Id)
        {
            if (ModelState.IsValid)
            {
                Collection collection = new Collection
                {
                    Name = model.Name,
                    Description = model.Description,
                    Category = model.Category,              
                };
                var user = await _context.Users
                .Include(u => u.Collections) // Include the Collections navigation property
                    .FirstOrDefaultAsync(u => u.Id == Id);
                if (user != null)
                { 
                    collection.User = user;
                    collection.UserId = user.Id;
                    _context.Collections.Add(collection);
                    user.Collections.Add(collection);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "ManageUser", new {userId = Id});
                }
            }
           return View();
        }
    }
}
