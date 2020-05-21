using System;
using System.Text.Json.Serialization;

namespace ICTS_API_v1.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string LotId { get; set; }

        public string ProductName { get; set; }

        public DateTime ExpirationDate { get; set; }

        public int Quantity { get; set; }

        public string VirtualSiteName { get; set; }

        public int CartId { get; set; } // Foreign Key

        [JsonIgnore]
        public Cart Cart { get; set; } // Navigation Property
    }
}
