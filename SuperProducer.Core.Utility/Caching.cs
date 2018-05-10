using System;
using System.Web.Caching;

namespace SuperProducer.Core.Utility
{
    /// <summary>
    /// [全局] ASP.NET Cache
    /// </summary>
    public class Caching
    {
        /// <summary>
        /// 获取本地缓存
        /// </summary>
        public static T Get<T>(string key)
        {
            return Get(key, default(T));
        }

        /// <summary>
        /// 获取本地缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">默认值</param>
        public static T Get<T>(string key, T defaultValue)
        {
            try
            {
                return (T)HttpRuntimeCache.Get(key);
            }
            catch { }
            return defaultValue;
        }

        /// <summary>
        /// 设置本地缓存
        /// </summary>
        /// <param name="name">Key</param>
        /// <param name="value">Value</param>
        public static bool Set(string name, object value)
        {
            try
            {
                HttpRuntimeCache.Set(name, value, null);
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 设置本地缓存
        /// </summary>
        /// <param name="name">Key</param>
        /// <param name="value">Value</param>
        /// <param name="cacheDependency">依赖项</param>
        public static bool Set(string name, object value, CacheDependency cacheDependency)
        {
            try
            {
                HttpRuntimeCache.Set(name, value, cacheDependency);
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 设置本地缓存
        /// </summary>
        /// <param name="name">Key</param>
        /// <param name="value">Value</param>
        /// <param name="minutes">有效期/分钟</param>
        /// <param name="isAbsoluteExpiration">是否绝对过期</param>
        /// <param name="onRemoveCallback">缓存过期回调</param>
        public static bool Set(string name, object value, int minutes, bool isAbsoluteExpiration, CacheItemRemovedCallback onRemoveCallback)
        {
            try
            {
                HttpRuntimeCache.Set(name, value, minutes, isAbsoluteExpiration, onRemoveCallback);
                return true;
            }
            catch { }
            return false;
        }
    }
}
