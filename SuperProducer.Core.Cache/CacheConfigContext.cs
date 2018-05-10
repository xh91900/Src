using SuperProducer.Core.Config;
using SuperProducer.Core.Config.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SuperProducer.Core.Cache
{
    internal class CacheConfigContext
    {
        private static readonly object lockObject = new object();

        internal static CacheConfig CacheConfig
        {
            get
            {
                return CachedFileConfigContext.Current.CacheConfig;
            }
        }

        /// <summary>
        /// 首次加载所有的CacheConfig
        /// </summary>
        private static List<WrapCacheConfigItem> wrapCacheConfigItems;

        internal static List<WrapCacheConfigItem> WrapCacheConfigItems
        {
            get
            {
                if (wrapCacheConfigItems == null)
                {
                    lock (lockObject)
                    {
                        if (wrapCacheConfigItems == null)
                        {
                            wrapCacheConfigItems = new List<WrapCacheConfigItem>();

                            foreach (var item in CacheConfig.CacheConfigItems)
                            {
                                var cacheWrapConfigItem = new WrapCacheConfigItem();
                                cacheWrapConfigItem.CacheConfigItem = item;
                                cacheWrapConfigItem.CacheProviderItem = CacheConfig.CacheProviderItems.SingleOrDefault(value => value.Name == item.ProviderName);
                                cacheWrapConfigItem.CacheProvider = CacheProviders[item.ProviderName];
                                wrapCacheConfigItems.Add(cacheWrapConfigItem);
                            }
                        }
                    }
                }

                return wrapCacheConfigItems;
            }
        }


        /// <summary>
        /// 首次加载所有的CacheProviders
        /// </summary>
        private static Dictionary<string, ICacheProvider> cacheProviders;

        internal static Dictionary<string, ICacheProvider> CacheProviders
        {
            get
            {
                if (cacheProviders == null)
                {
                    lock (lockObject)
                    {
                        if (cacheProviders == null)
                        {
                            cacheProviders = new Dictionary<string, ICacheProvider>();

                            foreach (var item in CacheConfig.CacheProviderItems)
                            {
                                cacheProviders.Add(item.Name, (ICacheProvider)Activator.CreateInstance(Type.GetType(item.Type)));
                            }
                        }
                    }
                }

                return cacheProviders;
            }
        }


        /// <summary>
        /// 根据Key，通过正则匹配从WrapCacheConfigItems里筛选出符合的缓存项目，然后通过字典缓存起来
        /// </summary>
        private static Dictionary<string, WrapCacheConfigItem> wrapCacheConfigItemDic;

        internal static WrapCacheConfigItem GetCurrentWrapCacheConfigItem(string key)
        {
            if (wrapCacheConfigItemDic == null)
                wrapCacheConfigItemDic = new Dictionary<string, WrapCacheConfigItem>();

            if (wrapCacheConfigItemDic.ContainsKey(key))
                return wrapCacheConfigItemDic[key];

            var currentWrapCacheConfigItem = WrapCacheConfigItems.Where(item =>
                Regex.IsMatch(ModuleName, item.CacheConfigItem.ModuleRegex, RegexOptions.IgnoreCase) &&
                Regex.IsMatch(key, item.CacheConfigItem.KeyRegex, RegexOptions.IgnoreCase))
                .OrderByDescending(item => item.CacheConfigItem.Priority).FirstOrDefault();

            if (currentWrapCacheConfigItem == null)
                throw new Exception(string.Format("Get Cache '{0}' Config Exception", key));

            lock (lockObject) // 缓存多了会导致字典变大
            {
                if (!wrapCacheConfigItemDic.ContainsKey(key))
                    wrapCacheConfigItemDic.Add(key, currentWrapCacheConfigItem);
            }

            return currentWrapCacheConfigItem;
        }

        /// <summary>
        /// 得到网站项目的入口程序模块名名字，用于CacheConfigItem.ModuleRegex
        /// </summary>
        private static string moduleName;

        public static string ModuleName
        {
            get
            {
                if (moduleName == null)
                {
                    lock (lockObject)
                    {
                        if (moduleName == null)
                        {
                            var entryAssembly = Assembly.GetEntryAssembly();

                            if (entryAssembly != null)
                            {
                                moduleName = entryAssembly.FullName;
                            }
                            else
                            {
                                moduleName = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Name;
                            }
                        }
                    }
                }
                return moduleName;
            }
        }
    }

    public class WrapCacheConfigItem
    {
        public CacheConfigItem CacheConfigItem { get; set; }
        public CacheProviderItem CacheProviderItem { get; set; }
        public ICacheProvider CacheProvider { get; set; }
    }
}
