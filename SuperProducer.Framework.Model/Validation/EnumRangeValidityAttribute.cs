using SuperProducer.Core.Utility;
using System;

namespace SuperProducer.Framework.Model.Validation
{
    public class EnumRangeValidityAttribute : BaseValidityAttribute
    {
        public Type EnumType { get; set; }

        public EnumRangeValidityAttribute(Type enumType)
        {
            this.EnumType = enumType;
        }

        public override bool IsValid(object value)
        {
            return EnumHelper.IsDefined(this.EnumType, value);
        }
    }
}
