using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperProducer.Framework.Web
{
    /// <summary>
    /// 模型验证错误
    /// </summary>
    public class ModelStateError
    {
        /// <summary>
        /// 出错的模型key
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string msg { get; set; }
    }
}
