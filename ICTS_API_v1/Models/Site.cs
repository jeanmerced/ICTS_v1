
namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Model class for sites.
    /// Model objects retrieve and store model state in the persistance store like a database.
    /// </summary>
    public class Site
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
