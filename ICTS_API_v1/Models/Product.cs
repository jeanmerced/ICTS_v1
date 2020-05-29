using System;

namespace ICTS_API_v1.Models
{
    public class Product
    {
        public int ProductId
        {
            get;
            set;
        }

        public string LotId
        {
            get;
            set;
        }

        public string ProductName
        {
            get;
            set;
        }

        public DateTime? ExpirationDate
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }

        public string VirtualSiteName
        {
            get;
            set;
        }

        // Foreign Key
        public int? CartId
        {
            get;
            set;
        }

        // Navigation Property
        public Cart Cart
        {
            get;
            set;
        } 
    }
}