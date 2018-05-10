using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using SuperProducer.Core.Utility.Encrypt;

namespace SuperProducer.Core.Utility
{
    public static class ConvertHelper
    {
        #region "基础类型转换"

        public static bool GetBool(object value)
        {
            return GetBool(value, false);
        }

        public static uint GetUInt(object value)
        {
            return GetUInt(value, uint.MinValue);
        }

        public static int GetInt(object value)
        {
            return GetInt(value, int.MinValue);
        }

        public static string GetString(object value)
        {
            return GetString(value, string.Empty);
        }

        public static decimal GetDecimal(object value)
        {
            return GetDecimal(value, decimal.MinValue);
        }

        public static Double GetDouble(object value)
        {
            return GetDouble(value, double.MinValue);
        }

        public static float GetFloat(object value)
        {
            return GetFloat(value, float.MinValue);
        }

        public static long GetLong(object value)
        {
            return GetLong(value, long.MinValue);
        }

        public static DateTime GetDateTime(object value)
        {
            return GetDateTime(value, DateTime.MinValue);
        }

        public static byte GetByte(object value)
        {
            return GetByte(value, byte.MinValue);
        }

        public static byte[] GetByteArray(object value)
        {
            return GetByteArray(value, null);
        }

        public static Encoding GetEncoding(string encodeName)
        {
            return GetEncoding(encodeName, null);
        }

        public static Encoding GetEncoding(int codePage)
        {
            return GetEncoding(codePage, null);
        }



        public static bool GetBool(object value, bool defaultValue)
        {
            bool retVal = defaultValue;
            if (value != null)
            {
                try
                {
                    retVal = Convert.ToBoolean(value);
                }
                catch { }
            }
            return retVal;
        }

        public static int GetInt(object value, int defaultValue)
        {
            int retVal = defaultValue;
            if (value != null)
            {
                try
                {
                    retVal = Convert.ToInt32(value);
                }
                catch { }
            }
            return retVal;
        }

        public static uint GetUInt(object value, uint defaultValue)
        {
            uint retVal = defaultValue;
            if (value != null)
            {
                try
                {
                    retVal = Convert.ToUInt32(value);
                }
                catch { }
            }
            return retVal;
        }

        public static string GetString(object value, string defaultValue)
        {
            string retVal = defaultValue;
            if (value != null)
            {
                try
                {
                    retVal = value.ToString();
                }
                catch { }
            }
            return retVal;
        }

        public static decimal GetDecimal(object value, decimal defaultValue)
        {
            decimal retVal = defaultValue;
            if (value != null)
            {
                try
                {
                    retVal = Convert.ToDecimal(value);
                }
                catch { }
            }
            return retVal;
        }

        public static Double GetDouble(object value, Double defaultValue)
        {
            Double retVal = defaultValue;
            if (value != null)
            {
                try
                {
                    retVal = Convert.ToDouble(value);
                }
                catch { }
            }
            return retVal;
        }

        public static long GetLong(object value, long defaultValue)
        {
            long retVal = defaultValue;
            if (value != null)
            {
                try
                {
                    retVal = Convert.ToInt64(value);
                }
                catch { }
            }
            return retVal;
        }

        public static float GetFloat(object value, float defaultValue)
        {
            float retVal = defaultValue;
            if (value != null)
            {
                try
                {
                    retVal = Convert.ToSingle(value);
                }
                catch { }
            }
            return retVal;
        }

        public static DateTime GetDateTime(object value, DateTime defaultValue)
        {
            DateTime retVal = defaultValue;
            if (value != null)
            {
                try
                {
                    retVal = Convert.ToDateTime(value);
                }
                catch { }
            }
            return retVal;
        }

        public static byte GetByte(object value, byte defaultValue)
        {
            byte retVal = defaultValue;
            if (value != null)
            {
                try
                {
                    retVal = Convert.ToByte(value);
                }
                catch { }
            }
            return retVal;
        }

        public static byte[] GetByteArray(object value, byte[] defaultValue)
        {
            byte[] retVal = defaultValue;
            if (value != null)
            {
                try
                {
                    retVal = (byte[])value;
                }
                catch { }
            }
            return retVal;
        }

        public static Encoding GetEncoding(string encodeName, Encoding defaultValue)
        {
            Encoding retVal = defaultValue;
            if (!string.IsNullOrEmpty(encodeName))
            {
                try
                {
                    return Encoding.GetEncoding(encodeName);
                }
                catch { }
            }
            return retVal;
        }

        public static Encoding GetEncoding(int codePage, Encoding defaultValue)
        {
            Encoding retVal = defaultValue;
            if (codePage > 0)
            {
                try
                {
                    return Encoding.GetEncoding(codePage);
                }
                catch { }
            }
            return retVal;
        }

        #endregion

