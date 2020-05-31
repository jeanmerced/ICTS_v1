using System;
using System.Collections.Generic;

namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Model class for carts.
    /// Model objects retrieve and store model state in the persistance store like a database.
    /// </summary>
    public class Cart
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

        //Foreign Key of site where cart is located
        public int? SiteId
        { 
            get;
            set;
        }

        //Navigation Property of site where cart is located
        public Site Site
        {
            get;
            set;
        }

        //Navigation Property of list of product contained in cart
        public List<Product> Products
        {
            get;
            set;
        } 
    }
}
