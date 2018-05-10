using System;
using System.Web;

namespace SuperProducer.Core.Utility
{
    public class WebHelper
    {
        /// <summary>
        /// 获取Get参数
        /// </summary>
        public static string GetParas(string name)
        {
            var retVal = string.Empty;
            if (HttpContext.Current != null)
                retVal = HttpContext.Current.Request.QueryString[name];
            return string.IsNullOrEmpty(retVal) ? string.Empty : retVal;
        }

        /// <summary>
        /// 获取Post参数
        /// </summary>
        public static string PostParas(string name)
        {
            var retVal = string.Empty;
            if (HttpContext.Current != null)
                retVal = HttpContext.Current.Request.Form[name];
            return string.IsNullOrEmpty(retVal) ? string.Empty : retVal;
        }

        /// <summary>
        /// 获取Header参数
        /// </summary>
        public static string HeaderParas(string name)
        {
            var retVal = string.Empty;
            if (HttpContext.Current != null)
                retVal = HttpContext.Current.Request.Headers[name];
            return string.IsNullOrEmpty(retVal) ? string.Empty : retVal;
        }

        /// <summary>
        /// 获取顶级域名
        /// </summary>
        public static string GetTopDomain(string domain)
        {
            domain = domain.Trim().ToLower();

            var rootDomain = ".com.cn|.gov.cn|.cn|.com|.net|.org|.so|.co|.mobi|.tel|.biz|.info|.name|.me|.cc|.tv|.asiz|.hk"; // 注意排序
            if (domain.StartsWith("http://")) domain = domain.Replace("http://", "");
            if (domain.StartsWith("https://")) domain = domain.Replace("https://", "");
            if (domain.StartsWith("www.")) domain = domain.Replace("www.", "");
            if (domain.IndexOf("/") > 0)
                domain = domain.Substring(0, domain.IndexOf("/"));

            foreach (var item in StringHelper.SplitString(rootDomain, "|"))
            {
                if (domain.EndsWith(item))
                {
                    domain = domain.Replace(item, "");
                    if (domain.LastIndexOf(".") > 0)
                        domain = domain.Replace(domain.Substring(0, domain.LastIndexOf(".") + 1), "");
                    return domain + item;
                }
                continue;
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取当前请求的Url
        /// </summary>
        public static string CurrentUrl
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Request.Url.ToString();
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取当前请求的完整Url
        /// </summary>
        public static string CurrentExactUrl
        {
            get
            {
                if (HttpContext.Current != null)
                    return string.Format("{0}://{1}{2}",
                        HttpContext.Current.Request.Url.Scheme,
                        HttpContext.Current.Request.Url.Authority,
                        HttpContext.Current.Request.Url.AbsolutePath);
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取当前请求的主域[如pay.example.com主域是example.com]
        /// </summary>
        public static string ServerDomain
        {
            get
            {
                if (HttpContext.Current != null)
                    return GetTopDomain(HttpContext.Current.Request.Url.Host);
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取当前请求的用户的IP
        /// </summary>
        public static string UserIPAddress
        {
            get
            {
                var retVal = string.Empty;
                if (HttpContext.Current != null)
                {
                    if (string.IsNullOrEmpty(retVal))
                    {
                        retVal = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    }
                    if (string.IsNullOrEmpty(retVal))
                    {
                        retVal = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }

                    if (!RegExpHelper.IsIPAddressV4(retVal) && !RegExpHelper.IsIPAddressV6(retVal))
                    {
                        retVal = "Unknown";
                    }
                }
                return retVal;
            }
        }

        /// <summary>
        /// 获取当前请求的用户浏览器代理
        /// </summary>
        public static string UserAgent
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Request.UserAgent;
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取当前请求的引用页
        /// </summary>
        public static string UrlReferrer
        {
            get
            {
                if (HttpContext.Current != null)
                    return ConvertHelper.GetString(HttpContext.Current.Request.UrlReferrer);
                return string.Empty;
            }
        }
    }
}
