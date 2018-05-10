using System.Text;
using System.IO;

namespace SuperProducer.Core.Utility
{
    public class LogHelper
    {
        #region "File Log"

        public static void WriteLog(string content)
        {
            WriteLog(content);
        }

        public static void WriteLog(string content, Encoding encode)
        {
            WriteLog(content, encode: encode);
        }

        public static void WriteLog(string content, string fileName = null, Encoding encode = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(content))
                {
                    if (string.IsNullOrEmpty(fileName))
                        fileName = "debug.txt";

                    encode = encode == null ? InternalConstant.DefaultEncode : encode;

                    string filePath = string.Format(@"{0}\{1}", AssemblyHelper.GetBaseDirectory().TrimEnd('\\'), fileName);
                    using (StreamWriter writer = new StreamWriter(filePath, true, encode))
                    {
                        writer.WriteLine(content);
                        writer.Flush();
                    }
                }
            }
            catch { }
        }

        #endregion
    }
}
