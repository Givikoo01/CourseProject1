namespace CourseProject1.ViewModels
{
    public class EditItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public int CollectionId { get; set; }
        public string UserId { get; set; }
        public List<CustomFieldForItemVM> CustomFields { get; set; } = new List<CustomFieldForItemVM>();
    }
}
