using System;
using System.Text.Json.Serialization;
using NpgsqlTypes;

namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Data transfer object for sending location history details.
    /// used to transport data between processes.
    /// We will use these DTOs to represent the data we want the clients of our Web API to receive.
    /// </summary>
    public class LocationHistoryDetailsDTO
    {
        //Id of the location history
        public int RecordId
        {
            get;
            set;
        }

        //Foreign Key of the cart associated to record
        public int? CartId
        {
            get;
            set;
        }

        //Coordinates of the cart associated to record
        public NpgsqlPoint CartCoordinates
        {
            get;
            set;
        }

        //Name of the site where associated cart is located
        public string SiteName
        {
            get
            {
                string siteName = "UNKNOWN";
                if (Site != null)
                {
                    siteName = Site.SiteName;
                }
                return siteName;
            }
        }

        //Date of record creation
        public DateTime RecordDate
        {
            get;
            set;
        }

        //Navigation Property of site where associated cart was located
        [JsonIgnore]
        public Site Site
        {
            set;
            get;
        }
    }
}
