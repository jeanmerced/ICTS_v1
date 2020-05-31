using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Data transfer object for creating products.
    /// used to transport data between processes.
    /// We will use these DTOs to represent the data we want the clients of our Web API to send.
    /// </summary>
    public class ProductDTO
    {
        //Lot id of the product
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_-]{13,17}",
            ErrorMessage = "Invalid input: LotId must contain between 13-17 characters, and can consist of letters, numbers, hyphens (-), and underscores (_).")]
        public string LotId
        {
            get;
            set;
        }


        // Foreign Key of Cart containing product
        [Required]
        public int? CartId
        {
            get;
            set;
        }
    }
}
