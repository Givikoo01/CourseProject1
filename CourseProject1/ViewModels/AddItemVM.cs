using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CourseProject1.Models;

namespace CourseProject1.ViewModels
{
    public class AddItemVM
    {
        public string userId {  get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Tags { get; set; } // Tags separated by commas

        public int CollectionId { get; set; }

        public List<CustomFieldVM> CustomFields { get; set; } 

        public AddItemVM()
        {
            CustomFields = new List<CustomFieldVM>();
        }

    }
}
