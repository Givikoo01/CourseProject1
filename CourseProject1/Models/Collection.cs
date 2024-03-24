using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CourseProject1.Models.Enums;

namespace CourseProject1.Models
{
    public class Collection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public Category Category { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<CustomField> CustomFields { get; set; }
        public Collection()
        {
            Items = new List<Item>();
            CustomFields = new List<CustomField>();
        }
    }
}
