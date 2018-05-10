using System;
using System.Security.Cryptography;

namespace SuperProducer.Core.Utility.Encrypt
{
    public class MD5 : EncryptBase
    {
        /// <summary>
        /// 加密
        /// </summary>
        public string Encrypt(string content, bool upperCase = true)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(content))
            {
                var buffer = this.DefaultEncode.GetBytes(content);
                buffer = new MD5CryptoServiceProvider().ComputeHash(buffer);
                for (int i = 0; i < buffer.Length; i++)
                {
                    retVal += buffer[i].ToString(upperCase ? "X" : "x").PadLeft(2, '0');
                }
            }
            return retVal;
        }
    }
}
