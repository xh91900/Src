using SuperProducer.Core.Utility.Encrypt;
using System;

namespace ConsoleApp
{
    public class CacheExample
    {
        public static void PageLevel()
        {
            Console.WriteLine("【Cache-页面级缓存】");
            var wechatParas = SuperProducer.Core.Cache.CacheHelper.GetItem<WechatArguments>("PageCache_WechatArguments", new WechatArguments());
            ShowPageLevel();
        }

        public static void ShowPageLevel()
        {
            var wechatParas = SuperProducer.Core.Cache.CacheHelper.GetItem<WechatArguments>("PageCache_WechatArguments");
            if (wechatParas != null)
                Console.WriteLine(string.Format("key:{0},access_token:{1}", wechatParas.key, wechatParas.access_token));
        }

        public static void RuntimeLevel()
        {
            Console.WriteLine("【Cache-运行时级缓存】");
        }

        public static void RedisLevel()
        {
            Console.WriteLine("【Cache-Redis级缓存】");
        }

        public class WechatArguments
        {
            public string key { get; set; }

            public string access_token { get; set; }

            public WechatArguments()
            {
                this.key = new MD5().Encrypt("WechatArguments");
                this.access_token = new AES(this.key).Encrypt("杨帅");
            }
        }
    }
}
