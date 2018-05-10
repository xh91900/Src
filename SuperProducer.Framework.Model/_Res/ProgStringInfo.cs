using SuperProducer.Framework.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperProducer.Framework.Model.Res
{
    [MultiLanguage("ProgStringInfo")]
    public class ProgStringInfo : ModelBase
    {
        public string StringKey { get; set; }

        public string StringValue { get; set; }

        public string Remark { get; set; }
    }
}
