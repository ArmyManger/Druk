using Druk.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Druk.Handle.Util
{
    public class FileConfig
    {
        #region //文件/图片 上传


        /// <summary>
        /// 图片访问域名,或者文件下载的域名
        /// </summary>
        public string FileSite_Domain { get { return (DataCache.Sys_Params.GetValue("FileSite.Domain") ?? "").ToString(); } }


        /// <summary>
        /// Excel上传方式  目前可选值ftp local ftps
        /// </summary>
        public string UploadType_Excel { get { return (DataCache.Sys_Params.GetValue("FileSite.Excel.UpLoad.Type") ?? "OSS").ToString(); } }

        /// <summary>
        /// 图片上传方式  目前可选值ftp local ftps  oss
        /// </summary>
        public string UploadType_Image { get { return (DataCache.Sys_Params.GetValue("FileSite.UpLoad.Type") ?? "OSS").ToString(); } }


        #endregion

        #region //本地存储

        /// <summary>
        /// 本地路径,如果为空,会使用FTP
        /// </summary>
        public string FileSite_Upload_LocalPath { get { return (DataCache.Sys_Params.GetValue("FileSite.UpLoad.LocalPath") ?? "").ToString(); } }
        #endregion

        #region //FTP相关
        /// <summary>
        /// 文件站上传ftp地址
        /// </summary>
        public string Ftps_AddressIp { get { return (DataCache.Sys_Params.GetValue("FileSite.UpLoad.Ftp.AddressIP") ?? "").ToString(); } }
        /// <summary>
        /// 文件站上传ftp端口
        /// </summary>
        public string Ftps_Port { get { return (DataCache.Sys_Params.GetValue("FileSite.UpLoad.Ftp.Port") ?? "990").ToString(); } }
        /// <summary>
        /// 文件站上传ftp用户
        /// </summary>
        public string Ftps_User { get { return (DataCache.Sys_Params.GetValue("FileSite.UpLoad.Ftp.User") ?? "").ToString(); } }
        /// <summary>
        /// 文件站上传ftp密码
        /// </summary>
        public string Ftps_Password { get { return (DataCache.Sys_Params.GetValue("FileSite.UpLoad.Ftp.Password") ?? "").ToString(); } }
        #endregion

        #region //OSS相关

        /// <summary>
        /// OSS存储的AppID
        /// </summary>
        public string OSS_AppID { get { return (DataCache.Sys_Params.GetValue("FileSite.UpLoad.OSS.AppID") ?? "").ToString(); } }

        /// <summary>
        /// OSS存储的AppSecret
        /// </summary>
        public string OSS_AppSecret { get { return (DataCache.Sys_Params.GetValue("FileSite.UpLoad.OSS.AppSecret") ?? "").ToString(); } }

        /// <summary>
        /// OSS存储的Bucket
        /// </summary>
        public string OSS_Bucket { get { return (DataCache.Sys_Params.GetValue("FileSite.UpLoad.OSS.Bucket") ?? "").ToString(); } }

        /// <summary>
        /// OSS存储的EndPoint
        /// </summary>
        public string OSS_EndPoint { get { return (DataCache.Sys_Params.GetValue("FileSite.UpLoad.OSS.EndPoint") ?? "oss-cn-shanghai.aliyuncs.com").ToString(); } }
        #endregion

        #region BLOB
        /// <summary>
        /// Blob连接字符串
        /// </summary>
        public string BlobPath { get { return DataCache.Sys_Params.GetValue("Blob.String"); } }
        /// <summary>
        /// blob存储文件地址
        /// </summary>
        public static string BlobFileURL { get { return DataCache.Sys_Params.GetValue("Blob.FileURL") ?? ""; } }
        /// <summary>
        /// Blob 容器名称
        /// </summary>
        public string BlobContainerName { get { return DataCache.Sys_Params.GetValue("Blob.ContainerName") ?? ""; } }
        #endregion

        #region //图片及Excel存放路径
        /// <summary>
        /// 本站图片上传路径
        /// </summary>
        public string Fixed_Path_Image { get { return (DataCache.Sys_Params.GetValue("FileSite.UpLoad.Image") ?? "/upload/image/").ToString(); } }
        /// <summary>
        /// 本站Excel文件上传路径
        /// </summary>
        public string Fixed_Path_Excel { get { return (DataCache.Sys_Params.GetValue("FileSite.UpLoad.Excel") ?? "/upload/excel/").ToString(); } }
        #endregion

        #region//上传文件大小限制(KB)
        /// <summary>
        /// 后台图片文件大小限制(KB)
        /// </summary>
        public int Size_Limit_Image { get { return (DataCache.Sys_Params.GetValue("FileSite.Upload.ImageSize") ?? "1024").ToInt(); } }
        /// <summary>
        /// Excel文件大小限制(KB)
        /// </summary>
        public int Size_Limit_Excel { get { return (DataCache.Sys_Params.GetValue("FileSite.Upload.FrontSize") ?? "2048").ToInt(); } }
        #endregion
    }
}
