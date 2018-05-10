using SuperProducer.Framework.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperProducer.Framework.Model.Res
{
    public class SystemExtendTypeInfo : ModelBase
    {
        public string TypeCode { get; set; }

        public string CnName { get; set; }

        public string EnName { get; set; }

        public string Remark { get; set; }
    }

    [MultiLanguage("SystemExtendTypeData")]
    public class SystemExtendTypeData : ModelBase
    {
        public long TypeID { get; set; }

        public string DataKey { get; set; }

        public string DataValue { get; set; }

        public string DataRemark { get; set; }
    }
}
