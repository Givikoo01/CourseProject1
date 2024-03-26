using System.ComponentModel.DataAnnotations;

namespace CourseProject1.ViewModels
{
    public class EditVM
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }

        public string UserId { get; set; }
    }
}
