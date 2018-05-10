using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SuperProducer.Core.Config.Model
{
    [Serializable]
    [Description("系统级配置")]
    public class SystemConfig : ConfigFileBase
    {
        /// <summary>
        /// 短信验证码过期时间[分]
        /// </summary>
        public int SmsCaptchaExpiresMinute { get; set; }

        /// <summary>
        /// 短信验证码长度
        /// </summary>
        public int SmsCaptchaLength { get; set; }

        /// <summary>
        /// 短信验证码发送间隔时间[秒]
        /// </summary>
        public int SmsCaptchaSendIntervalSecond { get; set; }

        /// <summary>
        /// 邮件验证码过期时间[分]
        /// </summary>
        public int EmailCaptchaExpiresMinute { get; set; }

        /// <summary>
        /// 邮件验证码长度
        /// </summary>
        public int EmailCaptchaLength { get; set; }

        /// <summary>
        /// 邮件验证码发送间隔时间[秒]
        /// </summary>
        public int EmailCaptchaSendIntervalSecond { get; set; }

        /// <summary>
        ///  AES加密Key[用于普通性加密]
        /// </summary>
        public string AESKey_Universal { get; set; }

        /// <summary>
        ///  AES加密Key[用于用户密码]
        /// </summary>
        public string AESKey_UserPassword { get; set; }
    }

    [Serializable]
    [Description("邮件供应商配置")]
    public class EmailProviderConfig : ConfigFileBase
    {
        public EmailProviderInfo Provider { get; set; }

        public class EmailProviderInfo
        {
            public string Server { get; set; }
            public ushort Port { get; set; }
            public bool EnabledSSL { get; set; }
            public string Account { get; set; }
            public string Password { get; set; }
            public string DisplayName { get; set; }
        }
    }

    [Serializable]
    [Description("短信供应商配置")]
    public class SmsProviderConfig : ConfigFileBase
    {
        public SmsProviderInfo Provider { get; set; }

        public class SmsProviderInfo
        {
            public string BasicUrl { get; set; }
            public string Account { get; set; }
            public string Password { get; set; }
            public string Sign { get; set; }
        }
    }

    [Serializable]
    [Description("微信公众号配置")]
    public class WechatGZHConfig : ConfigFileBase
    {
        public WechatGZHInfo GZH { get; set; }

        public class WechatGZHInfo
        {
            public string AppID { get; set; }

            public string AppKey { get; set; }

            public string AccessToken { get; set; }

            public string JSTikect { get; set; }
        }
    }

    [Serializable]
    [Description("支付方式配置")]
    public class PaymentMethodConfig : ConfigFileBase
    {
        public List<PaymentMethodInfo> Providers { get; set; }

        public class PaymentMethodInfo
        {
            /// <summary>
            /// 识别码
            /// </summary>
            public string TypeCode { get; set; }

            /// <summary>
            /// json字符串配置
            /// </summary>
            public string JsonConfig { get; set; }
        }
    }

    [Serializable]
    [Description("存储提供者配置")]
    public class StorageProviderConfig : ConfigFileBase
    {
        public List<StorageProviderInfo> Providers { get; set; }

        public class StorageProviderInfo
        {
            /// <summary>
            /// 识别码
            /// </summary>
            public string TypeCode { get; set; }

            /// <summary>
            /// json字符串配置
            /// </summary>
            public string JsonConfig { get; set; }
        }
    }

    [Serializable]
    [Description("百度API配置")]
    public class BaiduAPIConfig : ConfigFileBase
    {
        public BaiduAPIInfo API { get; set; }

        public class BaiduAPIInfo
        {
            public string AppID { get; set; }

            public string AppKey { get; set; }

            public string AccessToken { get; set; }
        }
    }
}
