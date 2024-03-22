using CourseProject1.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseProject1.ViewModels
{
    public class EditCollectionVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        //public string ImageUrl { get; set; }
        [Required]
        public Category Category { get; set; }
    }
}
