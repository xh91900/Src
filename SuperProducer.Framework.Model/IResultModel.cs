using System.Collections.Generic;

namespace SuperProducer.Framework.Model
{
    public interface IResultModel
    {
        int code { get; set; }

        string msg { get; set; }

        dynamic data { get; set; }

        void Refresh();


        #region "内部属性"

        /// <summary>
        /// 消息字段格式化所需的参数列表
        /// </summary>
        List<object> MsgFormatParameter { get; set; }

        /// <summary>
        /// 程序执行结果正确时的结果码
        /// </summary>
        int RightCode { get; set; }

        /// <summary>
        /// 是否自动填充msg字段
        /// </summary>
        bool AutoChangeMsg { get; set; }

        /// <summary>
        /// 自动更改Code
        /// </summary>
        bool AutoChangeCode { get; set; }

        #endregion
    }
}
