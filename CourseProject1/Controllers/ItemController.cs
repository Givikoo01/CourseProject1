using CourseProject1.Data;
using CourseProject1.Models;
using CourseProject1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseProject1.Controllers
{
    public class ItemController : Controller
    {
        private readonly AppDbContext _context;

        public ItemController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int collectionId,string userId, int itemId)
        {
            var user = await _context.Users
            .Include(u => u.Collections)
            .ThenInclude(c => c.CustomFields)
            .Include(u => u.Collections)
            .ThenInclude(c => c.Items)
            .ThenInclude(cv => cv.CustomFieldValues)
            .FirstOrDefaultAsync(u => u.Id == userId);

            var collection = user.Collections.FirstOrDefault(x => x.Id == collectionId);
            var item = collection.Items.FirstOrDefault(i => i.Id == itemId);
            return View(item);
        }
        [HttpGet]
        public async Task<IActionResult> AddItem(int collectionId, string userId)
        {
            var user = await _context.Users
            .Include(u => u.Collections)
            .ThenInclude(c => c.CustomFields)
            .Include(u => u.Collections)
            .ThenInclude(c => c.Items)
            .ThenInclude(cv => cv.CustomFieldValues)
            .FirstOrDefaultAsync(u => u.Id == userId);

            var collection = user.Collections.FirstOrDefault(x => x.Id == collectionId);

            // Convert custom fields from the collection to the view model format
            var customFieldsVM = collection.CustomFields.Select(cf => new CustomFieldVM
            {
                Name = cf.Name,
                FieldType = cf.FieldType
            }).ToList();

            var addItemVM = new AddItemVM
            {
                CollectionId = collectionId,
                CustomFields = customFieldsVM
            };

            return View(addItemVM);
        }
        [HttpPost]
        public async Task<IActionResult> AddItem(AddItemVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _context.Users
            .Include(u => u.Collections)
            .ThenInclude(c => c.CustomFields)
            .Include(u => u.Collections)
            .ThenInclude(c => c.Items)
            .ThenInclude(cv => cv.CustomFieldValues)
            .FirstOrDefaultAsync(u => u.Id == model.userId);
            var collection = user.Collections.FirstOrDefault(x => x.Id == model.CollectionId);
            var item = new Item
            {
                Name = model.Name,
                Tags = model.Tags,
                CollectionId = model.CollectionId,
                Collection = collection
            };
            collection.Items.Add(item);
            _context.Items.Add(item);
            _context.SaveChanges();
            var addedItem = collection.Items.FirstOrDefault(y => y.Id == item.Id);
            int counter = 0;
            foreach (var customField in collection.CustomFields)
            {
                var customFieldValue = new CustomFieldValue
                {
                    ItemId = addedItem.Id,
                    CustomFieldId = customField.Id,
                    Value = model.CustomFields[counter].Name,
                };
                counter++;
                item.CustomFieldValues.Add(customFieldValue);
                _context.CustomFieldValues.Add(customFieldValue);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Collection", new { collectionId = model.CollectionId, userId = model.userId });
        }
    }
}
