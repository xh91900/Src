using System;
using System.ComponentModel.DataAnnotations;

namespace SuperProducer.Framework.Model.Validation
{
    public class YearRangeValidityAttribute : BaseValidityAttribute
    {
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }


        /// <summary>
        /// 日期校验[可校验值是否在一段时间内]
        /// </summary>
        /// <param name="addYearOfMinDate">例如:-5等于当前时间减5年</param>
        /// <param name="addYearOfMaxDate">例如:5等于当前时间加5年</param>
        /// <param name="useCurrentDay">是否使用当前天作为日期的天</param>
        public YearRangeValidityAttribute(int addYearOfMinDate, int addYearOfMaxDate, bool useCurrentDay = true)
        {
            this.MinDate = new DateTime(DateTime.Now.Year, 1, 1).AddYears(addYearOfMinDate);
            if (useCurrentDay)
                this.MaxDate = DateTime.Now.AddYears(addYearOfMaxDate);
            else
                this.MaxDate = new DateTime(DateTime.Now.Year, 1, 1).AddYears(addYearOfMaxDate);
        }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                DateTime currentValue;
                if (DateTime.TryParse(value.ToString(), out currentValue))
                {
                    if (currentValue > MinDate && currentValue < MaxDate)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
