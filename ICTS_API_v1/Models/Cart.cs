using System;
using System.Collections.Generic;

namespace ICTS_API_v1.Models
{
    public class Cart
    {
        public int CartId { get; set; }

        public string CartName { get; set; }

        public string TagAddress { get; set; }

        public DateTime? LastUpdated { get; set; }

        // TODO: public TYPE Coordinates { get; set; }

        public int? SiteId { get; set; } // Foreign Key

        public Site Site { get; set; } // Navigation Property

        public List<Product> Products { get; set; } // Navigation Property
    }
}
