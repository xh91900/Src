using System;
using System.Web;

namespace SuperProducer.Core.Utility
{
    public class WebHeaderHelper
    {
        /// <summary>
        /// 获取Header值
        /// </summary>
        public static string GetRequestHeader(string name)
        {
            return HttpContext.Current.Request.Headers[name];
        }

        /// <summary>
        /// 获取Header值
        /// </summary>
        public static string GetResponseHeader(string name)
        {
            return HttpContext.Current.Response.Headers[name];
        }
    }
}
