using CourseProject1.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseProject1.ViewModels
{
    public class AddCollectionVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public Category Category { get; set; } 
        //public string ImageUrl { get; set; }

        //additional custom fields
    }
}
