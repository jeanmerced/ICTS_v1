using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    public class LocationHistoryDTO
    {
        [Required]
        public int? CartId
        {
            get;
            set;
        }

        [Required]
        public int? SiteId
        {
            get;
            set;
        }

        [Required]
        public string CartCoordinates
        {
            get;
            set;
        }
    }
}