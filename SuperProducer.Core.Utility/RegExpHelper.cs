using System;
using System.Text.RegularExpressions;

namespace SuperProducer.Core.Utility
{
    public class RegExpHelper
    {
        #region 正则常量字符串

        /// <summary>
        /// 纯数字
        /// </summary>
        public const string ExpressionAllNumber = @"^[0-9]+$";

        /// <summary>
        /// 纯字母
        /// </summary>
        public const string ExpressionAllLetter = @"^[a-zA-Z]+$";

        /// <summary>
        /// 纯汉字
        /// </summary>
        public const string ExpressionAllChineses = @"^[\u4e00-\u9fa5]+$";

        /// <summary>
        /// 整数
        /// </summary>
        public const string ExpressionNumberic = @"^\-?[0-9]+$";

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public const string ExpressionEmail = @"^(\w)+(\.\w+)*@(\w)+((\.\w+)+)$";

        /// <summary>
        /// QQ号码
        /// </summary>
        public const string ExpressionQQNo = @"^[0-9]{5,}$";

        /// <summary>
        /// 微信号码
        /// </summary>
        public const string ExpressionWechatNo = @"^[A-Za-z0-9_]{5,}$";

        /// <summary>
        /// 中国大陆手机号码
        /// </summary>
        public const string ExpressionChinaPRMobileNo = @"^[1][3-5|7-8]\d{9}$";

        /// <summary>
        /// IPV4地址
        /// </summary>
        public const string ExpressionIPAddressV4 = @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$";

        /// <summary>
        /// IPV6地址
        /// </summary>
        public const string ExpressionIPAddressV6 = @"^([\da-fA-F]{1,4}:){7}[\da-fA-F]{1,4}$|^:((:[\da-fA-F]{1,4}){1,6}|:)$|^[\da-fA-F]{1,4}:((:[\da-fA-F]{1,4}){1,5}|:)$|^([\da-fA-F]{1,4}:){2}((:[\da-fA-F]{1,4}){1,4}|:)$|^([\da-fA-F]{1,4}:){3}((:[\da-fA-F]{1,4}){1,3}|:)$|^([\da-fA-F]{1,4}:){4}((:[\da-fA-F]{1,4}){1,2}|:)$|^([\da-fA-F]{1,4}:){5}:([\da-fA-F]{1,4})?$|^([\da-fA-F]{1,4}:){6}:$";

        /// <summary>
        /// SQL关键字
        /// </summary>
        public const string ExpressionSQLKeywords = @"select |insert |delete |update |drop |truncate |exec | and | or |count\(|asc\(|mid\(|char\(|\'|\;";

        /// <summary>
        /// Unicode编码
        /// </summary>
        public const string ExpressionUnicode = @"^[\u4E00-\u9FA5\uE815-\uFA29]+$";

        /// <summary>
        /// Url地址
        /// </summary>
        public const string ExpressionUrl = @"^(http|https|ftp|rtsp|mms):(\/\/|\\\\)[A-Za-z0-9%\-_@]+\.[A-Za-z0-9%\-_@]+[A-Za-z0-9\.\/=\?%\-&_~`@:\+!;\{\}]*$";

        /// <summary>
        /// 非标准的Url地址
        /// </summary>
        public const string ExpressionNonStandardUrl = @"^(((ht|f)tp(s?))\://)?[A-Za-z0-9%\-_@]+\.[A-Za-z0-9%\-_@]+[A-Za-z0-9\.\/=\?%\-&_~`@:\+!;]*$";

        /// <summary>
        /// 中国大陆身份证
        /// </summary>
        public const string ExpressionChinaPRIDCard = @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$";

        /// <summary>
        /// 标准用户名
        /// </summary>
        public const string ExpressionStandardUserName = @"^[a-zA-z][a-zA-Z0-9_]{6,30}$";

        /// <summary>
        /// 标准用户密码
        /// </summary>
        public const string ExpressionStandardUserPass = @"^[!@#$%&*\-.?_A-Za-z0-9]{6,20}$";

        /// <summary>
        /// 简单用户密码
        /// </summary>
        public const string ExpressionSimpleUserPass = @"^(000000|00000000|000000000|0000000000|0123456789|100200|110110|110110110|111111|1111111|11111111|111111111|1111111111|11112222|111213|111222|111222tianya|112233|11223344|121212|12121212|123123|123123123|123321|123321123|12341234|12344321|123456|123456123|1234567|12345678|123456789|1234567890|123456789a|123456a|123456abc|123456q|1234qwer|123654|123654789|123654987|123698745|123qwe|1314520|1314521|137900|139.com|147258|147258369|159357|162534|163.com|168816|1814325|18n28n24a5|195561|1984130|198428|198541|19860210|198773|19890608|1q2w3e4r|1qaz2wsx|222222|22222222|280213676|31415926|328658|369369|4861111|502058|520131|5201314|520520|5211314|521521|5403693|540707|601445|6516415|654321|6586123|666666|66666666|690929|7758258|7758521|780525|789456123|81251310|87654321|880126|888888|88888888|987654321|9958123|999999|a000000|a111111|a123123|a123456|a1234567|a12345678|a123456789|a321654|a5201314|aaaaaa|aaaaaaaa|abc123|abc123456|abcd1234|as1230|asd123|asdasd|asdasdasd|asdfghjkl|caonima99|dearbook|hahabaobao|hotmail.com|iloveyou|leqing123|liuchang|ohwe1zvq|password|pp.com|q123456|q1q1q1q1|qazwsxedc|qq.com|qq123456|qq776491|qqqqqqqq|qwe123|qwertyui|qwertyuiop|RAND#a#8|sohu.com|tom.com|wangyut2|woaini|woaini123|woaini1314|woaini520|woaini521|xiazhili|xiekai1121|yahoo.cn|yahoo.com|yahoo.com.cn|z123456|zxcvbnm|zz123456)$";

