using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperProducer.Framework.Model.Log
{
    public class ModelChangeLog : ModelBase
    {
        public string UserName { get; set; }

        public string ModuleName { get; set; }

        public string TableName { get; set; }

        public long ModelID { get; set; }

        public string ModelValue { get; set; }

        public string EventType { get; set; }

        public string Remark { get; set; }
    }
}
