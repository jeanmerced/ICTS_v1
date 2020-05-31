using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Data transfer object for updating products.
    /// used to transport data between processes.
    /// We will use these DTOs to represent the data we want the clients of our Web API to send.
    /// </summary>
    public class ProductUpdateDTO
    {
        //Id of the product
        [Required]
        public int? ProductId
        {
            get;
            set;
        }

        //Virtual location of product according to MPROC
        [Required]
        [RegularExpression(@"[a-zA-Z0-9_-]{4,30}",
            ErrorMessage = "Invalid input: SiteName must contain between 4-30 characters, and can consist of letters, numbers, hyphens (-) and underscores (_).")]
        public string VirtualSiteName
        {
            get;
            set;
        }
    }
}
