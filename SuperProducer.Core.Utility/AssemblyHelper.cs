using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SuperProducer.Core.Utility
{
    public class AssemblyHelper
    {
        /// <summary>
        /// 获取正在执行的方法名称
        /// </summary>
        public static string GetRunningMethodName()
        {
            string retVal = string.Empty;
            try
            {
                StackTrace trace = new StackTrace(true);
                retVal = trace.GetFrame(1).GetMethod().Name.ToString();
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 获取当前应用程序的根目录
        /// </summary>
        public static string GetBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 获取当前应用程序的BIN目录
        /// </summary>
        public static string GetPrivateBinDirectory()
        {
            var retVal = GetBaseDirectory();
            if (HttpContext.Current == null)
                return retVal;
            else
                return Path.Combine(retVal, "bin");
        }

        /// <summary>
        /// 获取入口程序集[兼容Web和Winform]
        /// </summary>
        public static Assembly GetEntryAssembly()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                return entryAssembly;

            if (HttpContext.Current == null || HttpContext.Current.ApplicationInstance == null)
                return Assembly.GetExecutingAssembly();

            var type = HttpContext.Current.ApplicationInstance.GetType();
            while (type != null && type.Namespace == "ASP")
            {
                type = type.BaseType;
            }
            return type == null ? null : type.Assembly;
        }

        /// <summary>
        /// 根据程序集创建一个类型的实例
        /// </summary>
        public static object GetInstance(Assembly assembly, Type targetType)
        {
            if (assembly != null && targetType != null)
            {
                return GetInstance(assembly, targetType.FullName);
            }
            return null;
        }

        /// <summary>
        /// 根据程序集创建一个类型的实例
        /// </summary>
        public static object GetInstance(Assembly assembly, string typeFullName)
        {
            object retVal = null;
            try
            {
                if (assembly != null && !string.IsNullOrEmpty(typeFullName))
                {
                    retVal = assembly.CreateInstance(typeFullName);
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 根据条件表达式查找程序集中的资源流
        /// </summary>
        public static IList<Stream> GetResourceStream(Assembly assembly, Expression<Func<string, bool>> predicate)
        {
            List<Stream> retVal = new List<Stream>();
            if (assembly != null && predicate != null)
            {
                foreach (string resource in assembly.GetManifestResourceNames())
                {
                    if (predicate.Compile().Invoke(resource))
                    {
                        retVal.Add(assembly.GetManifestResourceStream(resource));
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// 扫描程序根目录下的所有程序集找到继承了某基类的所有子类
        /// </summary>
        public static List<Type> FindTypeByInheritType(Type parentType, string searchPattern = "*.dll")
        {
            var retVal = new List<Type>();
            if (parentType != null && !string.IsNullOrEmpty(searchPattern))
            {
                string binDir = GetBaseDirectory();
                try
                {
                    string[] dllFiles = Directory.GetFiles(binDir, searchPattern, SearchOption.TopDirectoryOnly);
                    foreach (var file in dllFiles)
                    {
                        retVal.AddRange(Assembly.LoadFrom(file).GetLoadableTypes().Where(item => item.BaseType == parentType));
                    }
                }
                catch { }
            }
            return retVal;
        }

        /// <summary>
        /// 扫描程序根目录下的所有程序集找到带有某个Attribute的所有PropertyInfo
        /// </summary>
        public static Dictionary<PropertyInfo, T> FindPropertyByAttribute<T>(string searchPattern = "*.dll") where T : Attribute
        {
            var retVal = new Dictionary<PropertyInfo, T>();
            if (!string.IsNullOrEmpty(searchPattern))
            {
                var targetType = typeof(T);
                string binDir = GetBaseDirectory();
                try
                {
                    string[] dllFiles = Directory.GetFiles(binDir, searchPattern, SearchOption.TopDirectoryOnly);
                    foreach (string file in dllFiles)
                    {
                        foreach (Type type in Assembly.LoadFrom(file).GetLoadableTypes())
                        {
                            foreach (var property in type.GetProperties())
                            {
                                var tempAttrs = property.GetCustomAttributes(targetType, true);
                                if (tempAttrs.Length == 0)
                                    continue;

                                retVal.Add(property, (T)tempAttrs.FirstOrDefault());
                            }
                        }
                    }
                }
                catch { }
            }
            return retVal;
        }

        /// <summary>
        /// 扫描程序根目录下的所有程序集找到所有带有Attribute的类型
        /// </summary>
        public static Dictionary<string, List<T>> FindTypeByAttribute<T>(string searchPattern = "*.dll") where T : Attribute
        {
            var retVal = new Dictionary<string, List<T>>();
            if (!string.IsNullOrEmpty(searchPattern))
            {
                var targetType = typeof(T);
                string binDir = GetBaseDirectory();
                try
                {
                    string[] dllFiles = Directory.GetFiles(binDir, searchPattern, SearchOption.TopDirectoryOnly);
                    foreach (string file in dllFiles)
                    {
                        var assemblyType = Assembly.LoadFrom(file).GetLoadableTypes();
                        if (assemblyType != null)
                        {
                            retVal.Add(assemblyType.FirstOrDefault().AssemblyQualifiedName, new List<T>());

                            foreach (Type type in assemblyType)
                            {
                                var tempAttrs = type.GetCustomAttributes(targetType, true);
                                if (tempAttrs.Length == 0)
                                    continue;

                                foreach (T item in tempAttrs)
                                    retVal.Last().Value.Add(item);
                            }
                        }
                    }
                }
                catch { }
            }
            return retVal;
        }

        /// <summary>
        /// 扫描程序根目录下的所有程序集找到实现了某接口的第一个实例
        /// </summary>
        public static T FindTypeByInterface<T>(string searchPattern = "*.dll") where T : class
        {
            if (!string.IsNullOrEmpty(searchPattern))
            {
                var targetType = typeof(T);
                string binDir = GetBaseDirectory();
                try
                {
                    string[] dllFiles = Directory.GetFiles(binDir, searchPattern, SearchOption.TopDirectoryOnly);
                    foreach (string file in dllFiles)
                    {
                        foreach (Type type in Assembly.LoadFrom(file).GetLoadableTypes())
                        {
                            if (targetType != type && targetType.IsAssignableFrom(type))
                            {
                                return Activator.CreateInstance(type) as T;
                            }
                        }
                    }
                }
                catch { }
            }
            return null;
        }
    }
}
