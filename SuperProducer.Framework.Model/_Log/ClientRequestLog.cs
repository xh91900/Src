using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperProducer.Framework.Model.Log
{
    public class ClientRequestLog : ModelBase
    {
        public string Url { get; set; }

        public string QueryParas { get; set; }

        public string FormParas { get; set; }

        public string HeaderParas { get; set; }

        public string IPAddress { get; set; }

        public long UserID { get; set; }
    }
}
