using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperProducer.Framework.Model
{
    public interface IClientContext
    {
        /// <summary>
        /// 用户语言
        /// </summary>
        CommonEnum.LanguageType ClientLanguage { get; set; }

        /// <summary>
        /// 平台类型
        /// </summary>
        byte PlatformType { get; set; }

        void Initialize();
    }
}
