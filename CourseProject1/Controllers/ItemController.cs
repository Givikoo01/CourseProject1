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
        public async Task<IActionResult> Index(int collectionId, string userId, int itemId)
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
                CustomFields = customFieldsVM,
                userId = userId
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
                Collection = collection,
                DateOfCreation = DateTime.Now
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
        [HttpGet]
        public async Task<IActionResult> EditItem(int collectionId, string userId, int itemId)
        {
            var user = await _context.Users
                .Include(u => u.Collections)
                .ThenInclude(c => c.CustomFields)
                .Include(u => u.Collections)
                .ThenInclude(c => c.Items)
                .ThenInclude(i => i.CustomFieldValues)
                .ThenInclude(cv => cv.CustomField)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var collection = user.Collections.FirstOrDefault(x => x.Id == collectionId);
            var item = collection.Items.FirstOrDefault(i => i.Id == itemId);
            var customFieldViewModels = item.CustomFieldValues.Select(cv => new CustomFieldForItemVM
            {
                Id = cv.CustomField.Id,
                Value = cv.Value,
                Name = cv.CustomField.Name
            }).ToList();

            var editItemViewModel = new EditItemVM
            {
                Id = item.Id,
                Name = item.Name,
                Tags = item.Tags,
                CollectionId = collectionId,
                UserId = userId,
                CustomFields = customFieldViewModels
            };

            return View(editItemViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(EditItemVM model)
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
        .ThenInclude(i => i.CustomFieldValues)
        .ThenInclude(cv => cv.CustomField)
        .FirstOrDefaultAsync(u => u.Id == model.UserId);

        var collection = user.Collections.FirstOrDefault(x => x.Id == model.CollectionId);
        var item = collection.Items.FirstOrDefault(i => i.Id == model.Id);

        item.Name = model.Name;
        item.Tags = model.Tags;

        var existingCustomFieldIds = collection.CustomFields.Select(cf => cf.Id).ToList();

        foreach (var customFieldViewModel in model.CustomFields)
        {
            var customFieldValue = item.CustomFieldValues.FirstOrDefault(cv => cv.CustomFieldId == customFieldViewModel.Id);

            if (customFieldValue != null)
            {
                customFieldValue.Value = customFieldViewModel.Value;
            }
            else
            {
                var newCustomFieldValue = new CustomFieldValue
                {
                    ItemId = item.Id,
                    CustomFieldId = customFieldViewModel.Id,
                    Value = customFieldViewModel.Value
                };

                item.CustomFieldValues.Add(newCustomFieldValue);
                _context.CustomFieldValues.Add(newCustomFieldValue);
            }
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Collection", new { collectionId = model.CollectionId, userId = model.UserId });
        }
        public async Task<IActionResult> RemoveItem(int collectionId, string userId, int itemId)
        {
            var user = await _context.Users
                .Include(u => u.Collections)
                .ThenInclude(c => c.Items)
                .ThenInclude(i => i.CustomFieldValues)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var collection = user.Collections.FirstOrDefault(x => x.Id == collectionId);
            var item = collection.Items.FirstOrDefault(i => i.Id == itemId);

            // Detach the related CustomFieldValue entities
            _context.CustomFieldValues.RemoveRange(item.CustomFieldValues);

            // Remove the Item entity
            _context.Items.Remove(item);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Collection", new { collectionId = collectionId, userId = userId });
        }
    }
}
