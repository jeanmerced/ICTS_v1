using System;

namespace ICTS_API_v1.Models
{
    public class MPROCProduct
    {
        public string STEPNAME { get; set; }

        public string LOTID { get; set; }

        public string PRODUCTNAME { get; set; }

        public string USEBEFOREDATE { get; set; }

        public int LOT_CREATION_DATE { get; set; }

        public int LOT_TIMEHERESINCE_AGING { get; set; }

        public int COMPONENTQTY { get; set; }
    }
}
