
namespace ICTS_API_v1.Models
{
    /// <summary>
    /// Model class for MPROC products.
    /// Model objects retrieve and store model state in the persistance store like a database.
    /// </summary>
    public class MPROCProduct
    {
        //VirtualSiteName
        public string STEPNAME
        {
            get;
            set;
        }

        //LotId
        public string LOTID
        {
            get;
            set;
        }

        //ProductName
        public string PRODUCTNAME
        {
            get;
            set;
        }

        //ExpirationDate
        public string USEBEFOREDATE
        {
            get;
            set;
        }

        public int LOT_CREATION_DATE
        {
            get;
            set;
        }

        public int LOT_TIMEHERESINCE_AGING
        {
            get;
            set;
        }

        //Quantity
        public int COMPONENTQTY
        {
            get;
            set;
        }
    }
}
