using System;
using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    public class LocationHistory
    {
        [Key]
        public int RecordID { get; set; }

        public DateTime DateAdded { get; set; }

        public double XCoordinate { get; set; }

        public double YCoordinate { get; set; }

        public int CartID { get; set; }

        public int SiteID { get; set; }
    }
}
