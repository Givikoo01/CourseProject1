using CourseProject1.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseProject1.Models
{
    public class CustomField
    {
        [Key]
        public int Id { get; set; }
        public FieldType FieldType { get; set; }
        public string Name { get; set; }

        [ForeignKey("Collection")]
        public int CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
