using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Dynamic;

namespace SuperProducer.Core.Utility
{
    public class ObjectHelper
    {
        /// <summary>
        /// 获取对象的属性列表
        /// </summary>
        public static PropertyInfo[] GetProperties(object obj)
        {
            if (obj != null)
            {
                return GetProperties(obj.GetType());
            }
            return null;
        }

        /// <summary>
        /// 获取对象的属性列表
        /// </summary>
        public static PropertyInfo[] GetProperties(Type type, string propertyName = null)
        {
            if (type != null)
            {
                if (!string.IsNullOrEmpty(propertyName))
                {
                    return type.GetProperties().Where(item => item.Name == propertyName).ToArray();
                }
                return type.GetProperties();
            }
            return null;
        }

        /// <summary>
        /// 获取对象的属性列表
        /// </summary>
        public static PropertyInfo[] GetProperties(Type type, BindingFlags flags)
        {
            if (type != null)
            {
                return type.GetProperties(flags);
            }
            return null;
        }

        /// <summary>
        /// 获得对象的某属性值
        /// </summary>
        public static object GetPropertyValue(object obj, string propertyName)
        {
            return GetPropertyValue<object>(obj, propertyName);
        }

        /// <summary>
        /// 获得对象的某属性值
        /// </summary>
        public static T GetPropertyValue<T>(object obj, string propertyName)
        {
            try
            {
                if (obj != null)
                {
                    var tmpProperties = GetProperties(obj);
                    var tmpProperty = tmpProperties.Where(item => item.Name == propertyName).FirstOrDefault();
                    if (tmpProperty != null)
                    {
                        var tempValue = tmpProperty.GetValue(obj);
                        if (tmpProperty.PropertyType.IsValueType || tmpProperty.PropertyType == typeof(string))
                            return (T)Convert.ChangeType(tempValue, typeof(T));
                        else
                            return (T)tempValue;
                    }
                }
            }
            catch { }
            return default(T);
        }

        /// <summary>
        /// 设置对象的某属性值
        /// </summary>
        public static bool SetPropertyValue(object obj, string propertyName, object value)
        {
            if (obj != null)
            {
                return SetPropertyValue<object>(obj, propertyName, value);
            }
            return false;
        }

        /// <summary>
        /// 设置对象的某属性值
        /// </summary>
        public static bool SetPropertyValue<T>(object obj, string propertyName, T value)
        {
            try
            {
                if (obj != null)
                {
                    var tmpProperty = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (tmpProperty != null)
                    {
                        if (tmpProperty.PropertyType.IsValueType || tmpProperty.PropertyType == typeof(string))
                        {
                            tmpProperty.SetValue(obj, Convert.ChangeType(value, tmpProperty.PropertyType));
                        }
                        else
                        {
                            tmpProperty.SetValue(obj, value);
                        }
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 设置对象的某字段值
        /// </summary>
        public static bool SetFieldValue<T>(object obj, string fieldName, T value)
        {
            try
            {
                if (obj != null)
                {
                    var tmpProperty = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (tmpProperty != null)
                    {
                        if (tmpProperty.FieldType.IsValueType || tmpProperty.FieldType == typeof(string))
                        {
                            tmpProperty.SetValue(obj, Convert.ChangeType(value, tmpProperty.FieldType));
                        }
                        else
                        {
                            tmpProperty.SetValue(obj, value);
                        }
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        public static object DefaultForType(Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        /// <summary>
        /// 对象深拷贝
        /// </summary>
        public static T DeepCopy<T>(object sourceObject) where T : new()
        {
            T retVal = new T();
            DeepCopy(sourceObject, retVal);
            return retVal;
        }

        /// <summary>
        /// 对象深拷贝
        /// </summary>
        public static void DeepCopy(object sourceObject, object targetObject)
        {
            if (sourceObject != null && targetObject != null)
            {
                var sourcePropertys = GetProperties(sourceObject.GetType(), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var targetPropertys = GetProperties(targetObject);

                foreach (var item in targetPropertys)
                {
                    try
                    {
                        PropertyInfo tempPro = null;

                        var attr = item.GetCustomAttribute<ColumnAttribute>();
                        if (attr != null)
                        {
                            tempPro = sourcePropertys.Where(value => value.Name == attr.Name).FirstOrDefault();
                            if (tempPro != null)
                            {
                                item.SetValue(targetObject, tempPro.GetValue(sourceObject));
                                continue;
                            }
                        }

                        tempPro = sourcePropertys.Where(value => value.Name == item.Name).FirstOrDefault();
                        if (tempPro != null)
                        {
                            item.SetValue(targetObject, tempPro.GetValue(sourceObject));
                            continue;
                        }
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 对象转Dictionary
        /// </summary>
        public static Dictionary<string, object> ToDictionary(object obj, bool inherit = true)
        {
            Dictionary<string, object> retVal = null;
            if (obj != null)
            {
                var bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
                if (!inherit)
                    bindingFlags |= BindingFlags.DeclaredOnly;

                var tmpProperties = GetProperties(obj.GetType(), bindingFlags);
                if (tmpProperties != null && tmpProperties.Length > 0)
                {
                    retVal = new Dictionary<string, object>();
                    foreach (var item in tmpProperties)
                    {
                        retVal.Add(item.Name, item.GetValue(obj));
                    }
                }
            }
            return retVal;
        }
    }
}
