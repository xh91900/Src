using SuperProducer.Core.Utility;
using System;

namespace SuperProducer.Core.Cache
{
    internal class LocalCacheProvider : ICacheProvider
    {
        public virtual object Get(string key)
        {
            return HttpRuntimeCache.Get(key);
        }

        public virtual void Set(string key, object value, int minutes, bool isAbsoluteExpiration, Action<string, object, string> onRemove)
        {
            HttpRuntimeCache.Set(key, value, minutes, isAbsoluteExpiration, (k, v, reason) =>
            {
                if (onRemove != null)
                    onRemove(k, v, reason.ToString());
            });
        }

        public virtual void Remove(string key)
        {
            HttpRuntimeCache.Remove(key);
        }

        public virtual void Clear(string keyRegex)
        {
            HttpRuntimeCache.RemoveAll(keyRegex);
        }
    }
}
