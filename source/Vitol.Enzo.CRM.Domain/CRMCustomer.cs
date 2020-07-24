using System;
using System.Collections.Generic;
using System.Text;

namespace Vitol.Enzo.CRM.Domain
{
   public  class CRMCustomer
    {
        public uint sl_customeridexternal { get; set; }
        public string lastname { get; set; }
        public string emailaddress1 { get; set; }
        public string telephone1 { get; set; }
        public string address1_line1 { get; set; }
        public decimal? sl_longitude { get; set; }
        public decimal? sl_latitude { get; set; }
        public string sl_registrationnumber { get; set; }

    }
}
