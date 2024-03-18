using CourseProject1.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseProject1.Models
{
    public class User:IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public virtual ICollection<Collection> Collections { get; set; }
    }
}
