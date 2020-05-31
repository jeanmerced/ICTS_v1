using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    public class CartUpdateDTO
    {
        [Required]
        public int? CartId
        {
            get;
            set;
        }

        [Required]
        [RegularExpression(@"[a-zA-Z0-9_-]{6,16}",
            ErrorMessage = "Invalid input: CartName must contain between 6-16 characters, and can consist of letters, numbers, hyphens (-) and underscores (_).")]
        public string CartName
        {
            get;
            set;
        }

        [Required]
        [RegularExpression(@"xDECA+[A-Z0-9]{9,12}",
            ErrorMessage = "Invalid input: TagAddress must contain 14-17 characters starting with \'xDECA\' followed by a combination of letters and numbers.")]
        public string TagAddress
        {
            get;
            set;
        }
    }
}