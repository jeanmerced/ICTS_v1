using System;
using System.Collections.Generic;

namespace ICTS_API_v1.Models
{
    public class Cart
    {
        public int CartId
        {
            get;
            set;
        }

        public string CartName
        {
            get;
            set;
        }

        public string TagAddress
        {
            get;
            set;
        }

        public DateTime? LastUpdated
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

        //Navigation Property
        public List<Product> Products
        {
            get;
            set;
        } 
    }
}