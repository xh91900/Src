using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using SuperProducer.Core.Log;
using SuperProducer.Core.Utility;

namespace SuperProducer.Core.Service
{
    public partial class ServiceHelper
    {
        public static ServiceFactory ServiceFactory = new IocServiceFactory();

        /// <summary>
        /// 创建服务实例
        /// </summary>
        public static T CreateService<T>() where T : class
        {
            T retVal = default(T);
            var service = ServiceFactory.CreateService<T>();
            if (service != null)
            {
                var generator = new ProxyGenerator();
                retVal = generator.CreateInterfaceProxyWithTargetInterface<T>(service, new InvokeInterceptor());
            }
            return retVal;
        }
    }

    public class InvokeInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                if (ex is ILogService)
                {
                    var exception = ex as ILogService;

                    #region "IExceptionInfo"

                    if (exception.MessageObject is IExceptionInfo)
                    {
                        var message = exception.MessageObject as IExceptionInfo;
                        if (message != null)
                        {
                            if (string.IsNullOrEmpty(message.Class))
                                message.Class = ConvertHelper.GetString(invocation.TargetType);
                            if (string.IsNullOrEmpty(message.Method))
                                message.Method = ConvertHelper.GetString(invocation.Method);
                            if (string.IsNullOrEmpty(message.Arguments))
                                message.Arguments = SerializationHelper.Newtonsoft_Serialize(invocation.Arguments);
                            if (string.IsNullOrEmpty(message.Content))
                                message.Content = ex.Message;
                        }
                    }

                    #endregion

                    switch (exception.Rank)
                    {
                        case LoggerRank.Info:
                            Log4NetHelper.Info(LoggerType.ServiceExceptionLog, exception.MessageObject, ex);
                            break;
                        case LoggerRank.Debug:
                            Log4NetHelper.Debug(LoggerType.ServiceExceptionLog, exception.MessageObject, ex);
                            break;
                        case LoggerRank.Warn:
                            Log4NetHelper.Warn(LoggerType.ServiceExceptionLog, exception.MessageObject, ex);
                            break;
                        case LoggerRank.Error:
                            Log4NetHelper.Error(LoggerType.ServiceExceptionLog, exception.MessageObject, ex);
                            break;
                        case LoggerRank.Fatal:
                            Log4NetHelper.Fatal(LoggerType.ServiceExceptionLog, exception.MessageObject, ex);
                            break;
                    }
                }
                else
                {
                    var message = new InternalExceptionInfo()
                    {
                        Class = ConvertHelper.GetString(invocation.TargetType),
                        Method = ConvertHelper.GetString(invocation.Method),
                        Arguments = SerializationHelper.Newtonsoft_Serialize(invocation.Arguments),
                        Content = ex.Message
                    };
                    if (ex.InnerException != null)
                    {
                        var tmpException = ex.InnerException;
                        while (tmpException != null)
                        {
                            message.Content = tmpException.Message;
                            tmpException = tmpException.InnerException;
                        }
                    }
                    Log4NetHelper.Error(LoggerType.WebExceptionLog, message, ex);
                }

                //throw;
            }
        }
    }
}
