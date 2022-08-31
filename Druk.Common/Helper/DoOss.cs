using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Druk.Common
{
    /// <summary>
    /// 阿里云OSS操作
    /// </summary>
    public class DoOss
    {
        /// <summary>
        /// //图档的远程域名 
        /// </summary>
        public string ossDomain = string.Empty;

        /// <summary>
        /// AcessKeyId
        /// </summary>
        private string ossAcessKeyId = string.Empty;

        /// <summary>
        /// AcessKeySecret
        /// </summary>
        private string ossAcessKeySecret = string.Empty;

        /// <summary>
        /// Endpoint
        /// </summary>
        private string ossEndpoint = string.Empty;

        /// <summary>
        /// Bucket
        /// </summary>
        private string ossBucket = string.Empty;


        public DoOss()
        {

            //ossAcessKeyId = CacheSettings.Upload.OssAcessKeyId ?? "LTAInPV5k466kdvH";
            //ossAcessKeySecret = CacheSettings.Upload.OssAcessKeySecret ?? "qqYQQxNFCIf2i1nSAfqo5MswYRkrJS";
            //ossEndpoint = CacheSettings.Upload.OssEndpoint ?? "oss-cn-beijing.aliyuncs.com";
            //ossBucket = CacheSettings.Upload.OssBucket ?? "cnliuye213";
            //ossDomain = CacheSettings.Upload.Domain ?? "https://cnliuye213.oss-cn-beijing.aliyuncs.com";
        }

        public OssClient GetClient()
        {
            return new OssClient(ossEndpoint, ossAcessKeyId, ossAcessKeySecret);
        }


        //public string PresignedUrl(string file) {
        //    var client = GetClient();
        //    client.GeneratePresignedUri()
        //}


        /// <summary>
        /// 上传一个图片
        /// </summary>
        /// <param name="base64Code">图片经过base64加密后的结果</param>
        /// <param name="fileName">文件名,例如:Emplyoee/dzzBack.jpg</param>
        public bool PushImg(string base64Code, string fileName)
        {
            try
            {
                var client = GetClient();

                MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64Code));
                return client.PutObject(ossBucket, fileName, stream).HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception)
            { }
            return false;
        }

        /// <summary>
        /// 上传一个图片
        /// </summary>
        /// <param name="stream">图片经过stream的结果</param>
        /// <param name="fileName">文件名,例如:Emplyoee/dzzBack.jpg</param>
        public (bool result, string msg) PushImg(Stream stream, string fileName)
        {
            try
            {
                var client = GetClient();
                var result = client.PutObject(ossBucket, fileName, stream);

                return (result.HttpStatusCode == System.Net.HttpStatusCode.OK, result.ToJson());
            }
            catch (Exception ex)
            {
                return (false, ex.ToJson());
            }
        }

        /// <summary>
        /// 上传一个文件
        /// </summary>
        /// <param name="filebyte">图片字节 </param>
        /// <param name="fileName">文件名,例如:Emplyoee/dzzBack.jpg</param>
        public bool PushFile(byte[] filebyte, string fileName)
        {
            try
            {
                var client = GetClient();
                MemoryStream stream = new MemoryStream(filebyte, 0, filebyte.Length);
                return client.PutObject(ossBucket, fileName, stream).HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception)
            { }
            return false;
        }

        /// <summary>
        /// 上传一个文件
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="fileName">文件名,例如:Emplyoee/dzzBack.jpg</param>
        public bool PushFile(Stream stream, string fileName)
        {
            try
            {
                var client = GetClient();

                return client.PutObject(ossBucket, fileName, stream).HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception)
            { }
            return false;
        }

        /// <summary>
        /// 获取鉴权后的URL,文件过期时间默认设置为100年
        /// </summary>
        /// <param name="fileName">文件名,例如:Emplyoee/dzzBack.jpg</param>
        /// <returns></returns>
        public string GetFileUrl(string fileName)
        {
            var client = GetClient();
            var key = fileName;
            var req = new GeneratePresignedUriRequest(ossBucket, key, SignHttpMethod.Get)
            {
                Expiration = DateTime.Now.AddYears(100)
            };
            return client.GeneratePresignedUri(req).ToString();
        }

        /// <summary>
        /// 获取鉴权后的URL
        /// </summary>
        /// <param name="fileName">文件名,例如:Emplyoee/dzzBack.jpg</param>
        /// <param name="expiration">URL有效日期,例如:DateTime.Now.AddHours(1) </param>
        /// <returns></returns>
        public string GetFileUrl(string fileName, DateTime expiration)
        {
            var client = GetClient();
            var key = fileName;
            var req = new GeneratePresignedUriRequest(ossBucket, key, SignHttpMethod.Get)
            {
                Expiration = expiration
            };
            return client.GeneratePresignedUri(req).ToString();
        }
    }
}
