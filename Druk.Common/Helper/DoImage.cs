using System;
using System.Collections.Generic; 
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Imaging;
using System.IO;
using System.Text;

namespace Druk.Common
{
    public class DoImage
    {
        #region //保存图片

        /// <summary>
        ///  小图后缀
        /// </summary>
        public static string ImageThumbnailSuffix { get { return "_small.jpg"; } }
        /// <summary>
        ///  上传服务器的根目录
        /// </summary>
        public static string UploadDirectory { get { return "../image.qianlan.companycn.net/"; } }

        /// <summary>
        ///  上传模式：1、本地文件服务器 2、阿里OSS
        /// </summary>
        public static int UploadType { get { return 1; } }

        #region //保存图片  流对象



        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="filePath">图片路径（包含图片文件名不包含根目录）</param>
        /// <param name="fileName">图片文件名</param>
        /// <param name="stream">图片流</param>
        /// <returns></returns>
        public static (bool result, string path) SaveImage(string filePath, Stream stream, int smallWidth = 0, int smallHeight = 0)
        {
            var msg = "图片上传到文件服务器失败";
            if (UploadType == ComEnum.UploadMode.本地文件服务器.GetHashCode())
            {
                var picUrl = DoPath.GetFullPath(UploadDirectory + filePath);
                var file = new FileInfo(picUrl);
                if (!file.Directory.Exists) { file.Directory.Create(); }
                if (file.Exists) { file.Delete(); }
                if (stream != null)
                {
                    try
                    {
                        Bitmap bitmap = new Bitmap(stream);

                        bitmap.Save(picUrl);

                    }
                    catch (Exception ex)
                    {
                        msg = "图片上传到文件服务器失败：" + ex.Message;
                    }
                }

                if (System.IO.File.Exists(picUrl))
                {
                    #region 生成缩略图

                    try
                    {
                        if (smallWidth > 0 && smallHeight > 0)
                        {
                            var ThumbnailImageFilePath = filePath + ImageThumbnailSuffix;

                            DoImage.MakeThumbnailImage(DoPath.GetFullPath(UploadDirectory + filePath), UploadDirectory + ThumbnailImageFilePath, smallWidth, smallHeight);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    #endregion

                    return (true, filePath);
                }

            }
            else if (UploadType == ComEnum.UploadMode.阿里OSS.GetHashCode())//浅蓝使用oss图档系统
            {
                Common.DoOss oss = new DoOss();
                string ossFile = UploadDirectory + filePath;
                var result = oss.PushImg(stream, ossFile);
                if (result.result)
                {
                    return (true, ossFile);
                }
                else
                {
                    msg = "图片上传到OSS服务器失败：" + result.msg;
                }

            }
            return (false, msg);

        }
        #endregion

        #region 生成缩列图
        /// <summary>
        /// 制作缩略图
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="newFileName">新图路径</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight)
        {
            //2012-02-05修改过，支持替换
            byte[] imageBytes = File.ReadAllBytes(fileName);
            Image img = Image.FromStream(new System.IO.MemoryStream(imageBytes));
            MakeThumbnailImage(img, newFileName, maxWidth, maxHeight);
        }
        /// <summary>
		/// 制作缩略图
		/// </summary>
		/// <param name="original">图片对象</param>
		/// <param name="newFileName">新图路径</param>
		/// <param name="maxWidth">最大宽度</param>
		/// <param name="maxHeight">最大高度</param>
        public static void MakeThumbnailImage(Image original, string newFileName, int maxWidth, int maxHeight)
        {
            Size _newSize = ResizeImage(original.Width, original.Height, maxWidth, maxHeight);

            using (Image displayImage = new Bitmap(original, _newSize))
            {
                try
                {
                    displayImage.Save(newFileName, original.RawFormat);
                }
                finally
                {
                    original.Dispose();
                }
            }
        }

        /// <summary>
        /// 计算新尺寸
        /// </summary>
        /// <param name="width">原始宽度</param>
        /// <param name="height">原始高度</param>
        /// <param name="maxWidth">最大新宽度</param>
        /// <param name="maxHeight">最大新高度</param>
        /// <returns></returns>
        private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
        {
            //此次2012-02-05修改过=================
            if (maxWidth <= 0)
                maxWidth = width;
            if (maxHeight <= 0)
                maxHeight = height;
            //以上2012-02-05修改过=================
            decimal MAX_WIDTH = (decimal)maxWidth;
            decimal MAX_HEIGHT = (decimal)maxHeight;
            decimal ASPECT_RATIO = MAX_WIDTH / MAX_HEIGHT;

            int newWidth, newHeight;
            decimal originalWidth = (decimal)width;
            decimal originalHeight = (decimal)height;

            if (originalWidth > MAX_WIDTH || originalHeight > MAX_HEIGHT)
            {
                decimal factor;
                // determine the largest factor 
                if (originalWidth / originalHeight > ASPECT_RATIO)
                {
                    factor = originalWidth / MAX_WIDTH;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
                else
                {
                    factor = originalHeight / MAX_HEIGHT;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }
            return new Size(newWidth, newHeight);
        }

        #endregion

        #region //保存图片 Base64字符串
        /// <summary>
        /// 保存图片 Base64字符串
        /// </summary>
        /// <param name="picUrl">图片路径（包含图片文件名不包含根目录）</param>
        /// <param name="Base64">Base64字符串</param>
        /// <returns></returns>
        public static bool SaveImage(string picUrl, string Base64)
        {
            var bytes = Convert.FromBase64String(Base64);
            return SaveImage(picUrl, new MemoryStream(bytes)).result;
        }
        #endregion

        #region //保存图片 二进制流

        /// <summary>
        /// 保存图片 二进制流
        /// </summary>
        /// <param name="picUrl">图片路径（包含图片文件名不包含根目录）</param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool SaveImage(string picUrl, byte[] bytes)
        {
            return SaveImage(picUrl, new MemoryStream(bytes)).result;
        }
        #endregion


        #endregion

        #region //合并图片..一前一后

        /// <summary>     
        /// 调用此函数后使此两种图片合并
        /// 类似相册，有个背景图，中间贴自己的目标图片     
        /// </summary>     
        /// <param name="imgBack">粘贴的源图片</param>
        /// <param name="destImg">粘贴的目标图片</param>
        /// <param name="logoImageHeight">Logo图片要求高度</param>
        /// <param name="logoImageWeight">Logo图片要求宽度</param>
        public static Image CombinImage(string BackImagePath, string LogoImagePath, int logoImageHeight, int logoImageWeight)
        {
            Image backImage = Image.FromFile(BackImagePath);
            Image logoImage = Image.FromFile(LogoImagePath);

            return CombinImage(backImage, logoImage, logoImageHeight, logoImageWeight);
        }


        /// <summary>
        /// 合并图片
        /// </summary>
        /// <param name="imgBack">原始图片</param>
        /// <param name="imgLogo">Logo图片</param>
        /// <param name="logoImageHeight">Logo图片要求高度</param>
        /// <param name="logoImageWeight">Logo图片要求宽度</param>
        /// <returns></returns>
        public static Image CombinImage(Image imgBack, Image imgLogo, int logoImageHeight, int logoImageWeight)
        {
            //规整Logo图片的大小
            imgLogo = KiResizeImage(imgLogo, logoImageWeight, logoImageHeight, 0);

            Graphics g = Graphics.FromImage(imgBack);

            g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);      

            //g.FillRectangle(Brushes.White, imgBack.Width / 2 - img.Width / 2 - 1, imgBack.Width / 2 - img.Width / 2 - 1,1,1);//相片四周刷一层黑色边框     

            //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);     

            g.DrawImage(imgLogo, imgBack.Width / 2 - imgLogo.Width / 2, imgBack.Width / 2 - imgLogo.Width / 2, imgLogo.Width, imgLogo.Height);
            GC.Collect();
            return imgBack;
        }

        #endregion

        #region //图片转换为流对象 byte[] Base64String

        #region //根据路径将图片转换为 byte[]
        /// <summary>
        /// 根据路径将图片转换为 byte[]
        /// </summary>
        /// <param name="PicUrl">图片路径</param>
        /// <returns></returns>
        public static byte[] ImageToByte(string PicUrl)
        {
            PicUrl = DoPath.GetFullPath(PicUrl);
            if (File.Exists(PicUrl))
            {
                try
                {
                    System.IO.MemoryStream m = new System.IO.MemoryStream();
                    Bitmap bp = new Bitmap(PicUrl);
                    bp.Save(m, System.DrawingCore.Imaging.ImageFormat.Gif);
                    byte[] b = m.GetBuffer();
                    return b;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return null;
        }
        #endregion

        #region //根据路径将图片转换为 byte[]
        /// <summary>
        /// 根据路径将图片转换为 byte[]
        /// </summary>
        /// <param name="PicUrl">图片路径</param>
        /// <returns></returns>
        public static byte[] ImageToByteJpg(string PicUrl)
        {
            PicUrl = DoPath.GetFullPath(PicUrl);
            if (File.Exists(PicUrl))
            {
                try
                {
                    using (System.IO.MemoryStream m = new System.IO.MemoryStream())
                    {

                        Bitmap bp = new Bitmap(PicUrl);
                        bp.Save(m, System.DrawingCore.Imaging.ImageFormat.Jpeg);
                        byte[] b = m.GetBuffer();
                        return b;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return null;
        }
        #endregion

        #region //根据路径将图片转换为 Base64位字符串
        /// <summary>
        /// 根据路径将图片转换为 Base64位字符串
        /// </summary>
        /// <param name="PicUrl">图片路径</param>
        /// <returns></returns>
        public static string ImageToByteJpgString(string PicUrl)
        {
            var bytes = ImageToByteJpg(PicUrl);
            if (bytes != null)
            {
                return Convert.ToBase64String(bytes);
            }
            return null;
        }
        #endregion



        #region //根据路径将图片转换为 Base64位字符串
        /// <summary>
        /// 根据路径将图片转换为 Base64位字符串
        /// </summary>
        /// <param name="PicUrl">图片路径</param>
        /// <returns></returns>
        public static string ImageToByteString(string PicUrl)
        {
            var bytes = ImageToByte(PicUrl);
            if (bytes != null)
            {
                return Convert.ToBase64String(bytes);
            }
            return null;
        }
        #endregion

        #region //根据路径将图片转换为Stream流对象
        /// <summary>
        /// 根据路径将图片转换为 Base64位字符串
        /// </summary>
        /// <param name="PicUrl">图片路径</param>
        /// <returns></returns>
        public static Stream ImageToStream(string PicUrl)
        {
            var bytes = ImageToByte(PicUrl);
            if (bytes != null)
            {
                return new MemoryStream(bytes);
            }
            return null;
        }
        #endregion

        #endregion

        #region //改变图片大小
        /// <summary>     
        /// 改变图片大小
        /// </summary>     
        /// <param name="bmp">原始Bitmap</param>     
        /// <param name="newW">新的宽度</param>     
        /// <param name="newH">新的高度</param>     
        /// <param name="Mode">保留着，暂时未用</param>     
        /// <returns>处理以后的图片</returns>     
        public static Image KiResizeImage(Image bmp, int newW, int newH, int Mode)
        {
            try
            {
                Image b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量     
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        #endregion

        #region //Stream <==> Byte[]

        #region //将 Stream 转成 byte[] 
        /// <summary> 
        /// 将 Stream 转成 byte[] 
        /// </summary> 
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        #endregion

        #region //将 byte[] 转成 Stream 
        /// <summary> 
        /// 将 byte[] 转成 Stream 
        /// </summary> 
        public Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
        #endregion
        #endregion


        /// <summary>  
        /// 该方法是将生成的随机数写入图像文件  
        /// </summary>  
        /// <param name="code">code是一个随机数</param>
        /// <param name="numbers">生成位数（默认4位）</param>  
        public static MemoryStream CreateImg(out string code, int numbers = 4)
        {
            code = RndNum(numbers);
            Bitmap Img = null;
            Graphics g = null;
            MemoryStream ms = null;
            Random random = new Random();
            //验证码颜色集合  
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };

            //验证码字体集合
            string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };


            //定义图像的大小，生成图像的实例  
            Img = new Bitmap((int)code.Length * 18, 32);

            g = Graphics.FromImage(Img);//从Img对象生成新的Graphics对象    

            g.Clear(Color.White);//背景设为白色  

            //在随机位置画背景点  
            for (int i = 0; i < 100; i++)
            {
                int x = random.Next(Img.Width);
                int y = random.Next(Img.Height);
                g.DrawRectangle(new Pen(Color.LightGray, 0), x, y, 1, 1);
            }
            //验证码绘制在g中  
            for (int i = 0; i < code.Length; i++)
            {
                int cindex = random.Next(7);//随机颜色索引值  
                int findex = random.Next(5);//随机字体索引值  
                Font f = new Font(fonts[findex], 15, FontStyle.Bold);//字体  
                Brush b = new SolidBrush(c[cindex]);//颜色  
                int ii = 4;
                if ((i + 1) % 2 == 0)//控制验证码不在同一高度  
                {
                    ii = 2;
                }
                g.DrawString(code.Substring(i, 1), f, b, 3 + (i * 12), ii);//绘制一个验证字符  
            }
            ms = new MemoryStream();//生成内存流对象  
            try
            {
                Img.Save(ms, ImageFormat.Png);//将此图像以Png图像文件的格式保存到流中  
            }
            finally
            {
                //回收资源  
                g.Dispose();
                Img.Dispose();

            }
            return ms;

        }

        private static string RndNum(int numbers)
        {
            char[] character = { '2', '3', '4', '5', '6', '8', '9', 'a', 'b', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'x', 'y', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            string code = string.Empty;//产生的随机数  

            Random rnd = new Random();
            //采用一个简单的算法以保证生成随机数的不同  
            for (int i = 0; i < 4; i++)
            {
                code += character[rnd.Next(character.Length)];
            }
            return code;
        } 
    }
}
