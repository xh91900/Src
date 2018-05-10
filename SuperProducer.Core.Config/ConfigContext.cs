using SuperProducer.Core.Utility;

namespace SuperProducer.Core.Config
{
    public class ConfigContext
    {
        public IConfigService ConfigService { get; set; }

        public ConfigContext(IConfigService configService)
        {
            this.ConfigService = configService;
        }

        public virtual T Get<T>(string index = null) where T : ConfigFileBase, new()
        {
            var retVal = new T();
            var name = this.GetConfigName<T>(index);
            var content = this.ConfigService.GetConfig(name);
            if (string.IsNullOrEmpty(content))
            {
                this.ConfigService.SaveConfig(name, string.Empty);
            }
            else
            {
                try
                {
                    retVal = SerializationHelper.XmlDeserialize<T>(content);
                }
                catch { }
            }
            return retVal;
        }

        public virtual bool Save<T>(T configObject, string index = null) where T : ConfigFileBase, new()
        {
            if (configObject != null)
            {
                configObject.Save();
                var name = this.GetConfigName<T>(index);
                this.ConfigService.SaveConfig(name, SerializationHelper.XmlSerialize(configObject));
                return true;
            }
            return false;
        }

        public virtual string GetConfigName<T>(string index = null)
        {
            var configName = typeof(T).Name;
            if (!string.IsNullOrEmpty(index))
                configName = string.Format("{0}_{1}", configName, index);
            return configName;
        }
    }
}
