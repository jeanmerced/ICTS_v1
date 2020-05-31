
namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Data transfer object for sending site details.
    /// used to transport data between processes.
    /// We will use these DTOs to represent the data we want the clients of our Web API to receive.
    /// </summary>
    public class SiteDetailsDTO
    {
        //Id of the site
        public int SiteId
        {
            get;
            set;
        }

        //Name of the site
        public string SiteName
        {
            get;
            set;
        }

        //Reference coordinates of the site
        public NpgsqlTypes.NpgsqlBox RefCoordinates
        {
            get;
            set;
        }
    }
}
