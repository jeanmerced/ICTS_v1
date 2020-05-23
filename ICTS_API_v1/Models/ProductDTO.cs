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

        //Cannot default
        [Required]
        public int CartId { get; set; }
    }
}
