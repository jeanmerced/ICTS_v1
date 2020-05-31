using System.ComponentModel.DataAnnotations;

namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Data transfer object for creating location histories.
    /// used to transport data between processes.
    /// We will use these DTOs to represent the data we want the clients of our Web API to send.
    /// </summary>
    public class LocationHistoryDTO
    {
        //Foreign Key of the cart associated to record
        [Required]
        public int? CartId
        {
            get;
            set;
        }

        //Foreign Key of site where associated cart was located
        [Required]
        public int? SiteId
        {
            get;
            set;
        }

        //Coordinates of the cart associated to record
        [Required]
        public string CartCoordinates
        {
            get;
            set;
        }
    }
}
