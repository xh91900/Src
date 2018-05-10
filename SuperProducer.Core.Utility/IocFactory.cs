using System;
using System.Collections.Generic;

namespace SuperProducer.Core.Utility
{
    /// <summary>
    /// 未实现[不可用]
    /// </summary>
    public class IocFactory
    {
        private Dictionary<Type, Type> InnerContainer { get; set; }

        public IocFactory()
        {
            this.InnerContainer = new Dictionary<Type, Type>();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="Interface">接口</typeparam>
        /// <typeparam name="Implement">实现</typeparam>
        public void Register<Interface, Implement>() where Implement : Interface
        {
            if (this.InnerContainer.ContainsKey(typeof(Interface)))
            {
                throw new Exception(string.Format("类型({0})已注册", typeof(Interface).FullName));
            }
        }
    }
}
