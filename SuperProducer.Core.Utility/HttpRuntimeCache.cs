using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace SuperProducer.Core.Utility
{
    /// <summary>
    /// [全局] ASP.NET Cache
    /// </summary>
    public class HttpRuntimeCache
    {
        /// <summary>
        /// 获取本地缓存
        /// </summary>
        public static object Get(string name)
        {
            return HttpRuntime.Cache.Get(name);
        }

        /// <summary>
        /// 移除本地缓存
        /// </summary>
        public static void Remove(string name)
        {
            if (HttpRuntime.Cache[name] != null)
                HttpRuntime.Cache.Remove(name);
        }

        /// <summary>
        /// 写入本地缓存(默认20分钟过期)
        /// </summary>
        public static void Set(string name, object value)
        {
            Set(name, value, null);
        }

        /// <summary>
        /// 写入本地缓存(默认20分钟过期)
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="value">value</param>
        /// <param name="cacheDependency">依赖项</param>
        public static void Set(string name, object value, CacheDependency cacheDependency)
        {
            HttpRuntime.Cache.Insert(name, value, cacheDependency, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
        }

        /// <summary>
        /// 写入本地缓存
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="value">value</param>
        /// <param name="minutes">有效期/分钟</param>
        public static void Set(string name, object value, int minutes)
        {
            HttpRuntime.Cache.Insert(name, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(minutes));
        }

        /// <summary>
        /// 本地缓存写入
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="value">value</param>
        /// <param name="minutes">有效期/分钟</param>
        /// <param name="isAbsoluteExpiration">是否绝对过期</param>
        /// <param name="onRemoveCallback">缓存过期回调</param>
        public static void Set(string name, object value, int minutes, bool isAbsoluteExpiration, CacheItemRemovedCallback onRemoveCallback)
        {
            if (isAbsoluteExpiration)
                HttpRuntime.Cache.Insert(name, value, null, DateTime.Now.AddMinutes(minutes), Cache.NoSlidingExpiration, CacheItemPriority.Normal, onRemoveCallback);
            else
                HttpRuntime.Cache.Insert(name, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(minutes), CacheItemPriority.Normal, onRemoveCallback);
        }

        /// <summary>
        /// 移除所有缓存
        /// </summary>
        public static void RemoveAll(string keyRegex = null)
        {
            var allRemoveKeys = new List<string>();

            var enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var key = enumerator.Key.ToString();
                if (string.IsNullOrWhiteSpace(keyRegex) || Regex.IsMatch(key, keyRegex, RegexOptions.IgnoreCase))
                {
                    allRemoveKeys.Add(key);
                }
            }

            foreach (var item in allRemoveKeys)
            {
                HttpRuntime.Cache.Remove(item);
            }
        }
    }
}
