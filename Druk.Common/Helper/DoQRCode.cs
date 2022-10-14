using System;
using System.Collections.Generic; 
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using QRCoder;

namespace Druk.Common
{
    public class DoQRCode
    {
        #region //创建二维码

        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="obj">目标字符串</param>
        /// <param name="filePath">生成的目标位置</param>
        /// <param name="Logo">中间Logo图片的位置</param>
        /// <returns></returns>
        public bool Create(string obj, string filePath, string Logo = "")
        {
            try
            {
                //文件夹不存在就创建  文件存在就删除
                FileInfo file = new FileInfo(DoPath.GetFullPath(filePath));
                if (!file.Directory.Exists) { file.Directory.Create(); }
                if (file.Exists) { file.Delete(); }

                Bitmap logo = null;
                if (!string.IsNullOrEmpty(Logo) && new FileInfo(Logo).Exists) { logo = new Bitmap(DoPath.GetFullPath(Logo)); }


                QRCodeData qrCodeData = new QRCoder.QRCodeGenerator().CreateQrCode(obj, QRCodeGenerator.ECCLevel.Q);
                QRCode qrcode = new QRCode(qrCodeData);

                // qrcode.GetGraphic 方法可参考最下发“补充说明”
                System.DrawingCore.Bitmap qrCodeImage = qrcode.GetGraphic(5, System.DrawingCore.Color.Black, System.DrawingCore.Color.White, logo, 15, 6, false);
                qrCodeImage.Save(file.FullName);

                //刷新文件状态 返回是否存在
                file.Refresh();
                return file.Exists;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            #region //GetGraphic方法参数说明

            /*
            public Bitmap GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, Bitmap icon = null, int iconSizePercent = 15, int iconBorderWidth = 6, bool drawQuietZones = true)            
             
            int pixelsPerModule:生成二维码图片的像素大小 ，我这里设置的是5             
            
            Color darkColor：暗色   一般设置为Color.Black 黑色            
            
            Color lightColor:亮色   一般设置为Color.White  白色            
            
            Bitmap icon :二维码 水印图标 例如：Bitmap icon = new Bitmap(context.Server.MapPath("~/images/zs.png")); 默认为NULL ，加上这个二维码中间会显示一个图标            
             
            int iconSizePercent： 水印图标的大小比例 ，可根据自己的喜好设置             
            
            int iconBorderWidth： 水印图标的边框            
            
            bool drawQuietZones:静止区，位于二维码某一边的空白边界,用来阻止读者获取与正在浏览的二维码无关的信息 即是否绘画二维码的空白边框区域 默认为true
            */
            #endregion
        }
        #endregion
    }
}
