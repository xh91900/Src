using System;
using System.Web;

namespace SuperProducer.Core.Utility
{
    public class WebCookieHelper
    {
        /// <summary>
        /// 获取Cookie值
        /// </summary>
        public static HttpCookie Get(string name)
        {
            return HttpContext.Current.Request.Cookies[name];
        }

        /// <summary>
        /// 获取Cookie值
        /// </summary>
        public static string GetValue(string name)
        {
            var cookie = Get(name);
            if (cookie != null)
                return cookie.Value;
            return string.Empty;
        }

        /// <summary>
        /// 移除Cookie
        /// </summary>
        public static void Remove(string name)
        {
            Remove(Get(name));
        }

        /// <summary>
        /// 移除Cookie
        /// </summary>
        public static void Remove(HttpCookie cookie)
        {
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddMilliseconds(0 - InternalConstant.DefaultNetworkRequestTimeoutMS);
                Save(cookie);
            }
        }

        /// <summary>
        /// 保存Cookie
        /// </summary>
        public static void Save(string name, string value, int expiresMinutes = 0)
        {
            var httpCookie = Get(name);
            if (httpCookie == null)
                httpCookie = Set(name);

            httpCookie.Value = value;
            Save(httpCookie, expiresMinutes);
        }

        /// <summary>
        /// 保存Cookie
        /// </summary>
        public static void Save(HttpCookie cookie, int expiresMinutes = 0)
        {
            string domain = WebHelper.ServerDomain;
            string host = HttpContext.Current.Request.Url.Host.ToLower();
            if (domain != host)
                cookie.Domain = domain;

            //if (expiresMinutes > 0)
            cookie.Expires = DateTime.Now.AddMinutes(expiresMinutes);

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 创建Cookie对象
        /// </summary>
        public static HttpCookie Set(string name)
        {
            return new HttpCookie(name);
        }
    }
}
