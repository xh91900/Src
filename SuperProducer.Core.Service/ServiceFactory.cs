using SuperProducer.Core.Utility;
using SuperProducer.Core.Cache;
using SuperProducer.Core.Config;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections;

namespace SuperProducer.Core.Service
{
    public abstract class ServiceFactory
    {
        public abstract T CreateService<T>() where T : class;
    }

    /// <summary>
    /// 直接引用提供服务
    /// </summary>
    public class RefServiceFactory : ServiceFactory
    {
        public override T CreateService<T>()
        {
            var interfaceName = typeof(T).Name;
            return CacheHelper.Get<T>(string.Format("{0}_{1}", InternalConstant.CacheKeyPrefixes.Service.ToString(), interfaceName), () =>
            {
                return AssemblyHelper.FindTypeByInterface<T>();
            });
        }
    }

    /// <summary>
    /// Ico提供服务
    /// </summary>
    public class IocServiceFactory : ServiceFactory
    {
        public override T CreateService<T>()
        {
            return Container.Current.Resolve<T>(findAllContainer: true);
        }
    }

    /// <summary>
    /// 通过Wcf提供服务
    /// </summary>
    public class WcfServiceFactory : ServiceFactory
    {
        public override T CreateService<T>()
        {
            if (CachedFileConfigContext.Current.WcfServiceConfig != null)
            {
                var wcfService = CachedFileConfigContext.Current.WcfServiceConfig;
                var wcf = wcfService.WcfServiceItems.Where(item => item.ContractType == typeof(T).FullName).FirstOrDefault();
                if (wcf != null)
                {
                    return WcfServiceProxy.CreateServiceProxy<T>(wcf.Uri, EnumHelper.Parse<WcfServiceProxy.WcfServiceBinding>(wcf.Binding));
                }
            }
            return default(T);
        }
    }
}
