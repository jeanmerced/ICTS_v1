using System;
using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    public class LocationHistory
    {
        [Key]
        public int RecordId { get; set; }

        public int? CartId { get; set; }

        public int? SiteId { get; set; }

        public DateTime RecordDate { get; set; }

       // public Type CartCoordinates { get; set; }
    }
}
