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

        public async Task<IActionResult> Index(string userId, int collectionId)
        {
            var user = await _context.Users
            .Include(u => u.Collections) // Include the Collections navigation property
            .ThenInclude(c => c.CustomFields) // Include the CustomFields navigation property within Collections
            .FirstOrDefaultAsync(u => u.Id == userId);
            var collection = user.Collections.FirstOrDefault(x => x.Id == collectionId);
            return View(collection);
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
                // Create a new collection
                Collection collection = new Collection
                {
                    Name = model.Name,
                    Description = model.Description,
                    Category = model.Category,
                };

                // Retrieve the user
                var user = await _context.Users
                    .Include(u => u.Collections)
                    .FirstOrDefaultAsync(u => u.Id == Id);

                if (user != null)
                {
                    // Add custom fields to the collection
                    if (model.CustomFields != null && model.CustomFields.Any())
                    {
                        foreach (var customField in model.CustomFields)
                        {
                            // Create a new CustomField entity
                            CustomField newCustomField = new CustomField
                            {
                                Name = customField.Name,
                                FieldType = customField.FieldType,  
                                Collection = collection
                            };

                            // Add the new custom field to the collection
                            collection.CustomFields.Add(newCustomField);
                        }
                    }

                    // Set the user for the collection
                    collection.User = user;
                    collection.UserId = user.Id;

                    // Add the collection to the context
                    _context.Collections.Add(collection);
                    // Add the collection to the user's collections
                    user.Collections.Add(collection);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Redirect to the ManageUser/Index action
                    return RedirectToAction("Index", "ManageUser", new { userId = Id });
                }
            }

            // If the model state is not valid, return the view with errors
            return View(model);
        }
        public async Task<IActionResult> RemoveCollection(string userId, int? collectionId)
        {
            if (collectionId == null)
            {
                return NotFound();
            }
            var user = await _context.Users
            .Include(u => u.Collections) // Include the Collections navigation property
            .ThenInclude(c => c.CustomFields) // Include the CustomFields navigation property within Collections
            .FirstOrDefaultAsync(u => u.Id == userId);
            var collection = await _context.Collections.FindAsync(collectionId);
            if (collection != null)
            {
                user.Collections.Remove(collection);
                _context.Collections.Remove(collection);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction("Index", "ManageUser", new { userId = userId });
        }

        public IActionResult EditCollection(string userId, int? collectionId)
        {
            return RedirectToAction("Index", "ManageUser");
        }
    }
}
