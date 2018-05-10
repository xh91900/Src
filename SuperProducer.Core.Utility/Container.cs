using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperProducer.Core.Utility
{
    /// <summary>
    /// 仅提供安装方式
    /// </summary>
    public sealed class Container : IDisposable
    {
        private Container() { }

        public static Container Current = new Container();

        public IWindsorContainer DefaultContainer = new WindsorContainer();

        /// <summary>
        /// 容器集合
        /// </summary>
        private Dictionary<string, IWindsorContainer> allContainer = new Dictionary<string, IWindsorContainer>();

        public void Dispose() { }

        /// <summary>
        /// 增加安装程序[组件]
        /// </summary>
        public void AddInstaller(string containerName, IWindsorInstaller ins)
        {
            var tmpContainer = DefaultContainer;
            if (!string.IsNullOrEmpty(containerName))
            {
                if (!allContainer.ContainsKey(containerName))
                    allContainer.Add(containerName, new WindsorContainer());
                tmpContainer = allContainer[containerName];
            }
            tmpContainer.Install(ins);
        }

        /// <summary>
        /// 移除安装程序[组件]
        /// </summary>
        public bool RemoveInstaller(string containerName = null, IWindsorContainer container = null)
        {
            IWindsorContainer targetContainer = null;
            if (!string.IsNullOrEmpty(containerName) && allContainer.ContainsKey(containerName))
                targetContainer = allContainer[containerName];
            if (container != null && allContainer.ContainsValue(container))
                targetContainer = allContainer.Where(item => item.Value == container).FirstOrDefault().Value;

            if (targetContainer != null)
            {
                targetContainer.Dispose();
                allContainer.Remove(containerName);
                targetContainer = null;
                return true;
            }
            return false;
        }


        public T Resolve<T>(string containerName = null, bool findAllContainer = false, string key = null) where T : class
        {
            var retVal = default(T);
            if (findAllContainer)
            {
                var targetValue = Resolve<T>(DefaultContainer, key);
                if (targetValue == null)
                {
                    foreach (var item in allContainer)
                    {
                        targetValue = Resolve<T>(item.Value, key);
                        if (targetValue != null)
                            break;
                    }
                }
                retVal = targetValue;
            }
            else if (string.IsNullOrEmpty(containerName))
            {
                retVal = Resolve<T>(DefaultContainer, key);
            }
            else if (allContainer.ContainsKey(containerName))
            {
                retVal = Resolve<T>(allContainer[containerName], key);
            }
            return retVal;
        }

        public T Resolve<T>(IWindsorContainer container, string key = null) where T : class
        {
            if (container != null)
            {
                try
                {
                    return string.IsNullOrEmpty(key) ? container.Resolve<T>() : container.Resolve<T>(key);
                }
                catch { }
            }
            return null;
        }

        public object Resolve(Type type, string containerName = null, bool findAllContainer = false, string key = null)
        {
            object retVal = null;
            if (findAllContainer)
            {
                var targetValue = Resolve(type, DefaultContainer, key);
                if (targetValue == null)
                {
                    foreach (var item in allContainer)
                    {
                        targetValue = Resolve(type, item.Value, key);
                        if (targetValue != null)
                            break;
                    }
                }
                retVal = targetValue;
            }
            else if (string.IsNullOrEmpty(containerName))
            {
                retVal = Resolve(type, DefaultContainer, key);
            }
            else if (allContainer.ContainsKey(containerName))
            {
                retVal = Resolve(type, allContainer[containerName], key);
            }
            return retVal;
        }

        public object Resolve(Type type, IWindsorContainer container, string key = null)
        {
            if (container != null)
            {
                try
                {
                    return string.IsNullOrEmpty(key) ? container.Resolve(type) : container.Resolve(key, type);
                }
                catch { }
            }
            return null;
        }

        public IEnumerable<object> ResolveAll(Type type, string containerName = null, bool findAllContainer = false)
        {
            IEnumerable<object> retVal = null;
            if (findAllContainer)
            {
                var targetValue = ResolveAll(type, DefaultContainer);
                if (targetValue == null)
                {
                    foreach (var item in allContainer)
                    {
                        targetValue = ResolveAll(type, item.Value);
                        if (targetValue != null)
                            break;
                    }
                }
                retVal = targetValue;
            }
            else if (string.IsNullOrEmpty(containerName))
            {
                retVal = ResolveAll(type, DefaultContainer);
            }
            else if (allContainer.ContainsKey(containerName))
            {
                retVal = ResolveAll(type, allContainer[containerName]);
            }
            return retVal;
        }

        public IEnumerable<object> ResolveAll(Type type, IWindsorContainer container)
        {
            if (container != null)
            {
                try
                {
                    var array = container.ResolveAll(type);
                    if (array != null && array.Length > 0)
                    {
                        var list = new List<object>();
                        foreach (object item in array)
                            list.Add(item);
                        return list;
                    }
                }
                catch { }
            }
            return null;
        }


        public abstract class BaseWindsorInstaller : IWindsorInstaller
        {
            protected virtual string AssemblyName { get { return null; } }

            protected virtual Func<FromAssemblyDescriptor, BasedOnDescriptor> BasicDescriptor { get { return null; } }

            protected virtual Func<ServiceDescriptor, BasedOnDescriptor> WithServiceDescriptor { get { return null; } }

            protected virtual Action<ComponentRegistration> Configurer { get { return null; } }

            public virtual void Install(IWindsorContainer container, IConfigurationStore store)
            {
                if (!string.IsNullOrEmpty(AssemblyName))
                {
                    var asmDesc = Classes.FromAssemblyNamed(this.AssemblyName);
                    var basedDesc = BasicDescriptor == null ? asmDesc.Pick() : BasicDescriptor(asmDesc);
                    if (WithServiceDescriptor != null)
                        basedDesc = WithServiceDescriptor(basedDesc.WithService);
                    if (Configurer != null)
                        basedDesc = basedDesc.Configure(this.Configurer);
                    container.Register(basedDesc);
                }
            }
        }
    }
}
