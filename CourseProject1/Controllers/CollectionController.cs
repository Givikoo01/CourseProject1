using CourseProject1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CourseProject1.Models;
using CourseProject1.Data;
using Microsoft.AspNetCore.Identity;
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
                var user = await _userManager.FindByIdAsync(Id);         
                Collection collection = new Collection
                {
                    Name = model.Name,
                    Description = model.Description,
                    Category = model.Category,
                   
                };
                if (user != null)
                {
                    collection.User = user;
                    collection.UserId = Id;
                    _context.Collections.Add(collection);
                    user.Collections.Add(collection);
                    var result = await _context.SaveChangesAsync();
                    if (result != 0)
                    {
                        return RedirectToAction("Index", "ManageUser");
                    }
                }
               
               
                
            }
           return View();
        }
    }
}
