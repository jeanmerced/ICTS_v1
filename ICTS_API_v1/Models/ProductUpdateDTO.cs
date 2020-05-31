﻿using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    public class ProductUpdateDTO
    {
        [Required]
        public int? ProductId
        {
            get;
            set;
        }

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