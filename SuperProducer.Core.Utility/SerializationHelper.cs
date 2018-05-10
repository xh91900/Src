using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace SuperProducer.Core.Utility
{
    public enum SerializationType
    {
        Xml,
        Json,
        DataContract,
        Binary
    }

    public class SerializationHelper
    {
        #region "XmlSerializer"

        /// <summary>
        /// 使用标准的XmlSerializer序列化对象到文件[不能序列化IDictionary接口]
        /// </summary>
        public static void XmlSerializeToFile(object obj, string fileName)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        /// <summary>
        /// 使用标准的XmlSerializer反序列化文件到对象[不能序列化IDictionary接口]
        /// </summary>
        public static object XmlDeserializeFromFile(Type type, string fileName)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(fs);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        /// <summary>
        /// 使用标准的XmlSerializer序列化对象到Xml字符串
        /// </summary>
        public static string XmlSerialize(object obj)
        {
            string retVal = string.Empty;
            if (obj != null)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, obj);
                    retVal = writer.ToString();
                }
            }
            return retVal;
        }

        /// <summary>
        /// 使用标准的XmlSerializer序列化对象到Xml字符串
        /// </summary>
        public static string XmlSerialize(object obj, Encoding encode)
        {
            string retVal = string.Empty;
            if (obj != null)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(stream, encode))
                    {
                        XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
                        xsn.Add(string.Empty, string.Empty);
                        serializer.Serialize(writer, obj, xsn);
                        retVal = encode.GetString(stream.ToArray());
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// 使用标准的XmlSerializer序列化对象到Xml字符串
        /// </summary>
        public static string Serialize(object obj, bool omitXmlDeclaration = false, bool omitNamespaces = true, Encoding encode = null)
        {
            try
            {
                if (obj != null)
                {
                    if (encode == null)
                        encode = InternalConstant.DefaultEncode;

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.OmitXmlDeclaration = omitXmlDeclaration;
                    settings.Encoding = encode;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (XmlWriter writer = XmlWriter.Create(stream, settings))
                        {
                            XmlSerializer serializer = new XmlSerializer(obj.GetType());
                            if (omitNamespaces)
                            {
                                XmlSerializerNamespaces names = new XmlSerializerNamespaces();
                                names.Add(string.Empty, string.Empty);
                                serializer.Serialize(writer, obj, names);
                            }
                            else
                            {
                                serializer.Serialize(writer, obj);
                            }
                        }
                        return encode.GetString(stream.ToArray());
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        /// <summary>
        /// 使用标准的XmlSerializer反序列化xml字符串到对象
        /// </summary>
        public static object XmlDeserialize(Type type, string xml)
        {
            object retVal = null;
            if (!string.IsNullOrEmpty(xml))
            {
                XmlSerializer serializer = new XmlSerializer(type);
                using (StringReader writer = new StringReader(xml))
                {
                    retVal = serializer.Deserialize(writer);
                }
            }
            return retVal;
        }

        /// <summary>
        /// 使用标准的XmlSerializer反序列化xml字符串到对象
        /// </summary>
        public static T XmlDeserialize<T>(string xml)
        {
            try
            {
                if (!string.IsNullOrEmpty(xml))
                {
                    RemoveInvalidXmlCharacter(ref xml);

                    using (StringReader reader = new StringReader(xml))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        return (T)serializer.Deserialize(reader);
                    }
                }
            }
            catch { }
            return default(T);
        }

        /// <summary>
        /// 增加xml声明[支持1.0声明]
        /// </summary>
        public static string AppendXmlDeclaration(string xml)
        {
            if (!string.IsNullOrEmpty(xml) && xml.Contains("<") && xml.Contains(">"))
            {
                string xmlDeclaratio_v1 = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";

                StringBuilder builder = new StringBuilder();
                if (!xml.ToLower().StartsWith(xmlDeclaratio_v1.Substring(0, 18)))
                {
                    builder.Append(xmlDeclaratio_v1);
                    builder.Append(xml);
                }

                return builder.ToString();
            }
            return xml;
        }

        /// <summary>
        /// 去除无效的低位字符
        /// </summary>
        public static void RemoveInvalidXmlCharacter(ref string xml)
        {
            if (!string.IsNullOrWhiteSpace(xml))
            {
                xml = Regex.Replace(xml, "[\\x00-\\x08\\x0b-\\x0c\\x0e-\\x1f]", string.Empty);
            }
        }

        #endregion

        #region "JsonSerialize"

        /// <summary>
        /// 使用标准的DataContractJsonSerializer序列化对象到Json字符串
        /// </summary>
        public static string JsonSerialize(object obj)
        {
            return JsonSerialize(obj, InternalConstant.DefaultEncode);
        }

        /// <summary>
        /// 使用标准的DataContractJsonSerializer序列化对象到json字符串
        /// </summary>
        public static string JsonSerialize(object obj, Encoding encoding)
        {
            string retVal = string.Empty;
            if (obj != null)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, obj);
                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        retVal = reader.ReadToEnd();
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// 使用标准的DataContractJsonSerializer反序列化标准的json字符串到对象
        /// </summary>
        public static T JsonDeserialize<T>(string json)
        {
            return JsonDeserialize<T>(json, InternalConstant.DefaultEncode);
        }

        /// <summary>
        /// 使用标准的DataContractJsonSerializer反序列化标准的json字符串到对象
        /// </summary>
        public static T JsonDeserialize<T>(string json, Encoding encode)
        {
            return (T)JsonDeserialize(typeof(T), json, encode);
        }

        /// <summary>
        /// 使用标准的DataContractJsonSerializer反序列化标准的json字符串到对象
        /// </summary>
        public static object JsonDeserialize(Type type, string json)
        {
            return JsonDeserialize(type, json, InternalConstant.DefaultEncode);
        }

        /// <summary>
        /// 使用标准的DataContractJsonSerializer反序列化标准的json字符串到对象
        /// </summary>
        public static object JsonDeserialize(Type type, string json, Encoding encode)
        {
            object retVal = null;
            if (!string.IsNullOrEmpty(json))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
                using (MemoryStream stream = new MemoryStream(encode.GetBytes(json.ToCharArray())))
                {
                    retVal = serializer.ReadObject(stream);
                }
            }
            return retVal;
        }

        #endregion

        #region "JavascriptSerialize"

        /// <summary>
        /// 使用标准的JavaScriptSerializer序列化对象到Json字符串
        /// </summary>
        public static string JavascriptSerialize(object obj)
        {
            string retVal = string.Empty;
            if (obj != null)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                retVal = jss.Serialize(obj);
            }
            return retVal;
        }

        /// <summary>
        /// 使用标准的JavaScriptSerializer反序列化标准的json字符串到对象
        /// </summary>
        public static T JavascriptDeserialize<T>(string json)
        {
            var retVal = default(T);
            if (!string.IsNullOrEmpty(json))
            {
                retVal = new JavaScriptSerializer().Deserialize<T>(json);
            }
            return retVal;
        }

        /// <summary>
        /// 使用标准的JavaScriptSerializer反序列化标准的json字符串到对象
        /// </summary>
        public static object JavascriptDeserialize(string json)
        {
            object retVal = null;
            if (!string.IsNullOrEmpty(json))
            {
                retVal = new JavaScriptSerializer().DeserializeObject(json);
            }
            return retVal;
        }

        #endregion

        #region "Newtonsoft Json"

        /// <summary>
        /// Newtonsoft序列化
        /// </summary>
        public static string Newtonsoft_Serialize(object value)
        {
            string retVal = string.Empty;
            try
            {
                if (value != null)
                    retVal = JsonConvert.SerializeObject(value);
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// Newtonsoft序列化
        /// </summary>
        public static string Newtonsoft_Serialize(object value, JsonSerializerSettings setting)
        {
            string retVal = string.Empty;
            try
            {
                if (value != null)
                    retVal = JsonConvert.SerializeObject(value, setting);
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// Newtonsoft反序列化
        /// </summary>
        public static T Newtonsoft_Deserialize<T>(string json) where T : new()
        {
            T retVal = new T();
            try
            {
                if (!string.IsNullOrWhiteSpace(json))
                    retVal = JsonConvert.DeserializeObject<T>(json);
            }
            catch { }
            return retVal;
        }

        #endregion
    }
}
