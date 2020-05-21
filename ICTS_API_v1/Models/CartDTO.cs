using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    public class CartDTO
    { 
        //TODO: Unique
        [Required]
        [RegularExpression(@"xDECA+[A-Z0-9]{1,12}", ErrorMessage = "Invalid: TagAddress must start with \'xDECA\' followed by [A-Z0-9].")]
        [StringLength(17)]
        public string TagAddress { get; set; }
    }
}
