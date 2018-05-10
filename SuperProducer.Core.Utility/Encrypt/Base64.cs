using System;

namespace SuperProducer.Core.Utility.Encrypt
{
    public class Base64 : EncryptBase
    {
        /// <summary>
        /// 加密
        /// </summary>
        public string Encrypt(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                return this.Encrypt(this.DefaultEncode.GetBytes(content));
            }
            return string.Empty;
        }

        /// <summary>
        /// 加密
        /// </summary>
        public string Encrypt(byte[] buffer)
        {
            try
            {
                if (buffer != null && buffer.Length > 0)
                {
                    return Convert.ToBase64String(buffer);
                }
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// 解密
        /// </summary>
        public string Decrypt(string content)
        {
            var retVal = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(content))
                {
                    var buff = Convert.FromBase64String(content);
                    retVal = this.DefaultEncode.GetString(buff);
                }
            }
            catch { }
            return retVal;
        }
    }
}
