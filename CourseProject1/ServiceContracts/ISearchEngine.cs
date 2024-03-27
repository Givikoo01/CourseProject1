using CourseProject1.Models;

namespace CourseProject1.ServiceContracts
{
    public interface ISearchEngine
    {
        public Task<IEnumerable<Item>> SearchItems(string searchQuery);
    }
}
