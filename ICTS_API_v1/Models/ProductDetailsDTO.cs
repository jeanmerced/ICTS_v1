using System;
using System.Text.Json.Serialization;

namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Data transfer object for sending product details.
    /// used to transport data between processes.
    /// We will use these DTOs to represent the data we want the clients of our Web API to receive.
    /// </summary>
    public class ProductDetailsDTO
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

        //Detects if discrepancy between the location of product and cart containing product exists
        public bool DiscrepancyExists
        {
            get
            {
                bool discrepancyExists = false;
                if (Cart != null)
                {
                    if (Cart.Site != null)
                    {
                        if (Cart.Site.SiteName != VirtualSiteName)
                        {
                            discrepancyExists = true;
                        }
                    }
                }
                return discrepancyExists;
            }
        }

        // Foreign Key of cart containing product
        public int? CartId
        {
            get;
            set;
        }

        //Navigation Property of cart containing product
        [JsonIgnore]
        public Cart Cart
        {
            get;
            set;
        }
    }
}
