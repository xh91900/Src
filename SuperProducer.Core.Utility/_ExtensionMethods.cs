using SuperProducer.Core.Utility.Encrypt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SuperProducer.Core.Utility
{
    public static class ExtensionMethods
    {
        #region "System.Object"

        /// <summary>
        /// 是否是引用对象
        /// </summary>
        public static bool IsReferenceObject(this object value)
        {
            return value != null && !value.GetType().IsValueType && value.GetType() != typeof(string);
        }

        /// <summary>
        /// 替换掉对象属性值中的SQL关键字
        /// </summary>
        public static void ReplaceSQLKeywords(this object value)
        {
            if (value.IsReferenceObject())
            {
                Action<object> fn = null;
                fn = (obj) =>
                {
                    foreach (var item in ObjectHelper.GetProperties(obj))
                    {
                        try
                        {
                            var tmpValue = item.GetValue(obj);
                            if (tmpValue != null)
                            {
                                if (item.PropertyType == typeof(string))
                                {
                                    item.SetValue(obj, ConvertHelper.GetString(tmpValue).ReplaceSQLKeywords());
                                }
                                else if (!item.PropertyType.IsValueType)
                                {
                                    fn(tmpValue);
                                }
                            }
                        }
                        catch { }
                    }
                };
                fn(value);
            }
        }

        #endregion

        #region "System.Int64"

        public static DateTime ToDatetime(this Int64 timestamp)
        {
            var retVal = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            if (timestamp > 0)
            {
                return retVal.AddSeconds(timestamp);
            }
            return retVal;
        }

        #endregion

        #region "System.DataTime"

        /// <summary>
        /// 将时间精确到哪个级别
        /// </summary>
        public static DateTime CutOff(this DateTime dateTime, long cutTicks = TimeSpan.TicksPerSecond)
        {
            return new DateTime(dateTime.Ticks - (dateTime.Ticks % cutTicks), dateTime.Kind);
        }

        /// <summary>
        /// 时间转换为字符串[2018-01-01]
        /// </summary>
        public static string ToCnDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 时间转换为字符串[2018-01-01 11:11:11]
        /// </summary>
        public static string ToCnDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 转换为unix时间戳
        /// </summary>
        public static long ToUnixTimestamp(this DateTime datetime)
        {
            var start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return Convert.ToInt64((datetime - start).TotalSeconds);
        }

        /// <summary>
        /// 获取某月的第一天
        /// </summary>
        public static DateTime FirstDayOfMonth(this DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }

        /// <summary>
        /// 获取某月的最后一天
        /// </summary>
        public static DateTime LastDayOfMonth(this DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 获取上个月第一天
        /// </summary>
        public static DateTime FirstDayOfPreviousMonth(this DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(-1);
        }

        /// <summary>
        /// 获取上个月的最后一天
        /// </summary>
        public static DateTime LastDayOfPrdviousMonth(this DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddDays(-1);
        }

        #endregion

        #region "System.Double"

        /// <summary>
        ///小数转换为整数[满五加一]
        /// </summary>
        public static int ToInt(this double value)
        {
            return ((decimal)value).ToInt();
        }

        /// <summary>
        /// 小数转换为价格[如3.125123会转成3.13]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format">小数位数格式</param>
        /// <returns></returns>
        public static string ToPrice(this double value, string format = "0.00")
        {
            return value.ToString(format);
        }

        /// <summary>
        /// 小数转换为评分[如3.6转成4,3.3转成3.5,3转成3]
        /// </summary>
        public static double ToScore(this double value)
        {
            var leftNum = value - (int)value;
            if (0 < leftNum && leftNum <= 0.5)
                return ((int)value) + 0.5;
            else if (0 < leftNum && leftNum > 0.5)
                return ((int)value) + 1;
            return value;
        }

        #endregion

        #region "System.Decimal"

        /// <summary>
        /// 小数转换为整数[满0.5加1]
        /// </summary>
        public static int ToInt(this decimal value)
        {
            var leftNum = value - (int)value;
            if (leftNum >= 0.5m)
                return ((int)value) + 1;
            else
                return (int)value;
        }

        /// <summary>
        /// 小数转换为价格[如3.125123会转成3.13]
        /// </summary>
        public static decimal ToPrice(this decimal value)
        {
            return ConvertHelper.GetDecimal(value.ToPrice("0.00"));
        }

        /// <summary>
        /// 小数转换为价格[如3.125123会转成3.13]
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format">小数位数格式</param>
        /// <returns></returns>
        public static string ToPrice(this decimal value, string format = "0.00")
        {
            return value.ToString(format);
        }

        /// <summary>
        /// 小数转换为中文价格[如"¥200.00",小于0时转换为"暂无价格"]
        /// </summary>
        public static string ToCnPrice(this decimal value, string format = "0.00")
        {
            if (value < 0)
                return "暂无价格";
            return string.Format("&yen;{0}", value.ToString(format));
        }

        /// <summary>
        /// 小数转换为整数型的价格区间[如200-300]
        /// </summary>
        public static string ToPriceRange(this decimal fromValue, decimal toValue)
        {
            if (fromValue == toValue)
                return fromValue.ToCnPrice("0");
            else
                return string.Format("{0}-{1}", fromValue.ToCnPrice("0"), toValue.ToCnPrice("0"));
        }

        #endregion

        #region "System.String"

        /// <summary>
        /// 字符串转换为MD5字符串
        /// </summary>
        public static string ToMD5(this string str)
        {
            return new MD5().Encrypt(str);
        }

        /// <summary>
        /// 使用对象替换字符串里以对象属性命名的动态变量[如"{name}"]
        /// </summary>
        public static string ReplaceVariableByObject(this string str, object obj)
        {
            if (!string.IsNullOrEmpty(str) && obj != null)
            {
                var tmpProperties = ObjectHelper.GetProperties(obj);
                foreach (var item in tmpProperties)
                {
                    str = str.Replace("{" + item.Name + "}", ConvertHelper.GetString(item.GetValue(obj)));
                }
            }
            return str;
        }

        /// <summary>
        /// 替换掉字符串中的SQL关键字
        /// </summary>
        public static string ReplaceSQLKeywords(this string str, string replaceStr = "")
        {
            return StringHelper.ReplaceSQLKeywords(str, replaceStr);
        }

        #endregion

        #region "System.Data.DataTable"

        public static bool AddColumn(this DataTable dtl, string columnName, Type columnType = null, object defaultData = null)
        {
            bool retVal = false;
            try
            {
                if (dtl != null)
                {
                    if (columnType != null)
                        dtl.Columns.Add(columnName, columnType);
                    else
                        dtl.Columns.Add(columnName);

                    if (dtl.Rows.Count > 0 && defaultData != null)
                    {
                        foreach (DataRow item in dtl.Rows)
                            item[columnName] = defaultData;
                    }

                    retVal = dtl.Columns.Contains(columnName);
                }
            }
            catch { }
            return retVal;
        }

        public static List<T> ToList<T>(this DataTable dtl) where T : new()
        {
            try
            {
                if (dtl != null && dtl.Rows.Count > 0)
                {
                    var lstDataInfo = new List<T>();
                    foreach (DataRow row in dtl.Rows)
                    {
                        T data = new T();
                        var allPropertys = ObjectHelper.GetProperties(typeof(T), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var item in allPropertys)
                        {
                            try
                            {
                                var attr = item.GetCustomAttribute<ColumnAttribute>();
                                if (attr != null && dtl.Columns.Contains(attr.Name))
                                {
                                    object tempValue = row[attr.Name];
                                    if (tempValue == DBNull.Value || tempValue == null)
                                        tempValue = ObjectHelper.DefaultForType(item.PropertyType);
                                    item.SetValue(data, tempValue);
                                    continue;
                                }

                                if (dtl.Columns.Contains(item.Name))
                                {
                                    object tempValue = row[item.Name];
                                    if (tempValue == DBNull.Value)
                                        tempValue = ObjectHelper.DefaultForType(item.PropertyType);
                                    item.SetValue(data, tempValue);
                                    continue;
                                }
                            }
                            catch { }
                        }
                        lstDataInfo.Add(data);
                    }
                    return lstDataInfo;
                }
            }
            catch { }
            return null;
        }

        #endregion

        #region "System.Collections.Generic.IEnumerable<T>"

        /// <summary>
        /// 从数组或集合中随机挑选一个元素
        /// </summary>
        public static T Random<T>(this IEnumerable<T> colls)
        {
            return colls.Random<T>(1).SingleOrDefault();
        }

        /// <summary>
        /// 从数组或集合中随机挑选N个元素
        /// </summary>
        public static IEnumerable<T> Random<T>(this IEnumerable<T> colls, int takeCount)
        {
            var rand = new Random();
            return colls.OrderBy(c => rand.Next()).Take(takeCount);
        }

        /// <summary>
        /// 转换数组或集合中对象的类型为T类型,类型不需要互相兼容
        /// </summary>
        public static List<T> DeepCast<T>(this IEnumerable colls) where T : new()
        {
            if (colls != null)
            {
                List<T> retVal = new List<T>();
                var tmpIterator = colls.GetEnumerator();
                while (tmpIterator.MoveNext())
                {
                    retVal.Add(ObjectHelper.DeepCopy<T>(tmpIterator.Current));
                }
                return retVal;
            }
            return null;
        }

        #endregion

        #region "System.Collections.Specialized.NameValueCollection"

        public static Dictionary<string, object> ToDictionary(this NameValueCollection value)
        {
            var retVal = new Dictionary<string, object>();
            try
            {
                var allKeys = value.AllKeys;
                foreach (var item in allKeys)
                {
                    retVal.Add(item, value[item]);
                }
            }
            catch { }
            return retVal;
        }

        #endregion

        #region "System.Collections.Generic.Dictionary<string, object>"

        public static T GetValue<T>(this Dictionary<string, object> dic, string key)
        {
            if (dic != null && dic.ContainsKey(key))
            {
                return ConvertHelper.ChangeType<T>(dic[key]);
            }
            return default(T);
        }

        /// <summary>
        /// 当key不存在时增加，存在时返回false
        /// </summary>
        public static bool AddItem(this Dictionary<string, object> dic, string key, object value)
        {
            if (dic != null && !dic.ContainsKey(key))
            {
                dic.Add(key, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 当key不存在时增加，存在时更新
        /// </summary>
        public static void UpdateItem(this Dictionary<string, object> dic, string key, object value)
        {
            if (dic != null)
            {
                if (!dic.ContainsKey(key))
                    dic.Add(key, value);
                else
                    dic[key] = value;
            }
        }

        #endregion

        #region "System.Collections.Generic.Dictionary<string, string>"

        public static string GetValue(this Dictionary<string, string> dic, string key)
        {
            if (dic != null && dic.ContainsKey(key))
            {
                return dic[key];
            }
            return null;
        }

        /// <summary>
        /// 当key不存在时增加，存在时返回false
        /// </summary>
        public static bool AddItem(this Dictionary<string, string> dic, string key, string value)
        {
            if (dic != null && !dic.ContainsKey(key))
            {
                dic.Add(key, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 当key不存在时增加，存在时更新
        /// </summary>
        public static void UpdateItem(this Dictionary<string, string> dic, string key, string value)
        {
            if (dic != null)
            {
                if (!dic.ContainsKey(key))
                    dic.Add(key, value);
                else
                    dic[key] = value;
            }
        }

        #endregion

        #region "System.Collections.Generic.SortedDictionary<string, string>"

        public static string GetValue(this SortedDictionary<string, string> dic, string key)
        {
            if (dic != null && dic.ContainsKey(key))
            {
                return dic[key];
            }
            return string.Empty;
        }

        /// <summary>
        /// 当key不存在时增加，存在时返回false
        /// </summary>
        public static bool AddItem(this SortedDictionary<string, string> dic, string key, string value)
        {
            if (dic != null && !dic.ContainsKey(key))
            {
                dic.Add(key, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 当key不存在时增加，存在时更新
        /// </summary>
        public static void UpdateItem(this SortedDictionary<string, string> dic, string key, string value)
        {
            if (dic != null)
            {
                if (!dic.ContainsKey(key))
                    dic.Add(key, value);
                else
                    dic[key] = value;
            }
        }

        #endregion

        #region "System.Collections.Generic.SortedDictionary<string, object>"

        public static T GetValue<T>(this SortedDictionary<string, object> dic, string key)
        {
            if (dic != null && dic.ContainsKey(key))
            {
                return ConvertHelper.ChangeType<T>(dic[key]);
            }
            return default(T);
        }

        /// <summary>
        /// 当key不存在时增加，存在时返回false
        /// </summary>
        public static bool AddItem(this SortedDictionary<string, object> dic, string key, object value)
        {
            if (dic != null && !dic.ContainsKey(key))
            {
                dic.Add(key, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 当key不存在时增加，存在时更新
        /// </summary>
        public static void UpdateItem(this SortedDictionary<string, object> dic, string key, object value)
        {
            if (dic != null)
            {
                if (!dic.ContainsKey(key))
                    dic.Add(key, value);
                else
                    dic[key] = value;
            }
        }

        #endregion

        #region "System.Reflection.Assembly"

        /// <summary>
        /// 获取程序集中所有的类型
        /// </summary>
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            return assembly != null ? assembly.GetTypes() : null;
        }

        #endregion

        #region "System.Linq.T"

        public static string GetVariableName<T>(this T name, Expression<Func<T, T>> exp)
        {
            return ((MemberExpression)exp.Body).Member.Name;
        }

        #endregion
    }
}
