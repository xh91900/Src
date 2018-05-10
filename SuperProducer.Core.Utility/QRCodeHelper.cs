using System.Drawing;
using System.Text;
using ThoughtWorks.QRCode.Codec;

namespace SuperProducer.Core.Utility
{
    public class QRCodeHelper
    {
        public enum ENCODE_MODE
        {
            ALPHA_NUMERIC = 0,
            NUMERIC = 1,
            BYTE = 2
        }

        public enum ERROR_CORRECTION
        {
            L = 0,
            M = 1,
            Q = 2,
            H = 3
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="text">二维码文本</param>
        /// <param name="scale">尺寸</param>
        /// <param name="version">版本</param>
        /// <param name="mode">编码模式</param>
        /// <param name="errorCorrect">错误纠正级别</param>
        /// <returns></returns>
        public static Image GenerateQRCode(string text, int scale = 5, int version = 0, ENCODE_MODE mode = ENCODE_MODE.BYTE, ERROR_CORRECTION errorCorrect = ERROR_CORRECTION.M)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var qrCoder = new QRCodeEncoder();
                    qrCoder.QRCodeEncodeMode = EnumHelper.Parse<QRCodeEncoder.ENCODE_MODE>(mode.ToString());
                    qrCoder.QRCodeScale = 5;
                    qrCoder.QRCodeVersion = 0;
                    qrCoder.QRCodeErrorCorrect = EnumHelper.Parse<QRCodeEncoder.ERROR_CORRECTION>(errorCorrect.ToString());
                    return Image.FromHbitmap(qrCoder.Encode(text, Encoding.GetEncoding("GB2312")).GetHbitmap());
                }
            }
            catch { }
            return null;
        }
    }
}
