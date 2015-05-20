using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace SDNUOJ.Utilities.Drawing
{
    /// <summary>
    /// 验证码类
    /// </summary>
    public class CheckCode
    {
        #region 字段
        private String _code;
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前验证码字符串
        /// </summary>
        public String CodeText
        {
            get { return this._code; }
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 初始化新的验证码
        /// </summary>
        public CheckCode()
        {
            this.Generate();
        }

        /// <summary>
        /// 生成新的验证码
        /// </summary>
        public void Generate()
        {
            String list = "123456789abcdefghijklmnpqrstuvwxyz";
            String code = "";
            Random r = new Random();

            for (Int32 i = 0; i < 4; i++)
            {
                code += list[r.Next(list.Length)];
            }

            this._code = code;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取验证码数据
        /// </summary>
        /// <returns>验证码数据</returns>
        public Byte[] GetBitmapData()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Bitmap bitmap = this.GetBitmap();
                bitmap.Save(ms, ImageFormat.Jpeg);

                return ms.ToArray();
            }
        }
        
        /// <summary>
        /// 获取验证码图像
        /// </summary>
        /// <returns>验证码图像</returns>
        public Bitmap GetBitmap()
        {
            Bitmap image = new Bitmap(120, 50);
            Graphics pic = Graphics.FromImage(image);

            try
            {
                Random random = new Random();
                Brush fc = new SolidBrush(new Color[] {
                        Color.FromArgb(255, 44, 89, 127),
                        Color.FromArgb(255, 81, 126, 19),
                        Color.FromArgb(255, 183, 68, 5)
                    }[random.Next(3)]);
                String ff = new String[] { "Arial", "Cambria", "Georgia", "Calibri" }[random.Next(4)];
                Font font = new Font(ff, 30, FontStyle.Bold);

                pic.SmoothingMode = SmoothingMode.AntiAlias;
                pic.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                pic.Clear(Color.FromArgb(255, 248, 248, 248));

                //绘制验证码
                Int32 last = random.Next(3) + 3;

                for (Int32 i = 0; i < this._code.Length; i++)
                {
                    Int32 ro = random.Next(50) * (random.Next(3) - 1);
                    Single sc = 0.9f + (Single)(random.NextDouble() / 5.0) * (random.Next(3) - 1);
                    
                    pic.ResetTransform();
                    pic.TranslateTransform(last + 13, 20);
                    pic.RotateTransform(ro);
                    pic.ScaleTransform(sc, sc);
                    pic.TranslateTransform(-last - 13, -20);
                    pic.Flush();

                    String des = (random.Next(2) == 0 ? Char.ToLower(this._code[i]) : Char.ToUpper(this._code[i])).ToString();
                    pic.DrawString(des, font, fc, last, random.Next(5) + 2);

                    last += image.Width / this._code.Length - last / 5;
                }

                pic.ResetTransform();

                //绘制背景曲线
                for (Int32 i = 1; i <= 4; i++)
                {
                    List<Point> list = new List<Point>();

                    for (Int32 j = 0; j < 4; j++)
                    {
                        list.Add(new Point(j * 40, random.Next(50)));
                    }

                    pic.DrawBeziers(new Pen(fc, random.Next(3)), list.ToArray());
                }
            }
            finally
            {
                pic.Dispose();
            }

            return image;
        }
        #endregion
    }
}