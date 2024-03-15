using CourseProject1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using CourseProject1.Models;
using CourseProject1.Data;
namespace CourseProject1.Controllers
{
    public class CollectionController : Controller
    {
        private readonly AppDbContext _context;
        public CollectionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult AddCollection()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCollection(AddCollectionVM model)
        {
            if (ModelState.IsValid)
            {
                var collection = new Models.Collection
                {
                    Name = model.Name,
                    Description = model.Description,
                    Category = model.Category,
                };

                _context.Collections.Add(collection);
                _context.SaveChanges();
                return RedirectToAction("Index", "ManageUser");
            }
           return View();
        }
    }
}
