using System;
using System.ComponentModel.DataAnnotations;

namespace SuperProducer.Framework.Model.Validation
{
    public class BaseValidityAttribute : ValidationAttribute
    {
        /// <summary>
        /// 当前验证对象
        /// </summary>
        protected object CurrentValidityObject { get; set; }

        /// <summary>
        /// 当前验证值
        /// </summary>
        protected object CurrentValidityValue { get; set; }

        /// <summary>
        /// 当前验证的成员名称
        /// </summary>
        protected string CurrentValidityMemberName { get; set; }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.CurrentValidityValue = value;
            this.CurrentValidityObject = validationContext.ObjectInstance;
            this.CurrentValidityMemberName = validationContext.MemberName;
            
            return base.IsValid(value, validationContext);
        }
    }
}
