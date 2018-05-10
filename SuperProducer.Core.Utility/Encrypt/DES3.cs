using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SuperProducer.Core.Utility.Encrypt
{
    public class DES3 : EncryptBase
    {
        private const int CONST_NUMBER_1 = 8;
        private const int CONST_NUMBER_2 = 24;

        internal string Key { get; set; }

        internal string IV { get; set; }

        /// <summary>
        /// 初始化3DES加密
        /// </summary>
        /// <param name="key">key必须是24位字符串[不支持中文]</param>
        /// <param name="iv">默认取key的前8位</param>
        public DES3(string key, string iv = null)
        {
            this.Key = string.Empty;
            this.IV = string.Empty;

            if (!string.IsNullOrEmpty(key) && key.Length == CONST_NUMBER_2)
            {
                this.Key = key;
            }

            if (!string.IsNullOrEmpty(iv) && iv.Length == CONST_NUMBER_1)
            {
                this.IV = iv;
            }
            else if (!string.IsNullOrEmpty(this.Key) && string.IsNullOrEmpty(this.IV))
            {
                this.InitIvByKey();
            }

            this.CheckKeyAndIvIsOkay();
        }

        private void InitIvByKey()
        {
            this.IV = this.Key.Substring(0, CONST_NUMBER_1);
        }

        public bool CheckKeyAndIvIsOkay()
        {
            bool keyIsOkay = false;
            bool ivIsOkay = false;

            if (this.Key.Length == CONST_NUMBER_2)
            {
                keyIsOkay = !keyIsOkay;
            }

            if (this.IV.Length == CONST_NUMBER_1)
            {
                ivIsOkay = !ivIsOkay;
            }

            if (!keyIsOkay)
            {
                throw new ArgumentNullException("key", string.Format("key必须是{0}位的字符串", CONST_NUMBER_2));
            }

            if (!ivIsOkay)
            {
                throw new ArgumentNullException("iv", string.Format("iv必须是{0}位的字符串", CONST_NUMBER_1));
            }

            return keyIsOkay && ivIsOkay;
        }


        /// <summary>
        ///加密
        /// </summary>
        public string Encrypt(string content)
        {
            var retVal = new StringBuilder();

            if (!string.IsNullOrEmpty(content) && this.CheckKeyAndIvIsOkay())
            {
                try
                {
                    using (SymmetricAlgorithm des = new TripleDESCryptoServiceProvider())
                    {
                        //des.Mode = CipherMode.CBC;
                        //des.Padding = PaddingMode.PKCS7;
                        des.Key = this.DefaultEncode.GetBytes(this.Key);
                        des.IV = this.DefaultEncode.GetBytes(this.IV);

                        var cntBuffer = this.DefaultEncode.GetBytes(content);

                        using (var desObject = des.CreateEncryptor())
                        {
                            using (var ms = new MemoryStream())
                            {
                                using (var cs = new CryptoStream(ms, desObject, CryptoStreamMode.Write))
                                {
                                    cs.Write(cntBuffer, 0, cntBuffer.Length);
                                    cs.FlushFinalBlock();
                                }

                                foreach (var item in ms.ToArray())
                                {
                                    retVal.AppendFormat("{0:X2}", item);
                                }
                            }
                        }
                    }
                }
                catch { }
            }
            return retVal.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        public string Decrypt(string content)
        {
            var retVal = string.Empty;

            if (!string.IsNullOrEmpty(content) && content.Length % 2 == 0 && this.CheckKeyAndIvIsOkay())
            {
                try
                {
                    using (SymmetricAlgorithm des = new TripleDESCryptoServiceProvider())
                    {
                        //des.Mode = CipherMode.CBC;
                        //des.Padding = PaddingMode.PKCS7;
                        des.Key = this.DefaultEncode.GetBytes(this.Key);
                        des.IV = this.DefaultEncode.GetBytes(this.IV);

                        var cntBuffer = ConvertHelper.ConvertHexStringToByteArray(content);
                        using (var desObject = des.CreateDecryptor())
                        {
                            using (var ms = new MemoryStream())
                            {
                                using (var cs = new CryptoStream(ms, desObject, CryptoStreamMode.Write))
                                {
                                    cs.Write(cntBuffer, 0, cntBuffer.Length);
                                    cs.FlushFinalBlock();
                                }
                                retVal = this.DefaultEncode.GetString(ms.ToArray());
                            }
                        }
                    }
                }
                catch { }
            }
            return retVal;
        }
    }
}
