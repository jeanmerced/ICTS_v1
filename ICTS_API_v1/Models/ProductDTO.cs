using System;
using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    public class ProductDTO
    {
        //Unique
        [Required]
        [StringLength(20)]
        public string LotId { get; set; }

        [Required]
        [StringLength(20)]
        public string ProductName { get; set; }

        //Nullable
        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [StringLength(30)]
        public string VirtualSiteName { get; set; }

        //Cannot default
        [Required]
        public int CartId { get; set; }
    }
}
