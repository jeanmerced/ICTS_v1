using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ICTS_API_v1.Models
{
    public class CartDetailsDTO
    {
        public int CartId { get; set; }

        public string CartName { get; set; }

        public string TagAddress { get; set; }

        public DateTime? LastUpdated { get; set; }

        // public TYPE Coordinates { get; set; }

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
 
        [JsonIgnore]
        public Site Site { get; set; }

        [JsonIgnore]
        public List<Product> Products { get; set; }
    }
}
