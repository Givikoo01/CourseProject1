using System.ComponentModel.DataAnnotations.Schema;

namespace CourseProject1.Models
{
    public class Like
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual User User { get; set; }
        [ForeignKey("Item")]
        public int ItemId { get; set; }
        public virtual Item Item { get; set; }
    }
}
