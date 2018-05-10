using SuperProducer.Core.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperProducer.Framework.Model
{
    public class CommonEnum
    {
        /// <summary>
        /// 程序错误提示字符串
        /// </summary>
        public enum ProgErrorString
        {
            /// <summary>
            /// 成功
            /// </summary>
            Key_200 = 200,

            /// <summary>
            /// 未授权的访问
            /// </summary>
            Key_100001 = 100001,

            /// <summary>
            /// 参数错误
            /// </summary>
            Key_999996 = 999996,

            /// <summary>
            /// 服务器内部错误，请联系客服
            /// </summary>
            Key_999997 = 999997,

            /// <summary>
            /// 服务器内部错误，请稍重试
            /// </summary>
            Key_999998 = 999998,

            /// <summary>
            /// 未知错误
            /// </summary>
            Key_999999 = 999999
        }

        /// <summary>
        /// 语言类型
        /// </summary>
        public enum LanguageType
        {
            /// <summary>
            /// 简体-中国
            /// </summary>
            [EnumTitle("CN")]
            zhcn = 1,

            /// <summary>
            /// 英语-美国
            /// </summary>
            [EnumTitle("EN")]
            enus = 2
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public enum DbConnection
        {
            Main = 1,
            Res = 2,
            Log = 3
        }

        /// <summary>
        /// 模型代码区域
        /// </summary>
        public enum CodeRangeForModel
        {
            Start = 600000,
            End = 899999
        }
    }
}
