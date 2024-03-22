using CourseProject1.Models.Enums;
using System.ComponentModel.DataAnnotations;
namespace CourseProject1.ViewModels
{
    public class CustomFieldVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Custom field name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Custom field type is required")]
        public FieldType FieldType { get; set; }
    }
}
