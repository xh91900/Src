using System;
using System.Security.Cryptography;

namespace SuperProducer.Core.Utility.Encrypt
{
    public class AES : EncryptBase
    {
        public enum OutputFormat
        {
            Base64 = 1,
            Hex = 2
        }

        private const int CONST_NUMBER_1 = 16;
        private const int CONST_NUMBER_2 = 24;
        private const int CONST_NUMBER_3 = 32;

        internal string Key { get; set; }

        internal string IV { get; set; }

        public OutputFormat Format { get; set; }


        /// <summary>
        /// 初始化AES加密
        /// </summary>
        /// <param name="key">key必须是16、24、32位字符串[不支持中文]</param>
        /// <param name="iv">默认取key的前16位</param>
        public AES(string key, string iv = null, OutputFormat format = OutputFormat.Base64)
        {
            this.Key = string.Empty;
            this.IV = string.Empty;
            this.Format = OutputFormat.Base64;

            if (!string.IsNullOrEmpty(key) && (key.Length == CONST_NUMBER_1 || key.Length == CONST_NUMBER_2 || key.Length == CONST_NUMBER_3))
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

            if (EnumHelper.IsDefined(typeof(OutputFormat), format))
            {
                this.Format = format;
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

            if (this.Key.Length == CONST_NUMBER_1 || this.Key.Length == CONST_NUMBER_2 || this.Key.Length == CONST_NUMBER_3)
            {
                keyIsOkay = !keyIsOkay;
            }

            if (this.IV.Length == CONST_NUMBER_1)
            {
                ivIsOkay = !ivIsOkay;
            }

            if (!keyIsOkay)
            {
                throw new ArgumentNullException("key", string.Format("key必须是{0}位、{1}位或{2}位的字符串", CONST_NUMBER_1, CONST_NUMBER_2, CONST_NUMBER_3));
            }

            if (!ivIsOkay)
            {
                throw new ArgumentNullException("iv", string.Format("iv必须是{0}位的字符串", CONST_NUMBER_1));
            }

            return keyIsOkay && ivIsOkay;
        }


        /// <summary>
        /// 加密
        /// </summary>
        public string Encrypt(string content)
        {
            var retVal = string.Empty;

            if (!string.IsNullOrEmpty(content) && this.CheckKeyAndIvIsOkay())
            {
                try
                {
                    var cntBuffer = this.DefaultEncode.GetBytes(content);
                    var keyBuffer = this.DefaultEncode.GetBytes(this.Key);
                    using (var aes = new RijndaelManaged())
                    {
                        aes.IV = this.DefaultEncode.GetBytes(this.IV);
                        aes.Key = keyBuffer;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;

                        var aesObject = aes.CreateEncryptor();
                        var retBuffer = aesObject.TransformFinalBlock(cntBuffer, 0, cntBuffer.Length);

                        switch (this.Format)
                        {
                            case OutputFormat.Hex:
                                retVal = ConvertHelper.ConvertByteArrayToHexString(retBuffer, false);
                                break;
                            case OutputFormat.Base64:
                                retVal = new Base64() { DefaultEncode = this.DefaultEncode }.Encrypt(retBuffer);
                                break;
                        }
                    }
                }
                catch { }
            }
            return retVal;
        }

        /// <summary>
        /// 解密
        /// </summary>
        public string Decrypt(string content)
        {
            var retVal = string.Empty;

            if (!string.IsNullOrEmpty(content) && this.CheckKeyAndIvIsOkay())
            {
                try
                {
                    byte[] cntBuffer = null;
                    switch (this.Format)
                    {
                        case OutputFormat.Hex:
                            cntBuffer = ConvertHelper.ConvertHexStringToByteArray(content);
                            break;
                        case OutputFormat.Base64:
                            cntBuffer = Convert.FromBase64String(content);
                            break;
                    }
                    var keyBuffer = this.DefaultEncode.GetBytes(this.Key);
                    using (var aes = new RijndaelManaged())
                    {
                        aes.IV = this.DefaultEncode.GetBytes(this.IV);
                        aes.Key = keyBuffer;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;

                        var aesObject = aes.CreateDecryptor();
                        var retBuffer = aesObject.TransformFinalBlock(cntBuffer, 0, cntBuffer.Length);
                        retVal = this.DefaultEncode.GetString(retBuffer);
                    }
                }
                catch { }
            }
            return retVal;
        }
    }
}
