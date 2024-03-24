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
        public virtual ICollection<CustomFieldValue> CustomFieldValues { get; set; }

        public Item()
        {
            CustomFieldValues = new List<CustomFieldValue>();
        }

    }
}