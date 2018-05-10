using System;
using System.ServiceModel;
using System.Xml;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

namespace SuperProducer.Core.Utility
{
    public class WcfServiceProxy
    {
        private const int maxReceivedMessageSize = 2147483647;

        private static TimeSpan timeout = TimeSpan.FromMilliseconds(InternalConstant.DefaultNetworkRequestTimeoutMS);

        public enum WcfServiceBinding
        {
            BasicHttpBinding,
            NetTcpBinding,
            WSHttpBinding
        }

        /// <summary>
        /// 动态创建Wcf客户端代理实例[支持HttpBinding]
        /// </summary>
        /// <typeparam name="T">Contract/接口</typeparam>
        /// <param name="uri">Wcf服务地址</param>
        /// <returns>代理实例</returns>
        public static T CreateServiceProxy<T>(string uri, WcfServiceBinding wsb)
        {
            var key = string.Format("{0} - {1}", typeof(T), uri);

            if (Caching.Get<T>(key) == null)
            {
                var binding = CreateBinding(wsb);
                if (binding != null)
                {
                    var chan = new ChannelFactory<T>(binding, new EndpointAddress(uri));
                    foreach (var item in chan.Endpoint.Contract.Operations)
                    {
                        var dataContractBehavior = item.Behaviors.Find<DataContractSerializerOperationBehavior>();
                        if (dataContractBehavior != null)
                            dataContractBehavior.MaxItemsInObjectGraph = int.MaxValue;
                    }
                    chan.Open();
                    var service = chan.CreateChannel();
                    Caching.Set(key, service);
                    return service;
                }
            }
            else
            {
                return Caching.Get<T>(key);
            }
            return default(T);
        }

        /// <summary>
        /// 创建通道绑定信息
        /// </summary>
        private static Binding CreateBinding(WcfServiceBinding wsb)
        {
            Binding binding = null;
            switch (wsb)
            {
                case WcfServiceBinding.BasicHttpBinding:
                    {
                        var tmpBinding = new BasicHttpBinding();
                        tmpBinding.MaxBufferSize = maxReceivedMessageSize;
                        tmpBinding.MaxBufferPoolSize = maxReceivedMessageSize;
                        tmpBinding.MaxReceivedMessageSize = maxReceivedMessageSize;

                        tmpBinding.MaxReceivedMessageSize = maxReceivedMessageSize;
                        tmpBinding.ReaderQuotas = new XmlDictionaryReaderQuotas();
                        tmpBinding.ReaderQuotas.MaxStringContentLength = maxReceivedMessageSize;
                        tmpBinding.ReaderQuotas.MaxArrayLength = maxReceivedMessageSize;
                        tmpBinding.ReaderQuotas.MaxBytesPerRead = maxReceivedMessageSize;

                        tmpBinding.OpenTimeout = timeout;
                        tmpBinding.ReceiveTimeout = timeout;
                        tmpBinding.SendTimeout = timeout;
                        tmpBinding.CloseTimeout = timeout;

                        binding = tmpBinding;
                    }
                    break;
                case WcfServiceBinding.NetTcpBinding:
                    {
                        var tmpBinding = new NetTcpBinding();
                        tmpBinding.MaxReceivedMessageSize = tmpBinding.MaxReceivedMessageSize * 1000;
                        tmpBinding.Security.Mode = SecurityMode.None;

                        binding = tmpBinding;
                    }
                    break;
                case WcfServiceBinding.WSHttpBinding:
                    {
                        var tmpBinding = new WSHttpBinding(SecurityMode.None);
                        tmpBinding.MaxReceivedMessageSize = tmpBinding.MaxReceivedMessageSize * 1000;
                        tmpBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
                        tmpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;

                        binding = tmpBinding;
                    }
                    break;
            }
            return binding;
        }
    }
}
