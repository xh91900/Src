using System;
using System.Security.Cryptography;

namespace SuperProducer.Core.Utility.Encrypt
{
    public class SHA1 : EncryptBase
    {
        /// <summary>
        /// 加密
        /// </summary>
        public string Encrypt(string str, bool removeSPChar = true)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(str))
            {
                var sha1 = new SHA1CryptoServiceProvider();
                var buffer = this.DefaultEncode.GetBytes(str);
                buffer = sha1.ComputeHash(buffer);
                retVal = BitConverter.ToString(buffer);

                if (removeSPChar)
                {
                    retVal = retVal.Replace("-", "");
                }
            }
            return retVal;
        }
    }
}
