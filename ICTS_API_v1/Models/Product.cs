using System;

namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Model class for products.
    /// Model objects retrieve and store model state in the persistance store like a database.
    /// </summary>
    public class Product
    {
        //Id of the product
        public int ProductId
        {
            get;
            set;
        }

        //Lot id of the product
        public string LotId
        {
            get;
            set;
        }

        //Product name of the product
        public string ProductName
        {
            get;
            set;
        }

        //Expiration date of the product
        public DateTime? ExpirationDate
        {
            get;
            set;
        }

        //Product quantity in lot
        public int Quantity
        {
            get;
            set;
        }

        //Virtual location of product according to MPROC
        public string VirtualSiteName
        {
            get;
            set;
        }

        // Foreign Key of cart containing product
        public int? CartId
        {
            get;
            set;
        }

        // Navigation Property of cart containing product
        public Cart Cart
        {
            get;
            set;
        } 
    }
}
