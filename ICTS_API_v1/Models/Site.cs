namespace ICTS_API_v1.Models
{
    public class Site
    {
        public int SiteId
        {
            get;
            set;
        }

        public string SiteName
        {
            get;
            set;
        }

        public NpgsqlTypes.NpgsqlBox RefCoordinates
        {
            get;
            set;
        }
    }
}
