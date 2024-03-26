using CourseProject1.Models;

namespace CourseProject1.ViewModels
{
    public class HomeIndexVM
    {
        public List<LatestItemVM> LatestItems { get; set; }
        public List<TopCollectionVM> TopCollections { get; set; }
    }
}
