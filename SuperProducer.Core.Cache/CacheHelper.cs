using System;
using System.Text.RegularExpressions;
using System.Web;

namespace SuperProducer.Core.Cache
{
    /// <summary>
    /// 缓存帮助
    /// </summary>
    public class CacheHelper
    {
        /// <summary>
        /// 获取缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static object Get(string key)
        {
            var cacheConfig = CacheConfigContext.GetCurrentWrapCacheConfigItem(key);
            return cacheConfig.CacheProvider.Get(key);
        }

        /// <summary>
        /// 设置缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        public static void Set(string key, object value)
        {
            var cacheConfig = CacheConfigContext.GetCurrentWrapCacheConfigItem(key);
            cacheConfig.CacheProvider.Set(key, value, cacheConfig.CacheConfigItem.Minitus, cacheConfig.CacheConfigItem.IsAbsoluteExpiration, null);
        }

        /// <summary>
        /// 移除缓存项
        /// </summary>
        /// <param name="key">缓存键</param>
        public static void Remove(string key)
        {
            var cacheConfig = CacheConfigContext.GetCurrentWrapCacheConfigItem(key);
            cacheConfig.CacheProvider.Remove(key);
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <param name="keyRegex"></param>
        /// <param name="moduleRegex"></param>
        public static void Clear(string keyRegex = ".*", string moduleRegex = ".*")
        {
            if (!Regex.IsMatch(CacheConfigContext.ModuleName, moduleRegex, RegexOptions.IgnoreCase))
                return;

            foreach (var cacheProviders in CacheConfigContext.CacheProviders.Values)
                cacheProviders.Clear(keyRegex);
        }



        #region "双层缓存[缓存层,页面级临时缓存]"

        public static T Get<T>(string key)
        {
            return Get<T>(key, () => { return default(T); });
        }

        public static T Get<T>(string key, Func<T> getRealData)
        {
            var getDataFromCache = new Func<T>(() =>
            {
                T data = default(T);
                var cacheData = Get(key);
                if (cacheData != null)
                {
                    data = (T)cacheData;
                }
                else
                {
                    data = getRealData();
                    if (data != null)
                        Set(key, data);
                }
                return data;
            });
            return GetItem<T>(key, getDataFromCache);
        }

        public static T Get<T>(string key, int id, Func<int, T> getRealData)
        {
            return Get<int, T>(key, id, getRealData);
        }

        public static T Get<T>(string key, string id, Func<string, T> getRealData)
        {
            return Get<string, T>(key, id, getRealData);
        }

        public static T Get<T>(string key, string branchKey, Func<T> getRealData)
        {
            return Get<string, T>(key, branchKey, id => getRealData());
        }

        public static T Get<P, T>(string key, P arg, Func<P, T> getRealData)
        {
            key = string.Format("{0}_{1}", key, arg);

            var getDataFromCache = new Func<T>(() =>
            {
                T data = default(T);
                var cacheData = Get(key);
                if (cacheData != null)
                {
                    data = (T)cacheData;
                }
                else
                {
                    data = getRealData(arg);
                    if (data != null)
                        Set(key, data);
                }
                return data;
            });
            return GetItem<T>(key, getDataFromCache);
        }

        #endregion

        #region "Web临时缓存-页面生命周期内有效"

        /// <summary>
        /// 临时缓存-页面级
        /// </summary>
        public static T GetItem<T>(string key)
        {
            return GetItem(key, default(T));
        }

        /// <summary>
        /// 临时缓存-页面级
        /// </summary>
        public static T GetItem<T>(string key, T defaultValue)
        {
            return GetItem(key, () => { return defaultValue; });
        }

        /// <summary>
        /// 临时缓存-页面级
        /// </summary>
        public static T GetItem<T>() where T : new()
        {
            return GetItem<T>(typeof(T).ToString(), () => new T());
        }

        /// <summary>
        /// 临时缓存-页面级
        /// </summary>
        public static T GetItem<T>(Func<T> getRealData)
        {
            return GetItem<T>(typeof(T).ToString(), getRealData);
        }

        /// <summary>
        /// 临时缓存-页面级
        /// </summary>
        public static T GetItem<T>(string key, Func<T> getRealData)
        {
            if (HttpContext.Current == null)
                return getRealData();

            var httpContextItems = HttpContext.Current.Items;
            if (httpContextItems.Contains(key))
            {
                return (T)httpContextItems[key];
            }
            else
            {
                var data = getRealData();
                if (data != null)
                    httpContextItems[key] = data;
                return data;
            }
        }

        /// <summary>
        /// 临时缓存-页面级
        /// </summary>
        public static bool RemoveItem<T>()
        {
            return RemoveItem(typeof(T).ToString());
        }

        /// <summary>
        /// 临时缓存-页面级
        /// </summary>
        public static bool RemoveItem(string key)
        {
            if (HttpContext.Current == null)
                return false;

            var httpContextItems = HttpContext.Current.Items;
            if (httpContextItems.Contains(key))
            {
                httpContextItems[key] = null;
                httpContextItems.Remove(key);
                return true;
            }
            return false;
        }

        #endregion
    }
}
