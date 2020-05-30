﻿using System;
using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace ICTS_API_v1.Models
{
    public class LocationHistory
    {
        [Key]
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

        public DateTime RecordDate
        {
            get;
            set;
        }

        //Foreign Key
        public int? SiteId
        {
            get;
            set;
        }

        //Navigation Property
        public Site Site
        {
            get;
            set;
        }
    }
}