using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Data transfer object for sending cart details.
    /// used to transport data between processes.
    /// We will use these DTOs to represent the data we want the clients of our Web API to receive.
    /// </summary>
    public class CartDetailsDTO
    {
        //Id of the cart
        public int CartId
        {
            get;
            set;
        }

        //Name of the cart
        public string CartName
        {
            get;
            set;
        }

        //Address of tracking device attached to cart
        public string TagAddress
        {
            get;
            set;
        }

        //Date of the last time location was updated
        public DateTime? LastUpdated
        {
            get;
            set;
        }

        //Name of the site where cart is located
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

        //Detects if discrepancy between the location of cart and any product contained in cart exists
        public bool DiscrepancyExists
        {
            get
            {
                bool discrepancyExists = false;
                if (Products != null)
                {
                    foreach (var product in Products)
                    {
                        if (product.VirtualSiteName != SiteName)
                        {
                            discrepancyExists = true;
                            break;
                        }
                    }
                }
                return discrepancyExists;
            }
        }

        //Count of products contained in cart which expire within 7 days
        public int NearExpirationDateWarningCount
        {
            get
            {
                int nearExpirationDateWarningCount = 0;
                if (Products != null)
                {
                    foreach (var product in Products)
                    {
                        if (product.ExpirationDate.HasValue)
                        {
                            var dayDifference = (product.ExpirationDate.Value - DateTime.Today).TotalDays;
                            if (dayDifference <= 7 && dayDifference > 0)
                            {
                                nearExpirationDateWarningCount++;
                            }
                        }
                    }
                }
                return nearExpirationDateWarningCount;
            }
        }

        //Count of products contained in cart which expired
        public int ExpiredWarningCount
        {
            get
            {
                int expiredWarningCount = 0;
                if (Products != null)
                {
                    foreach (var product in Products)
                    {
                        if (product.ExpirationDate.HasValue)
                        {
                            var dayDifference = (product.ExpirationDate.Value - DateTime.Today).TotalDays;
                            if (dayDifference <= 0)
                            {
                                expiredWarningCount++;
                            }
                        }
                    }
                }
                return expiredWarningCount;
            }
        }

        //Navigation Property of site where cart is located
        [JsonIgnore]
        public Site Site
        {
            get;
            set;
        }

        //Navigation Property of list of product contained in cart
        [JsonIgnore]
        public List<Product> Products
        {
            get;
            set;
        }
    }
}
