using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SuperProducer.Core.Utility
{
    public class EnumHelper
    {
        /// <summary>
        /// 判断枚举值是否存在
        /// </summary>
        public static bool IsDefined(Type enumType, object value)
        {
            try
            {
                if (enumType.IsEnum && value != null)
                {
                    if (ConvertHelper.IsIntegerType(value.GetType()))
                        return enumType.IsEnumDefined(ConvertHelper.GetInt(value));
                    return enumType.IsEnumDefined(value);
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 转换字符串值到指定的枚举值
        /// </summary>
        /// <typeparam name="T">指定的枚举类型</typeparam>
        /// <param name="enumString">枚举字符串</param>
        /// <param name="defaultValue">默认值</param>
        public static T Parse<T>(string enumString, T defaultValue = default(T))
        {
            try
            {
                if (string.IsNullOrEmpty(enumString))
                    return defaultValue;
                return (T)Enum.Parse(typeof(T), enumString);
            }
            catch { }
            return defaultValue;
        }

        /// <summary>
        /// 获取所有的枚举键
        /// </summary>
        public static List<string> GetAllKey(Type enumType)
        {
            List<string> retVal = null;
            if (enumType.IsEnum)
            {
                retVal = new List<string>();
                try
                {
                    foreach (var item in enumType.GetEnumNames())
                    {
                        retVal.Add(item);
                    }
                }
                catch { }
            }
            return retVal;
        }

        /// <summary>
        /// 获取所有的枚举键
        /// </summary>
        public static List<int> GetAllValue(Type enumType)
        {
            List<int> retVal = null;
            if (enumType.IsEnum)
            {
                retVal = new List<int>();
                try
                {
                    foreach (var item in enumType.GetEnumValues())
                    {
                        retVal.Add((int)item);
                    }
                }
                catch { }
            }
            return retVal;
        }

        /// <summary>
        /// 获取所有枚举项
        /// </summary>
        public static List<T> GetAllItem<T>()
        {
            List<T> retVal = null;
            if (typeof(T).IsEnum)
            {
                retVal = new List<T>();
                try
                {
                    foreach (object item in GetAllValue(typeof(T)))
                    {
                        retVal.Add((T)item);
                    }
                }
                catch { }
            }
            return retVal;
        }

        /// <summary>
        /// 获取EnumTitle
        /// </summary>
        public static string GetEnumTitle(Enum e)
        {
            var title = GetEnumTitleAttribute(e);
            if (title != null)
            {
                return title.Title;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取EnumTitleAttribute
        /// </summary>
        public static EnumTitleAttribute GetEnumTitleAttribute(Enum e)
        {
            if (IsDefined(e.GetType(), e))
            {
                var field = e.GetType().GetField(e.ToString());
                return field.GetCustomAttribute<EnumTitleAttribute>();
            }
            return null;
        }

        /// <summary>
        /// 获取指定枚举类型的所有EnumTitleAttribute.Title
        /// </summary>
        public static List<string> GetEnumTitles(Type enumType)
        {
            var data = GetEnumTitleAttributes(enumType);
            if (data != null && data.Count > 0)
            {
                return data.Select(item => item.Title).ToList<string>();
            }
            return null;
        }

        /// <summary>
        /// 获取指定枚举类型的所有EnumTitleAttribute
        /// </summary>
        public static List<EnumTitleAttribute> GetEnumTitleAttributes(Type enumType)
        {
            List<EnumTitleAttribute> retVal = null;
            if (enumType != null && enumType.IsEnum)
            {
                var fields = enumType.GetFields();
                if (fields != null && fields.Length > 0)
                {
                    retVal = new List<EnumTitleAttribute>();
                    foreach (var item in fields)
                    {
                        var attr = item.GetCustomAttribute<EnumTitleAttribute>();
                        if (attr != null)
                        {
                            retVal.Add(attr);
                        }
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// 根据枚举的EnumTitleAttribute获取枚举值
        /// </summary>
        public static T GetEnumValueByEnumTitle<T>(string enumTitle)
        {
            var data = GetEnumAndEnumTitleAttribute<T>();
            if (data != null && data.Count > 0)
            {
                var target = data.Where(item => item.Value.Title == enumTitle).FirstOrDefault();
                if (target.Key != null && target.Value != null)
                {
                    return target.Key;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 获取指定枚举的各项枚举值以及对应的EnumTitleAttribute
        /// </summary>
        public static Dictionary<T, EnumTitleAttribute> GetEnumAndEnumTitleAttribute<T>()
        {
            var enumType = typeof(T);
            var fields = enumType.GetFields();
            if (fields != null && fields.Length > 0)
            {
                var tmpObject = AssemblyHelper.GetInstance(enumType.Assembly, enumType);
                if (tmpObject != null)
                {
                    var retVal = new Dictionary<T, EnumTitleAttribute>();
                    foreach (var item in fields)
                    {
                        var attr = item.GetCustomAttribute<EnumTitleAttribute>();
                        if (attr != null)
                        {
                            var tmpValue = item.GetValue(tmpObject);
                            retVal.Add((T)tmpValue, attr);
                        }
                    }
                    return retVal;
                }
            }
            return null;
        }
    }

    public class EnumTitleAttribute : Attribute
    {
        public EnumTitleAttribute() : this(null) { }

        public EnumTitleAttribute(string title, params string[] synonyms)
        {
            IsDisplay = true;
            Title = title;
            Synonyms = synonyms;
            Order = int.MaxValue;
        }

        public bool IsDisplay { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Letter { get; set; }

        /// <summary>
        /// 近义词
        /// </summary>
        public string[] Synonyms { get; set; }
        public int Category { get; set; }
        public int Order { get; set; }
    }
}
