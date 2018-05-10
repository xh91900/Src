using System;
using System.Text;
using System.Collections.Generic;

namespace SuperProducer.Core.Utility
{
    public class InternalConstant
    {
        /// <summary>
        /// 默认的MS数据库最小时间
        /// </summary>
        public static readonly DateTime DefaultMSdbMinDate = DateTime.Parse("1753-01-01");

        /// <summary>
        /// 默认的网络请求超时时间[毫秒]
        /// </summary>
        public static readonly int DefaultNetworkRequestTimeoutMS = 60000;

        /// <summary>
        /// 默认的网络响应超时时间[毫秒]
        /// </summary>
        public static readonly int DefaultNetworkResponseTimeoutMS = 60000;

        /// <summary>
        /// 默认的队列项最大数量
        /// </summary>
        public static readonly int DefaultQueueItemMaxCount = 1024;

        /// <summary>
        /// 默认的编码格式
        /// </summary>
        public static readonly Encoding DefaultEncode = Encoding.UTF8;

        /// <summary>
        /// 默认的用户代理[PC]
        /// </summary>
        public static readonly Dictionary<string, string> DefaultUserAgentForPC = new Dictionary<string, string>()
        {
            { "Firefox", "Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.04" },
            { "Chrome", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36" },
            { "InternetExplorer", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko" },
        };

        /// <summary>
        /// 缓存Key前缀
        /// </summary>
        public enum CacheKeyPrefixes
        {
            /// <summary>
            /// 文件配置缓存
            /// </summary>
            ConfigFile = 1,

            /// <summary>
            /// 数据库配置缓存
            /// </summary>
            ConfigDatabase,

            /// <summary>
            /// 服务缓存
            /// </summary>
            Service,

            /// <summary>
            /// 程序缓存
            /// </summary>
            Program,
        }

        /// <summary>
        /// AES加密Key
        /// </summary>
        public static readonly string AESKey = "AodAYIcls5@^1TspvVzOzC2nH#IlYBtA";
    }
}
