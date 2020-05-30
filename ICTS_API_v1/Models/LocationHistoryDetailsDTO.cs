using System;
using System.Text.Json.Serialization;
using NpgsqlTypes;

namespace ICTS_API_v1.Models
{
    public class LocationHistoryDetailsDTO
    {
        public int RecordId
        {
            get;
            set;
        }

        public int? CartId
        {
            get;
            set;
        }

        public NpgsqlPoint CartCoordinates
        {
            get;
            set;
        }

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

        public DateTime RecordDate
        {
            get;
            set;
        }

        //Navigation Property
        [JsonIgnore]
        public Site Site
        {
            set;
            get;
        }
    }
}