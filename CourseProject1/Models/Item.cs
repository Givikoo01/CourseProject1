using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseProject1.Models
{
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Standard properties
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Tags { get; set; } // Tags separated by commas
        [ForeignKey("Collection")]
        public int CollectionId { get; set; }
        public virtual Collection Collection { get; set; }
        public DateTime DateOfCreation { get; set; }
        public virtual ICollection<CustomFieldValue> CustomFieldValues { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }

        public Item()
        {
            CustomFieldValues = new List<CustomFieldValue>();
            Comments = new List<Comment>();
            Likes = new List<Like>();
        }

    }
}