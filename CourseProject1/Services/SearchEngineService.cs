using CourseProject1.Data;
using CourseProject1.Models;
using CourseProject1.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.Linq;
using static CourseProject1.Services.SearchEngineService;

namespace CourseProject1.Services
{
    public class SearchEngineService : ISearchEngine
    {
        private readonly AppDbContext _context;
        public SearchEngineService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Item>> SearchItems(string searchQuery)
        {
            if (searchQuery == null)
            {
                return Enumerable.Empty<Item>();
            }
            //this allows user to search by item name, collection name, description etc..
            await _context.Users.Include(u => u.Collections).ToListAsync();
            var items = _context.Items
            .Where(item => EF.Functions.Contains(item.Name, $"\"{searchQuery}\"") || EF.Functions.Contains(item.Collection.Name, $"\"{searchQuery}\"") ||
            EF.Functions.Contains(item.Collection.User.UserName, $"\" {searchQuery} \"") || EF.Functions.Contains(item.Collection.User.FirstName, $"\"{searchQuery}\"") ||
            EF.Functions.Contains(item.Collection.User.LastName, $"\" {searchQuery} \"") || EF.Functions.Contains(item.Collection.Description, $"\" {searchQuery} \""));
            return items;
        }
    }
}
