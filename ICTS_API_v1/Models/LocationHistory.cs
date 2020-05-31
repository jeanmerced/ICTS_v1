using System;
using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Model class for location histories.
    /// Model objects retrieve and store model state in the persistance store like a database.
    /// </summary>
    public class LocationHistory
    {
        //Id of the location history
        [Key]
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

        //Date of record creation
        public DateTime RecordDate
        {
            get;
            set;
        }

        //Foreign Key of site where associated cart was located
        public int? SiteId
        {
            get;
            set;
        }

        //Navigation Property of site where associated cart was located
        public Site Site
        {
            get;
            set;
        }
    }
}
