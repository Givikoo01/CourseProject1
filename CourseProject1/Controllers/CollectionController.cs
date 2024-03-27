using CourseProject1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CourseProject1.Models;
using CourseProject1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CourseProject1.ServiceContracts;
namespace CourseProject1.Controllers
{
    public class CollectionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUploadImage _uploadimage;
        public CollectionController(AppDbContext context, UserManager<User> userManager, IUploadImage uploadImage)
        {
            _context = context;
            _userManager = userManager;
            _uploadimage = uploadImage;
        }

        public async Task<IActionResult> Index(string userId, int collectionId, string sortOrder)
        {
            var user = await _context.Users
                .Include(u => u.Collections)
                .ThenInclude(c => c.CustomFields)
                .Include(u => u.Collections)
                .ThenInclude(c => c.Items)
                .ThenInclude(cv => cv.CustomFieldValues)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var collection = user.Collections.FirstOrDefault(x => x.Id == collectionId);

            // Sorting logic
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TagsSortParm"] = sortOrder == "Tags" ? "tags_desc" : "Tags";

            switch (sortOrder)
            {
                case "name_desc":
                    collection.Items = collection.Items.OrderByDescending(s => s.Name).ToList();
                    break;
                case "Tags":
                    collection.Items = collection.Items.OrderBy(s => s.Tags).ToList();
                    break;
                case "tags_desc":
                    collection.Items = collection.Items.OrderByDescending(s => s.Tags).ToList();
                    break;
                default:
                    collection.Items = collection.Items.OrderBy(s => s.Name).ToList();
                    break;
            }

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

                Collection collection = new Collection
                {
                    Name = model.Name,
                    Description = model.Description,
                    Category = model.Category,
                    ImageUrl = await _uploadimage.UploadImageToCloudStorage(model.ImageFile)
                };

                // Retrieve the user
                var user = await _context.Users
                    .Include(u => u.Collections)
                    .FirstOrDefaultAsync(u => u.Id == Id);

                if (user != null)
                {
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
                            collection.CustomFields.Add(newCustomField);
                        }
                    }

                    // Set the user for the collection
                    collection.User = user;
                    collection.UserId = user.Id;

                    // Add the collection to the context
                    _context.Collections.Add(collection);
                    user.Collections.Add(collection);

                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "ManageUser", new { userId = Id });
                }
            }

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
        [HttpGet]
        public async Task<IActionResult> EditCollection(string userId, int collectionId)
        {
            var user = await _context.Users
                .Include(u => u.Collections)
                .ThenInclude(c => c.CustomFields)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var collection = await _context.Collections
                .Include(c => c.CustomFields)
                .FirstOrDefaultAsync(c => c.Id == collectionId);

            if (collection == null)
            {
                return NotFound();
            }

            var customFieldsViewModel = collection.CustomFields.Select(cf => new CustomFieldVM
            {
                Id = cf.Id,
                Name = cf.Name,
                FieldType = cf.FieldType
            }).ToList();

            var viewModel = new EditCollectionVM
            {
                Id = collection.Id,
                Name = collection.Name,
                Description = collection.Description,
                Category = collection.Category,
                UserId = collection.UserId,
                CustomFields = customFieldsViewModel
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditCollection(EditCollectionVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Update the collection object with the new data from the view model
            var user = await _context.Users
                .Include(u => u.Collections)
                .ThenInclude(c => c.CustomFields)
                .FirstOrDefaultAsync(u => u.Id == viewModel.UserId);

            var collection = await _context.Collections
                .Include(c => c.CustomFields)
                .FirstOrDefaultAsync(c => c.Id == viewModel.Id);

            if (collection == null)
            {
                return NotFound();
            }

            collection.Name = viewModel.Name;
            collection.Description = viewModel.Description;
            collection.Category = viewModel.Category;

            // Update existing custom fields
            foreach (var customFieldVM in viewModel.CustomFields.Where(vmcf => vmcf.Id != 0)) // Filter out fields with Id = 0 (new)
            {
                var existingCustomField = collection.CustomFields.FirstOrDefault(cf => cf.Id == customFieldVM.Id);

                if (existingCustomField != null)
                {
                    existingCustomField.Name = customFieldVM.Name;
                    existingCustomField.FieldType = customFieldVM.FieldType;
                    _context.CustomFields.Update(existingCustomField);
                }
            }

            // Add newly added custom fields (with Id = 0)
            foreach (var customFieldVM in viewModel.CustomFields.Where(vmcf => vmcf.Id == 0))
            {
                collection.CustomFields.Add(new CustomField
                {
                    Name = customFieldVM.Name,
                    FieldType = customFieldVM.FieldType,
                    Collection = collection // Set the Collection relationship
                });
            }

            // Remove deleted custom fields
            var customFieldsToRemove = collection.CustomFields.Where(cf => cf.Id != 0 && !viewModel.CustomFields.Any(vmcf => vmcf.Id == cf.Id)).ToList();
            _context.CustomFields.RemoveRange(customFieldsToRemove);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ManageUser", new { userId = collection.UserId });
        }
    }
}
