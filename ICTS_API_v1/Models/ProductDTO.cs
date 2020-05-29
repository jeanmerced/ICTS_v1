using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    public class ProductDTO
    {
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_-]{13,17}",
            ErrorMessage = "Invalid input: LotId must contain between 13-17 characters, and can consist of letters, numbers, hyphens (-), and underscores (_).")]
        public string LotId
        {
            get;
            set;
        }

        [Required]
        public int? CartId
        {
            get;
            set;
        }
    }
}