using System;
using System.IO;
using System.Text;

namespace SuperProducer.Core.Utility
{
    public class FileHelper
    {
        /// <summary>
        /// 路径类型
        /// </summary>
        internal enum PathType
        {
            WindowPath = 1,
            WebUrl = 2,
        }

        /// <summary>
        /// 获取文本文件指定Key的值[key1=value1\r\nkey2=value2]
        /// </summary>
        public static string GetTXTFileValue(string filePathOrUrl, string key, Encoding encode = null)
        {
            string retVal = string.Empty;
            try
            {
                string tempContent = GetFileContent(filePathOrUrl, encode);
                if (!string.IsNullOrEmpty(tempContent))
                {
                    var tmpContentArray = StringHelper.SplitString(tempContent, "\r\n");
                    foreach (var item in tmpContentArray)
                    {
                        var temp = item.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        if (temp.Length == 2 && temp[0].Trim() == key)
                        {
                            retVal = temp[1];
                            break;
                        }
                    }
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 读取文件内容[绝对/相对/网址]
        /// </summary>
        public static string GetFileContent(string filePathOrUrl, Encoding encode = null)
        {
            string retVal = string.Empty;
            try
            {
                var stream = GetFileStream(filePathOrUrl);
                if (stream != null)
                {
                    using (stream)
                    {
                        retVal = StreamHelper.Read<string>(stream, encode);
                    }
                }
            }
            catch { }
            return retVal;
        }

        /// <summary>
        /// 获取文件的只读流
        /// </summary>
        public static Stream GetFileStream(string filePathOrUrl)
        {
            Stream retVal = null;
            try
            {
                if (!string.IsNullOrEmpty(filePathOrUrl))
                {
                    PathType pathType = 0;

                    var targetPath = filePathOrUrl;

                    if (RegExpHelper.IsUrl(filePathOrUrl))
                    {
                        pathType = PathType.WebUrl;
                        targetPath = filePathOrUrl;
                    }
                    else if (RegExpHelper.IsRelativePath(filePathOrUrl))
                    {
                        pathType = PathType.WindowPath;
                        targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePathOrUrl.Replace("/", @"\").TrimStart('/').TrimStart('\\'));
                    }
                    else if (RegExpHelper.IsPhysicalPath(filePathOrUrl))
                    {
                        pathType = PathType.WindowPath;
                        targetPath = filePathOrUrl;
                    }

                    if (Enum.IsDefined(typeof(PathType), pathType))
                    {
                        switch (pathType)
                        {
                            case PathType.WindowPath:
                                retVal = new FileStream(targetPath, FileMode.Open, FileAccess.Read);
                                break;
                            case PathType.WebUrl:
                                retVal = NetHelper.HttpGetStream(targetPath);
                                break;
                        }
                    }
                }
            }
            catch { }
            return retVal;
        }
    }
}
