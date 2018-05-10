using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace SuperProducer.Core.Utility
{
    public class StreamHelper
    {
        /// <summary>
        /// 读流[目前支持读出:String]
        /// </summary>
        /// <typeparam name="T">将流以什么类型读出</typeparam>
        /// <param name="stream">流</param>
        /// <param name="encode">默认编码</param>
        /// <param name="detectEncodingFromByteOrderMarks">指示是否在文件头查找字节顺序标记</param>
        /// <returns></returns>
        public static T Read<T>(Stream stream, Encoding encode = null, bool detectEncodingFromByteOrderMarks = true)
        {
            if (stream != null && stream.CanRead && stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);

                encode = encode == null ? InternalConstant.DefaultEncode : encode;

                var type = typeof(T);

                if (type == typeof(string))
                {
                    using (StreamReader reader = new StreamReader(stream, encode, detectEncodingFromByteOrderMarks))
                    {
                        return ConvertHelper.ChangeType<T>(reader.ReadToEnd());
                    }
                }
                else if (type == typeof(Image))
                {
                    return ConvertHelper.ChangeType<T>(ImageHelper.FormStream(stream));
                }
                else if (type == typeof(byte[]))
                {
                    return ConvertHelper.ChangeType<T>(GetByteArray(stream));
                }
            }
            return default(T);
        }

        /// <summary>
        /// 图片转流
        /// </summary>
        public static Stream GetStream(Image img, ImageFormat format = null)
        {
            if (img != null && img.Width > 0 && img.Height > 0)
            {
                try
                {
                    if (format == null) format = ImageFormat.Jpeg;
                    var stream = new MemoryStream();
                    img.Save(stream, format);
                    stream.Seek(0, SeekOrigin.Begin);
                    return stream;
                }
                catch { }
            }
            return null;
        }

        /// <summary>
        /// 流转字节数组
        /// </summary>
        public static byte[] GetByteArray(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
    }
}
