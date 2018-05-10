using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using SuperProducer.Core.Cache;
using SuperProducer.Core.Utility;

namespace SuperProducer.Framework.Model.Attributes
{
    /// <summary>
    /// 为模型启用多语言
    /// </summary>
    public class MultiLanguageAttribute : TableAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">模型对应的数据库表名</param>
        public MultiLanguageAttribute(string name) : base(name)
        {
            this.SetMultiLanguage();
        }

        private void SetMultiLanguage()
        {
            var field = typeof(TableAttribute).GetField("_name", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                var clientContext = CacheHelper.GetItem<IClientContext>(() => { return null; });
                if (clientContext != null)
                {
                    var langName = EnumHelper.GetEnumTitle(clientContext.ClientLanguage);
                    if (!string.IsNullOrEmpty(langName))
                        field.SetValue(this, string.Format("{0}_{1}", this.Name, langName));
                }
            }
        }
    }
}
