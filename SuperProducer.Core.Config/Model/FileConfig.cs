using System;
using System.Xml.Serialization;

namespace SuperProducer.Core.Config.Model
{
    #region "DaoConfig"

    [Serializable]
    public class DaoConfig : ConfigFileBase
    {
        public string Main { get; set; }

        public string Res { get; set; }

        public string Log { get; set; }
    }

    #endregion

    #region "CacheConfig"

    [Serializable]
    public class CacheConfig : ConfigFileBase
    {
        public CacheConfigItem[] CacheConfigItems { get; set; }

        public CacheProviderItem[] CacheProviderItems { get; set; }
    }

    public class CacheProviderItem
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "desc")]
        public string Desc { get; set; }
    }

    public class CacheConfigItem
    {
        [XmlAttribute(AttributeName = "keyRegex")]
        public string KeyRegex { get; set; }

        [XmlAttribute(AttributeName = "moduleRegex")]
        public string ModuleRegex { get; set; }

        [XmlAttribute(AttributeName = "providerName")]
        public string ProviderName { get; set; }

        [XmlAttribute(AttributeName = "minitus")]
        public int Minitus { get; set; }

        [XmlAttribute(AttributeName = "priority")]
        public int Priority { get; set; }

        [XmlAttribute(AttributeName = "isAbsoluteExpiration")]
        public bool IsAbsoluteExpiration { get; set; }

        [XmlAttribute(AttributeName = "desc")]
        public string Desc { get; set; }
    }

    #endregion

    #region "WcfServiceConfig"

    [Serializable]
    public class WcfServiceConfig : ConfigFileBase
    {
        public WcfServiceItem[] WcfServiceItems { get; set; }
    }

    public class WcfServiceItem
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "binding")]
        public string Binding { get; set; }

        [XmlAttribute(AttributeName = "uri")]
        public string Uri { get; set; }

        [XmlAttribute(AttributeName = "contractType")]
        public string ContractType { get; set; }

        [XmlAttribute(AttributeName = "desc")]
        public string Desc { get; set; }
    }

    #endregion
}
