using SuperProducer.Core.Config.Model;
using SuperProducer.Core.Utility;
using System;
using System.Web.Caching;

namespace SuperProducer.Core.Config
{
    public class CachedFileConfigContext : ConfigContext
    {
        public static CachedFileConfigContext Current = new CachedFileConfigContext();

        public CachedFileConfigContext() : base(new FileConfigService()) { }

        public override T Get<T>(string index = null)
        {
            var fileName = this.GetConfigName<T>(index);
            var key = string.Format("{0}_{1}", InternalConstant.CacheKeyPrefixes.ConfigFile.ToString(), fileName);
            var content = Caching.Get<T>(key);
            if (content != null)
                return content;

            var filePath = string.Empty;
            var tmpConfigService = this.ConfigService as FileConfigService;
            if (tmpConfigService != null)
                filePath = tmpConfigService.GetFilePath(fileName);

            var value = base.Get<T>(index);
            Caching.Set(key, value, new CacheDependency(filePath));
            return value;
        }

        public CacheConfig CacheConfig
        {
            get
            {
                return this.Get<CacheConfig>();
            }
        }

        public WcfServiceConfig WcfServiceConfig
        {
            get
            {
                return this.Get<WcfServiceConfig>();
            }
        }

        public DaoConfig DaoConfig
        {
            get
            {
                return base.Get<DaoConfig>();
            }
        }

        public SystemConfig SystemConfig
        {
            get
            {
                return base.Get<SystemConfig>();
            }
        }
    }
}
