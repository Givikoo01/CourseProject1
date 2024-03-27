using CourseProject1.Data;
using CourseProject1.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseProject1.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchEngine _searchEngine;
        public SearchController(ISearchEngine searchService)
        {
            _searchEngine = searchService;
        }

        public async Task<IActionResult> Index (string searchQuery)
        {
            var searchResults = await _searchEngine.SearchItems(searchQuery);
            return View(searchResults);
        }
    }
}
