using DevTrends.MvcDonutCaching;
using SuperProducer.Core.Cache;
using SuperProducer.Core.Config;
using SuperProducer.Core.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace SuperProducer.Core.Module
{
    public abstract class ContextCollectHandler : IHttpHandler, IRouteHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var res = context.Response;
            var req = context.Request;

            res.Clear();
            res.ContentEncoding = Encoding.Default;

            if (!string.Equals(WebHelper.ServerDomain, "localhost", StringComparison.OrdinalIgnoreCase))
            {
                res.Write("<h1>非法访问！</h1>");
            }
            else
            {
                ProcessRequest(req, res);
            }

            res.Flush();
            res.End();
            res.Close();
        }

        public abstract void ProcessRequest(HttpRequest req, HttpResponse res);

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }
    }

    public class ConfigCollect : ContextCollectHandler
    {
        public override void ProcessRequest(HttpRequest req, HttpResponse res)
        {
            var type = typeof(CachedFileConfigContext);
            var configContext = CachedFileConfigContext.Current;

            if (string.IsNullOrEmpty(req["config"]))
            {
                res.Write("<p><h1>网站当前配置列表：</h1><p>");

                foreach (var p in type.GetProperties())
                {
                    if (p.Name != "ConfigService")
                        res.Write(string.Format("<p><a href='?config={0}' target='_blank'>{0} [点击查看]</a></p>", p.Name));
                }
            }
            else
            {
                foreach (var p in type.GetProperties())
                {
                    if (p.Name == req["config"] && p.Name != "DaoConfig")
                    {
                        var currentConfig = p.GetValue(configContext, null);
                        if (currentConfig != null)
                        {
                            res.ContentType = "text/xml";
                            res.ContentEncoding = InternalConstant.DefaultEncode;
                            res.Write(SerializationHelper.XmlSerialize(currentConfig));
                            break;
                        }
                    }
                }
            }
        }
    }

    public class CacheCollect : ContextCollectHandler
    {
        public override void ProcessRequest(HttpRequest req, HttpResponse res)
        {
            if (req.QueryString.Count == 0)
            {
                res.Write("<p><h1>网站当前缓存列表：</h1><p>");

                var cacheItemList = new List<string>();
                var s = "<a href='?cacheclear=true' target='_blank'>！点击清除所有缓存</a>";
                cacheItemList.Add(s);

                var cacheEnumerator = HttpRuntime.Cache.GetEnumerator();
                while (cacheEnumerator.MoveNext())
                {
                    var key = cacheEnumerator.Key.ToString();
                    s = string.Format("<b>{0}</b>：{1}（<a href='?key={0}' target='_blank'>查看数据</a>）", key, cacheEnumerator.Value.GetType());
                    cacheItemList.Add(s);
                }

                cacheItemList.Sort();

                res.Write(string.Join("<hr>", cacheItemList));
            }
            else if (req["cacheclear"] != null)
            {
                CacheHelper.Clear();

                // 清空所有输出缓存[OutputCache]
                var cacheManager = new OutputCacheManager();
                cacheManager.RemoveItems();

                res.Write("清除缓存成功！");
            }
            else if (req["key"] != null)
            {
                var data = CacheHelper.Get(req["key"]);
                if (data != null)
                    res.Write(SerializationHelper.Newtonsoft_Serialize(data));
            }
        }
    }
}
