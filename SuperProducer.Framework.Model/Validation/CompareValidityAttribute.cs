using SuperProducer.Core.Utility;
using System;
using System.ComponentModel.DataAnnotations;

namespace SuperProducer.Framework.Model.Validation
{
    /// <summary>
    /// 大小比较[整数,小数,StringLength,DateTime]
    /// </summary>
    public class CompareValidityAttribute : BaseValidityAttribute
    {
        public enum CompareOp
        {
            greater,
            less,
            equals
        }

        public CompareOp OP { get; set; }

        public string OtherPropertyName { get; set; }

        public CompareValidityAttribute(CompareOp op, string otherPropertyName)
        {
            this.OP = op;
            this.OtherPropertyName = otherPropertyName;
        }

        public override bool IsValid(object value)
        {
            bool retVal = false;
            if (this.CurrentValidityObject != null)
            {
                var otherValue = ObjectHelper.GetPropertyValue(this.CurrentValidityObject, this.OtherPropertyName);
                if (otherValue != null)
                {
                    switch (this.OP)
                    {
                        case CompareOp.less:
                            {
                                if (this.CompareMethod(value, otherValue) == -1)
                                    retVal = true;
                            }
                            break;
                        case CompareOp.equals:
                            {
                                if (this.CompareMethod(value, otherValue) == 0)
                                    retVal = true;
                            }
                            break;
                        case CompareOp.greater:
                            {
                                if (this.CompareMethod(value, otherValue) == 1)
                                    retVal = true;
                            }
                            break;
                    }
                }
            }
            return retVal;
        }

        #region "比较方法"

        /// <summary>
        /// 比较方法[小于=-1,相等=0,大于=1]
        /// </summary>
        private short CompareMethod(object obj1, object obj2)
        {
            if (obj1 != null && obj2 != null)
            {
                if (obj1.GetType() == obj2.GetType() && obj1.GetType().IsValueType && obj2.GetType().IsValueType)
                {
                    if (ConvertHelper.IsIntegerType(obj1.GetType()))
                    {
                        if ((long)obj1 < (long)obj2)
                        {
                            return -1;
                        }
                        else if ((long)obj1 == (long)obj2)
                        {
                            return 0;
                        }
                        else if ((long)obj1 > (long)obj2)
                        {
                            return 1;
                        }
                    }
                    else if (ConvertHelper.IsDecimalsType(obj1.GetType()))
                    {
                        if ((decimal)obj1 < (decimal)obj2)
                        {
                            return -1;
                        }
                        else if ((decimal)obj1 == (decimal)obj2)
                        {
                            return 0;
                        }
                        else if ((decimal)obj1 > (decimal)obj2)
                        {
                            return 1;
                        }
                    }
                    else if (obj1 is string)
                    {
                        if (obj1.ToString().Length < obj2.ToString().Length)
                        {
                            return -1;
                        }
                        else if (obj1.ToString().Length == obj2.ToString().Length)
                        {
                            return 0;
                        }
                        else if (obj1.ToString().Length > obj2.ToString().Length)
                        {
                            return 1;
                        }
                    }
                    else if (obj1 is DateTime)
                    {
                        if ((DateTime)obj1 < (DateTime)obj2)
                        {
                            return -1;
                        }
                        else if ((DateTime)obj1 == (DateTime)obj2)
                        {
                            return 0;
                        }
                        else if ((DateTime)obj1 > (DateTime)obj2)
                        {
                            return 1;
                        }
                    }
                }
            }
            return short.MaxValue;
        }

        #endregion
    }
}
