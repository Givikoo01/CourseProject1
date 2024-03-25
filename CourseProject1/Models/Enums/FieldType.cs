using System.ComponentModel.DataAnnotations;

namespace CourseProject1.Models.Enums
{
    public enum FieldType
    {
        [Display(Name = "String")]
        Line,
        Numeric,
        [Display(Name = "Date / Time")]
        Date,
        Logical
    }
}
