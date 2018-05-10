using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace SuperProducer.Core.Utility
{
    /// <summary>
    /// 水印位置
    /// </summary>
    public enum ImagePosition
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 1,
        /// <summary>
        /// 左上
        /// </summary>
        LeftTop = 2,
        /// <summary>
        /// 左下
        /// </summary>
        LeftBottom = 3,
        /// <summary>
        /// 右上
        /// </summary>
        RightTop = 4,
        /// <summary>
        /// 右下
        /// </summary>
        RigthBottom = 5,
        //TopMiddle,     //顶部居中
        //BottomMiddle, //底部居中
        //Center           //中心
    }

    public class ImageHelper
    {
        public static Size FormatToSize(string sizeString)
        {
            var retVal = new Size(-1, -1);
            try
            {
                var tmpItemList = StringHelper.SplitString(sizeString, "*");
                if (tmpItemList.Count == 2)
                {
                    retVal = new Size(ConvertHelper.GetInt(tmpItemList.First(), -1), ConvertHelper.GetInt(tmpItemList.Last(), -1));
                }
            }
            catch { }
            return retVal;
        }

        public static Point FormatToPoint(string pointString)
        {
            var retVal = new Point(-1, -1);
            try
            {
                var tmpItemList = StringHelper.SplitString(pointString, ",");
                if (tmpItemList.Count == 2)
                {
                    retVal = new Point(ConvertHelper.GetInt(tmpItemList.First(), -1), ConvertHelper.GetInt(tmpItemList.Last(), -1));
                }
            }
            catch { }
            return retVal;
        }

        public static Image FromFile(string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (File.Exists(filePath))
                    {
                        return Image.FromFile(filePath);
                    }
                }
            }
            catch { }
            return null;
        }

        public static Image FromUrl(string url, Dictionary<string, string> header = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    if (header == null)
                    {
                        header = InternalConstant.DefaultUserAgentForPC.Where((item) => { if (item.Key == "Firefox") return true; return false; }).ToDictionary(item => item.Key, item => item.Value);
                    }

                    using (var stream = NetHelper.HttpGetStream(url, header))
                    {
                        return FormStream(stream);
                    }
                }
            }
            catch { }
            return null;
        }

        public static Image FormStream(Stream stream)
        {
            if (stream != null && stream.CanRead)
            {
                return Image.FromStream(stream);
            }
            return null;
        }

        public static Image Merge(Image[] imgs, Point[] pts)
        {
            if (imgs != null && pts != null && imgs.Length == pts.Length)
            {
                for (int i = 0; i < imgs.Length; i++)
                {
                    if (imgs[i] == null)
                    {
                        imgs[i] = new Bitmap(1, 1);
                        pts[i] = new Point(0, 0);
                    }
                }

                var ws = new int[imgs.Length];
                var hs = new int[imgs.Length];
                for (int i = 0; i < imgs.Length; i++)
                {
                    ws[i] = imgs[i].Width + pts[i].X;
                    hs[i] = imgs[i].Height + pts[i].Y;
                }

                var bmp = new Bitmap(ws.Max(), hs.Max());
                using (var grap = Graphics.FromImage(bmp))
                {
                    grap.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    for (int i = 0; i < imgs.Length; i++)
                    {
                        grap.DrawImage(imgs[i], new Rectangle(pts[i].X, pts[i].Y, imgs[i].Width, imgs[i].Height), new Rectangle(0, 0, imgs[i].Width, imgs[i].Height), GraphicsUnit.Pixel);
                    }
                }
                return bmp;
            }
            return null;
        }

        public static Image ChangeSize(Image img, Size size)
        {
            try
            {
                var bmp = new Bitmap(size.Width, size.Height);
                using (var grap = Graphics.FromImage(bmp))
                {
                    grap.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    grap.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                }
                return bmp;
            }
            catch { }
            return img;
        }

        public static Image DrawingString(Image img, string writeStr, Point writePos, int writeCount = 1, Font font = null, Brush brush = null)
        {
            try
            {
                if (img != null && !string.IsNullOrEmpty(writeStr) && writePos != null)
                {
                    Font tmpFont = font;
                    Brush tmpBrush = brush;

                    if (tmpFont == null)
                    {
                        var tmpFontFamily = FontFamily.Families.Where(item => item.Name == "方正兰亭黑简体").FirstOrDefault();
                        if (tmpFontFamily == null) tmpFontFamily = FontFamily.Families.FirstOrDefault();
                        tmpFont = new Font(tmpFontFamily, 10);
                    }

                    if (tmpBrush == null)
                    {
                        tmpBrush = new SolidBrush(Color.Black);
                    }

                    if (tmpFont != null && tmpBrush != null)
                    {
                        var bmp = new Bitmap(img.Width, img.Height);
                        using (Graphics grap = Graphics.FromImage(bmp))
                        {
                            grap.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            grap.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));

                            using (tmpFont)
                            {
                                using (tmpBrush)
                                {
                                    for (int i = 0; i < writeCount; i++)
                                    {
                                        grap.DrawString(writeStr, tmpFont, tmpBrush, 5, 5);
                                    }
                                }
                            }
                        }
                        return bmp;
                    }
                }
            }
            catch { }
            return null;
        }
    }
}
