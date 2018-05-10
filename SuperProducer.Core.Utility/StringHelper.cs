using SuperProducer.Core.Utility.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SuperProducer.Core.Utility
{
    public class StringHelper
    {
        /// <summary>
        /// 获取中文星期[如"星期日"]
        /// </summary>
        /// <param name="dateString">符合日期格式的字符串</param>
        /// <returns></returns>
        public static string GetChineseWeekName(string dateString)
        {
            try
            {
                var dayOfWeek = (int)ConvertHelper.GetDateTime(dateString).DayOfWeek;
                var weekdays = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
                return weekdays[dayOfWeek];
            }
            catch { }
            return dateString;
        }

        /// <summary>
        /// 获取中文姓氏[不支持复姓,如"杨"]
        /// </summary>
        public static string GetFamilyByChineseName(string chineseName)
        {
            if (RegExpHelper.IsRightLength(chineseName, 2))
            {
                return chineseName.Substring(0, 1);
            }
            return chineseName;
        }

        /// <summary>
        /// 隐藏中文姓名[不支持复姓,如"杨**"]
        /// </summary>
        public static string GetHiddenNameByChineseName(string chineseName, char replaceChar = '*')
        {
            if (RegExpHelper.IsRightLength(chineseName, 2))
            {
                return string.Format("{0}{1}", GetFamilyByChineseName(chineseName), new string(replaceChar, 2));
            }
            return chineseName;
        }

        /// <summary>
        /// 字符串隐藏类型
        /// </summary>
        public enum StringHiddenType
        {
            /// <summary>
            /// 开始部分
            /// </summary>
            Start = 1,

            /// <summary>
            /// 中间部分
            /// </summary>
            Middle = 2,

            /// <summary>
            /// 结尾部分
            /// </summary>
            End = 3,
        }

        /// <summary>
        /// 获取指定字符隐藏后的字符串
        /// </summary>
        /// <param name="str">待隐藏字符串</param>
        /// <param name="hiddenType">字符串隐藏类型</param>
        /// <param name="hiddenLength">隐藏长度</param>
        /// <param name="hiddenChar">隐藏字符</param>
        /// <returns></returns>
        public static string GetHiddenString(string str, StringHiddenType hiddenType = StringHiddenType.Start, int hiddenLength = 4, char hiddenChar = '*')
        {
            string retVal = str;
            if (RegExpHelper.IsRightLength(str, (uint)hiddenLength + 1))
            {
                if (hiddenType == StringHiddenType.Start)
                {
                    retVal = string.Format("{0}{1}", new string(hiddenChar, hiddenLength), str.Substring(hiddenLength));
                }
                else if (hiddenType == StringHiddenType.End)
                {
                    retVal = string.Format("{0}{1}", str.Substring(0, str.Length - hiddenLength), new string(hiddenChar, hiddenLength));
                }
                else if (hiddenType == StringHiddenType.Middle)
                {
                    var tmpArray = str.ToCharArray();
                    for (int i = 0; i < hiddenLength; i++)
                    {
                        var j = (tmpArray.Length - i) / 2;
                        while (j < tmpArray.Length && tmpArray[j] == '*')
                            j++;
                        tmpArray[j] = '*'; // 后面改成移除方式
                    }
                    retVal = new string(tmpArray);
                }
            }
            return retVal;
        }

        /// <summary>
        /// 获取随机数字串
        /// </summary>
        public static string GetRandomNumberString(int length)
        {
            return GetRandomString(length, "0123456789");
        }

        /// <summary>
        /// 获取随机英文字母串
        /// </summary>
        public static string GetRandomCaseString(int length)
        {
            return GetRandomString(length, "abcdefghigklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ");
        }

        /// <summary>
        /// 获取随机小写和数字字符串
        /// </summary>
        public static string GetRandomNumberLowerCaseString(int length)
        {
            return GetRandomString(length, "0123456789abcdefghigklmnopqrstuvwxyz");
        }

        /// <summary>
        /// 获取随机大写和数字字符串
        /// </summary>
        public static string GetRandomNumberUpperCaseString(int length)
        {
            return GetRandomString(length, "0123456789ABCDEFGHIGKLMNOPQRSTUVWXYZ");
        }

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        public static string GetRandomString(int length, string randString = null)
        {
            string retVal = string.Empty;
            if (length > 0 && !string.IsNullOrEmpty(randString))
            {
                string temp = randString;
                for (int i = 0; i < length; i++)
                    retVal += temp[new Random(Guid.NewGuid().GetHashCode()).Next(0, temp.Length)];
            }
            return retVal;
        }

        /// <summary>
        /// 获取随机字符串[大写字母和数字]
        /// </summary>
        public static string GetRandomString(int length)
        {
            string retVal = string.Empty;
            if (length > 0)
            {
                while (true)
                {
                    retVal += new MD5().Encrypt(Guid.NewGuid().ToString().Replace("-", "") + DateTime.Now.Ticks);
                    if (retVal.Length >= length)
                    {
                        retVal = retVal.Substring(0, length);
                        break;
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// 替换SQL关键字
        /// </summary>
        public static string ReplaceSQLKeywords(string str, string replaceStr = "")
        {
            if (!string.IsNullOrEmpty(str))
            {
                while (true)
                {
                    if (!RegExpHelper.IsContainSQLKeywords(str))
                        break;
                    str = Regex.Replace(str, RegExpHelper.ExpressionSQLKeywords, replaceStr, RegexOptions.IgnoreCase);
                }
            }
            return str;
        }

        /// <summary>
        /// 分割字符串[不返回NULL]
        /// </summary>
        public static List<string> SplitString(string str, string splitChar = null, StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries)
        {
            var retVal = new List<string>();
            if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(splitChar) && Enum.IsDefined(typeof(StringSplitOptions), splitOptions))
            {
                retVal.AddRange(str.Split(new string[] { splitChar }, splitOptions));
            }
            return retVal;
        }

        /// <summary>
        /// 组合Url网址
        /// </summary>
        public static string CombineUrlString(string url, Dictionary<string, string> query)
        {
            var retVal = string.Empty;
            try
            {
                var tmpQueryString = GetQueryStringByDictionary(query);
                if (!string.IsNullOrEmpty(tmpQueryString))
                {
                    retVal = CombineUrlString(url, tmpQueryString);
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 组合Url网址
        /// </summary>
        public static string CombineUrlString(string url, string query)
        {
            var retVal = string.Empty;
            try
            {
                string sitePart, queryPart;
                if (GetSitePathOrParameter(url, out sitePart, out queryPart))
                {
                    var builder = new StringBuilder();
                    builder.Append(sitePart.Trim('?'));
                    builder.Append('?');
                    builder.Append(queryPart.Trim('&'));
                    builder.Append('&');
                    builder.Append(query.Trim('&'));
                    retVal = builder.ToString();
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 传入完整Url获取站点路径[?之前的网址部分]
        /// </summary>
        public static string GetSitePath(string url)
        {
            string sitePart, queryPart;
            if (GetSitePathOrParameter(url, out sitePart, out queryPart))
            {
                return sitePart;
            }
            return string.Empty;
        }

        /// <summary>
        /// 传入完整Url获取QueryString
        /// </summary>
        public static string GetQueryString(string url)
        {
            string sitePart, queryPart;
            if (GetSitePathOrParameter(url, out sitePart, out queryPart))
            {
                return queryPart;
            }
            return string.Empty;
        }

        /// <summary>
        /// 传入完整Url通过输出参数获得网址和参数部分[以?区分]
        /// </summary>
        public static bool GetSitePathOrParameter(string url, out string sitePart, out string queryPart)
        {
            sitePart = string.Empty;
            queryPart = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    sitePart = url;
                    queryPart = string.Empty;

                    if (url.Contains("?"))
                    {
                        try
                        {
                            sitePart = url.Substring(0, url.IndexOf('?'));
                            queryPart = url.Substring(url.IndexOf('?') + 1);
                        }
                        catch { }
                    }

                    return true;
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// 传入字典类型得到QueryString格式的字符串
        /// </summary>
        public static string GetQueryStringByDictionary(Dictionary<string, string> dics, bool urlEncode = false)
        {
            var retVal = string.Empty;
            try
            {
                if (dics != null && dics.Count > 0)
                {
                    foreach (var item in dics)
                    {
                        retVal += string.Format("{0}={1}&", item.Key, urlEncode ? HttpUtility.UrlEncode(item.Value) : item.Value);
                    }
                    retVal = retVal.TrimEnd('&');
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 传入Url参数格式的字符串得到字典类型
        /// </summary>
        public static Dictionary<string, string> GetDictionaryByQueryString(string url, bool urlDecode = false)
        {
            Dictionary<string, string> retVal = null;
            try
            {
                string sitePart, queryPart;
                if (GetSitePathOrParameter(url, out sitePart, out queryPart))
                {
                    var allParas = SplitString(queryPart, "&");
                    if (allParas != null && allParas.Count > 0)
                    {
                        retVal = new Dictionary<string, string>();

                        foreach (var item in allParas)
                        {
                            var tmpItemList = SplitString(item, "=");
                            if (tmpItemList.Count == 2)
                            {
                                retVal.Add(tmpItemList.First(), urlDecode ? HttpUtility.UrlDecode(tmpItemList.Last()) : tmpItemList.Last());
                            }
                        }
                    }
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 获取汉字字符串的首字母拼音[大写]
        /// </summary>
        public static string GetChineseSpellCode(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                StringBuilder builder = new StringBuilder();
                foreach (char item in str)
                {
                    builder.Append(GetChineseSpellCodeByChar(item.ToString()));
                }
                return builder.ToString();
            }
            return str;
        }

        private static string GetChineseSpellCodeByChar(string str)
        {
            long charNumber;

            byte[] buffer = Encoding.GetEncoding("GB2312").GetBytes(str);

            if (buffer.Length == 1)
            {
                return str.ToUpper();
            }
            else
            {
                int n1 = (short)(buffer[0]);
                int n2 = (short)(buffer[1]);
                charNumber = n1 * 256 + n2;
            }

            if ((charNumber >= 45217) && (charNumber <= 45252))
                return "A";
            else if ((charNumber >= 45253) && (charNumber <= 45760))
                return "B";
            else if ((charNumber >= 45761) && (charNumber <= 46317))
                return "C";
            else if ((charNumber >= 46318) && (charNumber <= 46825))
                return "D";
            else if ((charNumber >= 46826) && (charNumber <= 47009))
                return "E";
            else if ((charNumber >= 47010) && (charNumber <= 47296))
                return "F";
            else if ((charNumber >= 47297) && (charNumber <= 47613))
                return "G";
            else if ((charNumber >= 47614) && (charNumber <= 48118))
                return "H";
            else if ((charNumber >= 48119) && (charNumber <= 49061))
                return "J";
            else if ((charNumber >= 49062) && (charNumber <= 49323))
                return "K";
            else if ((charNumber >= 49324) && (charNumber <= 49895))
                return "L";
            else if ((charNumber >= 49896) && (charNumber <= 50370))
                return "M";
            else if ((charNumber >= 50371) && (charNumber <= 50613))
                return "N";
            else if ((charNumber >= 50614) && (charNumber <= 50621))
                return "O";
            else if ((charNumber >= 50622) && (charNumber <= 50905))
                return "P";
            else if ((charNumber >= 50906) && (charNumber <= 51386))
                return "Q";
            else if ((charNumber >= 51387) && (charNumber <= 51445))
                return "R";
            else if ((charNumber >= 51446) && (charNumber <= 52217))
                return "S";
            else if ((charNumber >= 52218) && (charNumber <= 52697))
                return "T";
            else if ((charNumber >= 52698) && (charNumber <= 52979))
                return "W";
            else if ((charNumber >= 52980) && (charNumber <= 53640))
                return "X";
            else if ((charNumber >= 53689) && (charNumber <= 54480))
                return "Y";
            else if ((charNumber >= 54481) && (charNumber <= 55289))
                return "Z";
            else
                return ("?");
        }


        #region "中国大陆身份证"

        /// <summary>
        /// 是否是有效的中国大陆身份证[严格校验]
        /// </summary>
        public static bool IsChinaPRIDCard(string idCardNo)
        {
            if (string.IsNullOrEmpty(idCardNo))
            {
                return false;
            }

            var tmpType = idCardNo.Length;
            var tmpIndex = idCardNo.Length - 1;
            var tmpValue = idCardNo.Remove(tmpIndex);
            if (!RegExpHelper.IsAllNumber(tmpValue, (uint)tmpIndex, (uint)tmpIndex) || ConvertHelper.GetLong(tmpValue, 0) < Math.Pow(10, tmpIndex - 1))
            {
                return false;
            }

            var provinceCodes = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (provinceCodes.IndexOf(idCardNo.Remove(2)) < 0)
            {
                return false;
            }

            var interval = tmpType == 15 ? 2 : 0;
            var birthday = ConvertHelper.GetDateTime(idCardNo.Substring(6, 8 - interval).Insert(6 - interval, "-").Insert(4 - interval, "-"));
            if (birthday < InternalConstant.DefaultMSdbMinDate)
            {
                return false;
            }

            if (tmpType == 15)
            {
                return true;
            }

            // 继续18位校验
            var allCsNumber = new string[] { "1", "0", "x", "9", "8", "7", "6", "5", "4", "3", "2" };
            var allWiNumber = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            var allAiNumber = ConvertHelper.Cast<int>(tmpValue.ToList().ConvertAll((item) => { return item.ToString(); })).ToArray();

            var sum = 0;
            for (int i = 0; i < tmpIndex; i++)
            {
                sum += allAiNumber[i] * allWiNumber[i];
            }

            var mod = 0;
            Math.DivRem(sum, 11, out mod);
            if (allCsNumber[mod] != idCardNo.Substring(tmpIndex, 1).ToLower())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 中国大陆身份证15位转18位
        /// </summary>
        public static string ConvertChinaPRIDCard_15_18(string idCardNo)
        {
            if (!RegExpHelper.IsAllNumber(idCardNo, 15, 15))
            {
                return idCardNo;
            }

            var allCsNumber = new string[] { "1", "0", "x", "9", "8", "7", "6", "5", "4", "3", "2" };
            var allWiNumber = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };

            var tmpNewNumber = new StringBuilder(idCardNo);

            tmpNewNumber.Insert(6, "19");

            var sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += ConvertHelper.GetInt(tmpNewNumber[i].ToString()) * allWiNumber[i];
            }
            sum %= 11;

            tmpNewNumber.Append(allCsNumber[sum]);

            return tmpNewNumber.ToString();
        }

        #endregion
    }
}
