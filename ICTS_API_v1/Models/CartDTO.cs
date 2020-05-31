using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Data transfer object for creating carts.
    /// used to transport data between processes.
    /// We will use these DTOs to represent the data we want the clients of our Web API to send.
    /// </summary>
    public class CartDTO 
    {
        //Name of the cart
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_-]{6,16}",
            ErrorMessage = "Invalid input: CartName must contain between 6-16 characters, and can consist of letters, numbers, hyphens (-) and underscores (_).")]
        public string CartName
        {
            get;
            set;
        }

        //Address of tracking device attached to cart
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
