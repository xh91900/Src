using SuperProducer.Core.Utility;
using System.Web.Caching;
using System.Data.SqlClient;
using SuperProducer.Core.Config.Model;

namespace SuperProducer.Core.Config
{
    public class CachedDbConfigContext : ConfigContext
    {
        public static CachedDbConfigContext Current = new CachedDbConfigContext();

        public CachedDbConfigContext() : base(new DbConfigServices()) { }

        public override T Get<T>(string index = null)
        {
            var fileName = this.GetConfigName<T>(index);
            var key = string.Format("{0}_{1}", InternalConstant.CacheKeyPrefixes.ConfigDatabase.ToString(), fileName);
            var content = Caching.Get<T>(key);
            if (content != null)
                return content;

            SqlCommand command = null;
            var tmpConfigService = this.ConfigService as DbConfigServices;
            if (tmpConfigService != null)
                command = tmpConfigService.GetSqlCommand(null);

            var value = base.Get<T>(index);
            Caching.Set(key, value, new SqlCacheDependency(command));
            return value;
        }


        public SystemConfig SystemConfig
        {
            get
            {
                return this.Get<SystemConfig>();
            }
        }

        public EmailProviderConfig EmailProviderConfig
        {
            get
            {
                return this.Get<EmailProviderConfig>();
            }
        }

        public SmsProviderConfig SmsProviderConfig
        {
            get
            {
                return this.Get<SmsProviderConfig>();
            }
        }

        public WechatGZHConfig WechatGZHConfig
        {
            get
            {
                return this.Get<WechatGZHConfig>();
            }
        }

        public PaymentMethodConfig PaymentMethodConfig
        {
            get
            {
                return this.Get<PaymentMethodConfig>();
            }
        }

        public StorageProviderConfig StorageProviderConfig
        {
            get
            {
                return this.Get<StorageProviderConfig>();
            }
        }

        public BaiduAPIConfig BaiduAPIConfig
        {
            get
            {
                return this.Get<BaiduAPIConfig>();
            }
        }
    }
}
