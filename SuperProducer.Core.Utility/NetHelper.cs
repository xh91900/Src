using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SuperProducer.Core.Utility
{
    public class NetHelper
    {
        #region "HTTP"

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        /// <summary>
        /// 发送Post请求,并将返回结果通过指定的方式序列化为对象
        /// </summary>
        public static T HttpPost<T>(string url, object data, SerializationType requestSerializeType = SerializationType.Json, SerializationType responseSerializeType = SerializationType.Json, Dictionary<string, string> header = null, CookieContainer cookies = null, X509Certificate cer = null, Encoding requestEncode = null, Encoding responseEncode = null)
        {
            T retVal = default(T);
            string responseString = HttpPost(url, data, requestSerializeType, header, cookies, cer, requestEncode, responseEncode);
            if (!string.IsNullOrEmpty(responseString))
            {
                if (responseSerializeType == SerializationType.Xml)
                {
                    retVal = (T)SerializationHelper.XmlDeserialize(typeof(T), responseString);
                }
                else if (responseSerializeType == SerializationType.Json)
                {
                    retVal = SerializationHelper.JavascriptDeserialize<T>(responseString);
                }
            }
            return retVal;
        }

        /// <summary>
        /// 发送Post请求,返回响应内容字符串
        /// </summary>
        public static string HttpPost(string url, object data, SerializationType requestSerializeType = SerializationType.Json, Dictionary<string, string> header = null, CookieContainer cookies = null, X509Certificate cer = null, Encoding requestEncode = null, Encoding responseEncode = null)
        {
            string retVal = string.Empty;
            try
            {
                requestEncode = requestEncode == null ? InternalConstant.DefaultEncode : requestEncode;
                responseEncode = responseEncode == null ? InternalConstant.DefaultEncode : responseEncode;

                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Timeout = InternalConstant.DefaultNetworkRequestTimeoutMS;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

                if (cer != null)
                    request.ClientCertificates = new X509CertificateCollection { cer };

                #region "请求头"

                if (header != null && header.Count > 0)
                {
                    foreach (var item in header)
                    {
                        try
                        {
                            switch (item.Key.ToLower())
                            {
                                case "content-type":
                                    request.ContentType = item.Value;
                                    break;
                                case "user-agent":
                                    request.UserAgent = item.Value;
                                    break;
                                case "referer":
                                    request.Referer = item.Value;
                                    break;
                                default:
                                    request.Headers.Add(item.Key, item.Value);
                                    break;
                            }
                        }
                        catch { }
                    }
                }

                if (cookies != null)
                {
                    request.CookieContainer = cookies;
                }

                #endregion

                string dataString = string.Empty;
                if (data.GetType() == typeof(string))
                {
                    dataString = data.ToString();
                }
                else
                {
                    if (requestSerializeType == SerializationType.Xml)
                    {
                        dataString = SerializationHelper.XmlSerialize(data);
                    }
                    else if (requestSerializeType == SerializationType.Json)
                    {
                        dataString = SerializationHelper.JavascriptSerialize(data);
                    }
                }

                if (!string.IsNullOrEmpty(dataString))
                {
                    var buffer = requestEncode.GetBytes(dataString);
                    using (Stream us = request.GetRequestStream())
                    {
                        us.Write(buffer, 0, buffer.Length);
                    }
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    #region "存储输出头"

                    if (response.Headers != null)
                    {
                        if (header == null)
                        {
                            header = new Dictionary<string, string>();
                        }

                        foreach (var item in response.Headers.AllKeys)
                        {
                            header.UpdateItem(item, response.Headers[item]);
                        }
                    }

                    if (response.Cookies != null)
                    {
                        if (cookies == null)
                        {
                            cookies = new CookieContainer();
                        }

                        foreach (Cookie item in response.Cookies)
                        {
                            cookies.Add(item);
                        }
                    }

                    #endregion

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), responseEncode))
                    {
                        retVal = reader.ReadToEnd();
                    }
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 发送Get请求,并将返回结果通过指定的方式序列化为对象
        /// </summary>
        public static T HttpGet<T>(string url, SerializationType responseSerializeType = SerializationType.Json, Dictionary<string, string> header = null, CookieContainer cookies = null, X509Certificate cer = null, Encoding responseEncode = null)
        {
            T retVal = default(T);
            string responseString = HttpGet(url, header, cookies, cer, responseEncode);
            if (!string.IsNullOrEmpty(responseString))
            {
                if (responseSerializeType == SerializationType.Xml)
                {
                    retVal = (T)SerializationHelper.XmlDeserialize(typeof(T), responseString);
                }
                else if (responseSerializeType == SerializationType.Json)
                {
                    retVal = (T)SerializationHelper.JsonDeserialize(typeof(T), responseString);
                }
            }
            return retVal;
        }

        /// <summary>
        /// 发送Get请求,返回响应内容字符串
        /// </summary>
        public static string HttpGet(string url, Dictionary<string, string> header = null, CookieContainer cookies = null, X509Certificate cer = null, Encoding responseEncode = null)
        {
            string retVal = string.Empty;
            try
            {
                responseEncode = responseEncode == null ? InternalConstant.DefaultEncode : responseEncode;

                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Timeout = InternalConstant.DefaultNetworkRequestTimeoutMS;
                request.Method = "GET";

                if (cer != null)
                    request.ClientCertificates = new X509CertificateCollection { cer };

                #region "请求头"

                if (header != null && header.Count > 0)
                {
                    foreach (var item in header)
                    {
                        try
                        {
                            switch (item.Key.ToLower())
                            {
                                case "content-type":
                                    request.ContentType = item.Value;
                                    break;
                                case "user-agent":
                                    request.UserAgent = item.Value;
                                    break;
                                case "referer":
                                    request.Referer = item.Value;
                                    break;
                                default:
                                    request.Headers.Add(item.Key, item.Value);
                                    break;
                            }
                        }
                        catch { }
                    }
                }

                if (cookies != null)
                {
                    request.CookieContainer = cookies;
                }

                #endregion

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    #region "存储输出头"

                    if (response.Headers != null)
                    {
                        if (header == null)
                        {
                            header = new Dictionary<string, string>();
                        }

                        foreach (var item in response.Headers.AllKeys)
                        {
                            header.UpdateItem(item, response.Headers[item]);
                        }
                    }

                    if (response.Cookies != null)
                    {
                        if (cookies == null)
                        {
                            cookies = new CookieContainer();
                        }

                        foreach (Cookie item in response.Cookies)
                        {
                            cookies.Add(item);
                        }
                    }

                    #endregion

                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), responseEncode))
                    {
                        retVal = reader.ReadToEnd();
                    }
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 发送Get请求,返回响应流
        /// </summary>
        public static Stream HttpGetStream(string url, Dictionary<string, string> header = null, CookieContainer cookies = null, X509Certificate cer = null)
        {
            Stream retVal = null;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);

                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Timeout = InternalConstant.DefaultNetworkRequestTimeoutMS;
                request.Method = "GET";

                if (cer != null)
                    request.ClientCertificates = new X509CertificateCollection { cer };

                #region "请求头"

                if (header != null && header.Count > 0)
                {
                    foreach (var item in header)
                    {
                        try
                        {
                            switch (item.Key.ToLower())
                            {
                                case "content-type":
                                    request.ContentType = item.Value;
                                    break;
                                case "user-agent":
                                    request.UserAgent = item.Value;
                                    break;
                                case "referer":
                                    request.Referer = item.Value;
                                    break;
                                default:
                                    request.Headers.Add(item.Key, item.Value);
                                    break;
                            }
                        }
                        catch { }
                    }
                }

                if (cookies != null)
                {
                    request.CookieContainer = cookies;
                }

                #endregion

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    #region "存储输出头"

                    if (response.Headers != null)
                    {
                        if (header == null)
                        {
                            header = new Dictionary<string, string>();
                        }

                        foreach (var item in response.Headers.AllKeys)
                        {
                            header.UpdateItem(item, response.Headers[item]);
                        }
                    }

                    if (response.Cookies != null)
                    {
                        if (cookies == null)
                        {
                            cookies = new CookieContainer();
                        }

                        foreach (Cookie item in response.Cookies)
                        {
                            cookies.Add(item);
                        }
                    }

                    #endregion

                    var tempOnlineStream = response.GetResponseStream();
                    if (tempOnlineStream != null && tempOnlineStream.CanRead)
                    {
                        using (tempOnlineStream)
                        {
                            retVal = new MemoryStream();
                            byte[] buffer = new byte[8192];
                            while (true)
                            {
                                int tempLength = tempOnlineStream.Read(buffer, 0, buffer.Length);
                                if (tempLength == 0)
                                    break;
                                retVal.Write(buffer, 0, tempLength);
                            }
                            retVal.Position = 0;
                        }
                    }
                }
            }
            catch { }
            return retVal;
        }

        #endregion
    }
}