        /// <summary>
        /// 支付密码(6位数字)
        /// </summary>
        public const string ExpressionStandardUserPayPass = @"^\d{6}$";

        /// <summary>
        /// 中国大陆车辆牌照
        /// </summary>
        public const string ExpressionChinaPRCarNo = "^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1}$";

        #endregion



        /// <summary>
        /// 是否是正确的长度
        /// </summary>
        public static bool IsRightLength(string str, uint minLength = 1, uint maxLength = uint.MaxValue)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return str.Length >= minLength && str.Length <= maxLength;
        }

        /// <summary>
        /// 是否是纯数字
        /// </summary>
        public static bool IsAllNumber(string str, uint minLength = 1, uint maxLength = uint.MaxValue)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionAllNumber) && str.Length >= minLength && str.Length <= maxLength;
        }

        /// <summary>
        /// 是否是纯字母
        /// </summary>
        public static bool IsAllLetter(string str, uint minLength = 1, uint maxLength = uint.MaxValue)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionAllLetter) && str.Length >= minLength && str.Length <= maxLength;
        }

        /// <summary>
        /// 是否是纯汉字
        /// </summary>
        public static bool IsAllChinese(string str, uint minLength = 1, uint maxLength = uint.MaxValue)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionAllChineses) && str.Length >= minLength && str.Length <= maxLength;
        }

        /// <summary>
        /// 是否是整数
        /// </summary>
        public static bool IsNumeric(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionNumberic);
        }

        /// <summary>
        /// 是否是电子邮件
        /// </summary>
        public static bool IsEmail(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionEmail);
        }

        /// <summary>
        /// 是否是中国大陆手机号
        /// </summary>
        public static bool IsChinaPRMobileNo(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionChinaPRMobileNo);
        }

        /// <summary>
        /// 是否是IPV4地址
        /// </summary>
        public static bool IsIPAddressV4(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionIPAddressV4);
        }

        /// <summary>
        /// 是否是IPV6地址
        /// </summary>
        public static bool IsIPAddressV6(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionIPAddressV6);
        }

        /// <summary>
        /// 是否是绝对路径[有问题]
        /// </summary>
        public static bool IsPhysicalPath(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            string exps = @"^\s*[a-zA-Z]:.*$";
            return Regex.IsMatch(str, exps);
        }

        /// <summary>
        /// 是否是相对路径[有问题]
        /// </summary>
        public static bool IsRelativePath(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            if (str.StartsWith("/") || str.StartsWith("?"))
                return false;

            string exps = @"^\s*[a-zA-Z]{1,10}:.*$";
            if (Regex.IsMatch(str, exps))
                return false;

            return true;
        }

        /// <summary>
        /// 是否是包含SQL关键字
        /// </summary>
        public static bool IsContainSQLKeywords(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            str = Regex.Replace(str, @"\s", " ");
            str = Regex.Replace(str, @"%20", " ");
            return Regex.IsMatch(str, ExpressionSQLKeywords, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 是否是Unicode
        /// </summary>
        public static bool IsUnicode(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionUnicode);
        }

        /// <summary>
        /// 是否是Url地址
        /// </summary>
        public static bool IsUrl(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionUrl);
        }

        /// <summary>
        /// 是否是中国大陆身份证号
        /// 验证以下3种情况:
        /// 1. 身份证号码为15位数字
        /// 2. 身份证号码为18位数字
        /// 3. 身份证号码为17位数字+1个字母  
        /// </summary>
        public static bool IsChinaPRIDCard(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionChinaPRIDCard);
        }

        /// <summary>
        /// 是否是标准用户名
        /// </summary>
        /// <param name="str">开头必须是字母,可包含大小写字母,数字,下划线,6-30位</param>
        /// <returns></returns>
        public static bool IsStandardUserName(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionStandardUserName);
        }

        /// <summary>
        /// 是否是标准用户密码
        /// </summary>
        public static bool IsStandardUserPass(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionStandardUserPass);
        }

        /// <summary>
        /// 是否是简单用户密码
        /// </summary>
        public static bool IsSimpleUserPass(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionSimpleUserPass);
        }

        /// <summary>
        /// 是否是标准支付密码
        /// </summary>
        public static bool IsStandardUserPayPass(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionStandardUserPayPass);
        }

        /// <summary>
        /// 是否是中国大陆车辆牌照
        /// </summary>
        public static bool IsChinaPRCarNo(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return Regex.IsMatch(str, ExpressionChinaPRCarNo);
        }
    }
}
