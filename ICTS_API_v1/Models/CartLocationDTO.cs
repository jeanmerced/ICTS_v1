using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    public class CartLocationDTO
    {
        [Required]
        public int? CartId { get; set; }

        [Required]
        public int? SiteId { get; set; }

        // TODO: public TYPE Coordinates { get; set; }
    }
}
