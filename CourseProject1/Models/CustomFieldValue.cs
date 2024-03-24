using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CourseProject1.Models
{
    public class CustomFieldValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign key to the associated item
        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public virtual Item Item { get; set; }

        // Foreign key to the custom field definition
        [ForeignKey("CustomField")]
        public int CustomFieldId { get; set; }
        public virtual CustomField CustomField { get; set; }

        // Value of the custom field for this item
        public string Value { get; set; }
    }
}