        /// <summary>
        /// 是否是整数类型
        /// </summary>
        public static bool IsIntegerType(Type type)
        {
            if ((((!(type == typeof(int)) && !(type == typeof(short))) && (!(type == typeof(ushort)) && !(type == typeof(byte)))) && ((!(type == typeof(sbyte)) && !(type == typeof(uint))) && (!(type == typeof(long)) && !(type == typeof(ulong))))) && !(type == typeof(char)))
            {
                return (type == typeof(bool));
            }
            return true;
        }

        /// <summary>
        /// 是否是小数类型
        /// </summary>
        public static bool IsDecimalsType(Type type)
        {
            if ((!(type == typeof(double)) && !(type == typeof(float))) && !(type == typeof(decimal)))
            {
                return (type == typeof(bool));
            }
            return true;
        }

        /// <summary>
        /// 转换元素的类型,目标的类型必须与当前类型兼容
        /// </summary>
        public static T ChangeType<T>(object obj)
        {
            T retVal = default(T);
            try
            {
                if (obj != null)
                {
                    if (!obj.IsReferenceObject())
                        return (T)Convert.ChangeType(obj, typeof(T));
                    else
                        return (T)obj;
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 转换所有元素,目标的类型必须与当前类型兼容
        /// </summary>
        public static IEnumerable<T> Cast<T>(System.Collections.IEnumerable source)
        {
            List<T> retVal = null;
            if (source != null)
            {
                retVal = new List<T>();
                foreach (var item in source)
                {
                    retVal.Add(ChangeType<T>(item));
                }
            }
            return retVal;
        }

        /// <summary>
        /// 保留指定小数位[不进行四舍五入]
        /// </summary>
        /// <param name="value">Decimal值</param>
        /// <param name="decimals">小数位</param>
        /// <returns></returns>
        public static decimal Round(decimal value, int decimals)
        {
            var tmpString = value.ToString();
            var index = tmpString.IndexOf(".");
            var length = tmpString.Length;

            if (index != -1)
            {
                return Convert.ToDecimal(string.Format("{0}.{1}",
                    tmpString.Substring(0, index),
                    tmpString.Substring(index + 1, Math.Min(length - index - 1, decimals))));
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 十进制或十六进制字符串转中文
        /// </summary>
        /// <param name="str">待转换字符串</param>
        /// <param name="fromBase">进制值[10,16]</param>
        /// <returns></returns>
        public static string ConvertChineseByDEC_HEX(string str, int fromBase)
        {
            var retVal = str;
            var strBuild = new StringBuilder();
            switch (fromBase)
            {
                case 10:
                    {
                        var colls = Regex.Matches(str, @"&#([\d]{5})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        if (colls != null && colls.Count > 0)
                        {
                            foreach (Match item in colls)
                            {
                                string w = item.Value.Substring(2);
                                w = Convert.ToString(int.Parse(w), 16);
                                byte[] c = new byte[2];
                                string ss = w.Substring(0, 2);
                                int c1 = Convert.ToInt32(w.Substring(0, 2), 16);
                                int c2 = Convert.ToInt32(w.Substring(2), 16);
                                c[0] = (byte)c2;
                                c[1] = (byte)c1;
                                strBuild.Append(Encoding.Unicode.GetString(c));
                            }
                            retVal = strBuild.ToString();
                        }
                    }
                    break;
                case 16:
                    {
                        var colls = Regex.Matches(str, @"\\u([\w]{2})([\w]{2})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        if (colls != null && colls.Count > 0)
                        {
                            foreach (Match item in colls)
                            {
                                string v = item.Value;
                                string w = v.Substring(2);
                                byte[] c = new byte[2];
                                int c1 = Convert.ToInt32(w.Substring(0, 2), 16);
                                int c2 = Convert.ToInt32(w.Substring(2), 16);
                                c[0] = (byte)c2;
                                c[1] = (byte)c1;
                                strBuild.Append(Encoding.Unicode.GetString(c));
                            }
                            retVal = strBuild.ToString();
                        }
                    }
                    break;
            }
            return retVal;
        }

        /// <summary>
        /// 字节数组转十六进制字符串
        /// </summary>
        public static string ConvertByteArrayToHexString(byte[] buffer, bool isUpperCase = true)
        {
            var builder = new StringBuilder();
            foreach (var item in buffer)
            {
                builder.Append(item.ToString(isUpperCase ? "X2" : "x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 十六进制字符串转字节数组
        /// </summary>
        public static byte[] ConvertHexStringToByteArray(string content)
        {
            var cntBuffer = new byte[content.Length / 2];
            for (int i = 0; i < cntBuffer.Length; i++)
            {
                cntBuffer[i] = (byte)Convert.ToInt32(content.Substring(i * 2, 2), 16);
            }
            return cntBuffer;
        }
    }
}
