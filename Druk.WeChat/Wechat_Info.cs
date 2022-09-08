using Druk.Common;
using Druk.Common.Entity;
using Druk.WeChat.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Druk.WeChat
{
    public static class Wechat_Info
    {
        /// <summary>
        /// 获取第三方平台的信息
        /// </summary>
        /// <param name="appId">小程序的appid</param>
        /// <returns></returns>
        public static JsonResponse<Util.ComponentSimple> GetComponentInfo(string appId)
        {
            if (appId.IsNullOrEmpty()) return null;
            var url = $"{OpenService.ApiUrl.TrimEnd('/')}/api/GetAccessTokenByAppId";
            var result = DoWebRequest.PostAsync(url, new
            {
                UserId = OpenService.UserId,
                userSecret = OpenService.UserSecret,
                appid = appId
            }.ToJson()).ToObjectFromJson<JsonResponse<Util.ComponentSimple>>();
            return result;
        }


        /// <summary>
        /// 获取用户openId
        /// </summary>
        /// <param name="code"></param>
        /// <param name="appid">小程序id</param>
        /// <param name="appSecret">小程序appSecret</param>
        /// <returns></returns>
        public static JObject GetOpeinID(string code, string appid, string appSecret)
        {
            try
            {
                //System.Diagnostics.Stopwatch Timmer = new System.Diagnostics.Stopwatch();
                //Timmer.Start(); //启动计时器
                WeChatTools tools = new WeChatTools();
                var url = String.Format("https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code", appid, appSecret, code);
                if (OpenService.IsStart)
                {
                    var model = GetComponentInfo(appid);
                    if (model == null || model.body == null)
                        return null;
                    url = String.Format("https://api.weixin.qq.com/sns/component/jscode2session?appid={0}&js_code={1}&grant_type=authorization_code&component_appid={2}&component_access_token={3}", appid, code, model.body.componentAppId, model.body.accessToken);
                }
                var Json = Druk.Common.DoWebRequest.SendRequest_Get(url.ToString(), "");

                //Timmer.Stop();
                //var t = Timmer.ElapsedMilliseconds;
                // Log.Info($"调用微信获取openId接口所用时间：{t}毫秒");
                if (!string.IsNullOrEmpty(Json) && Json.Contains("openid"))
                {
                    return JObject.Parse(Json);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); return JObject.Parse(ex.Message); }
            return null;
        }


        /// <summary>
        /// 获取微信用户信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static string GetUserInfo(string openId = "")
        {
            WeChatTools tools = new WeChatTools();
            //获取 access_token
            string strUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + tools.MiniApp_AppId + "&secret=" + tools.MiniApp_AppSecret;
            var Json = Druk.Common.DoWebRequest.SendRequest_Get(strUrl.ToString(), "");

            if (!string.IsNullOrEmpty(Json) && Json.Contains("access_token"))
            {
                var jObject = JObject.Parse(Json);
                var access_token = jObject["access_token"].ToString();

                //获取微信用户信息
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN", access_token, openId);
                var userInfo = Druk.Common.DoWebRequest.SendRequest_Get(url.ToString(), "");
            };

            return "";

        }


        /// <summary>
        /// 获取第三方平台的信息
        /// </summary>
        /// <param name="appId">小程序的appid</param>
        /// <returns></returns>
        public static JsonResponse<Util.AccessTokenSimple> GetAccessToken(string appId)
        {
            if (appId.IsNullOrEmpty()) return null;
            var url = $"{OpenService.ApiUrl.TrimEnd('/')}/api/GetWechatAccessToken";
            var result = DoWebRequest.PostAsync(url, new { UserId = OpenService.UserId, userSecret = OpenService.UserSecret, appid = appId }.ToJson()).ToObjectFromJson<JsonResponse<Util.AccessTokenSimple>>();
            return result;
        }

        /// <summary>
        ///获取AccessToken
        /// </summary>
        /// <param name="MiniApp_AppId"></param>
        /// <param name="MiniApp_AppSecret"></param>
        /// <returns></returns>
        public static (string token, string errorMsg) GetAccessToken(string MiniApp_AppId, string MiniApp_AppSecret)
        {
            try
            {
                var token = string.Empty;

                if (OpenService.IsStart)
                {
                    #region 第三方平台
                    var _token = GetAccessToken(MiniApp_AppId);
                    token = _token.body?.accessToken;
                    #endregion
                }
                else
                {
                    #region 原生小程序
                    var result = Senparc.Weixin.MP.CommonAPIs.CommonApi.GetToken(MiniApp_AppId, MiniApp_AppSecret, "client_credential");
                    if (result.errcode == Senparc.Weixin.ReturnCode.请求成功)
                    {
                        token = result.access_token;
                    }
                    #endregion
                }

                return (token, "");
            }
            catch (Exception ex)
            {
                Druk.Log.Error("获取AccessToken:" + ex.Message);
                return ("", ex.Message);
            }
        }


        #region //获取小程序码


        /// <summary>
        /// /获取小程序码
        /// </summary>
        /// <param name="postData">小程序太阳码需要的参数</param>
        /// <param name="appid">小程序appid</param>
        /// <param name="appSecret">小程序appSecret</param>
        /// <param name="filePath">需要存储的本地文件路径</param>
        /// <returns></returns>
        public static JsonResponse GetMiniAPPCode(string postData, string appid, string appSecret, string filePath = "")
        {
            var result = GetMiniCode(postData, appid, appSecret, filePath);
            if (result.img.Length > 0)
            {
                return ComEnum.Code.操作成功.JsonR(new { img = result.img, url = result.url });
            }
            return ComEnum.Code.操作失败.JsonR();
        }

        /// <summary>
        /// 获取小程序码
        /// </summary>
        /// <param name="postData"></param>
        /// <param name="appid"></param>
        /// <param name="appSecret"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static (byte[] img, string url) GetMiniCode(string postData, string appid, string appSecret, string filePath = "")
        {
            var resultObj = (new byte[0], "");
            if (!string.IsNullOrEmpty(postData))
            {
                //获取accessToken
                var accessToken = Wechat_Info.GetAccessToken(appid, appSecret).token;

                if (!string.IsNullOrEmpty(accessToken))
                {
                    //System.Diagnostics.Stopwatch Timmer = new System.Diagnostics.Stopwatch();
                    //Timmer.Start(); //启动计时器 
                    //请求接口
                    var url = string.Format("https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token={0}", accessToken);
                    var request = (System.Net.HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/json;charset=UTF-8";
                    var payload = System.Text.Encoding.UTF8.GetBytes(postData);
                    request.ContentLength = payload.Length;
                    request.Proxy = null;
                    var writer = request.GetRequestStream();
                    writer.Write(payload, 0, payload.Length);
                    writer.Close();
                    var response = (System.Net.HttpWebResponse)request.GetResponse();
                    var stream = response.GetResponseStream();

                    //拿到图片
                    List<byte> bytes = new List<byte>();
                    int temp = stream.ReadByte();
                    while (temp != -1)
                    {
                        bytes.Add((byte)temp);
                        temp = stream.ReadByte();
                    }
                    var result = bytes.ToArray();

                    //保存图片
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        DoImage.SaveImage(filePath, new MemoryStream(result));
                    }

                    //Timmer.Stop();
                    //var t = Timmer.ElapsedMilliseconds; 
                    //Log.Info($"调用微信生成小程序码接口所用时间：{t}毫秒"); 
                    resultObj.Item1 = result;
                    //resultObj.Item2 = Upload.Domain + Common.DoImage.UploadDirectory + filePath;
                    resultObj.Item2 = Common.DoImage.UploadDirectory + filePath;
                }
            }

            return resultObj;
        }


        /// <summary>
        /// /获取小程序码
        /// </summary>
        /// <param name="qrCode"></param>
        /// <param name="appid"></param>
        /// <param name="appSecret"></param>
        /// <param name="filepath">文件夹+文件名</param>
        /// <returns></returns>
        public static (bool result, string path) GetSmallProgramCode(MiniQrCode qrCode, string appid, string appSecret, string filepath, int type = 1)
        {
            try
            {
                if (qrCode != null)
                {
                    //获取accessToken
                    var accessToken = Wechat_Info.GetAccessToken(appid, appSecret).token;

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        Stream stream = new MemoryStream();
                        //stream.Position = 0;

                        Senparc.Weixin.Entities.WxJsonResult result = null;
                        if (type == 1)
                            result = Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppApi.GetWxaCodeUnlimit(accessToken, stream, qrCode.scene, qrCode.page, qrCode.width, qrCode.auto_color);
                        else
                            result = Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppApi.CreateWxQrCode(accessToken, stream, qrCode.page, qrCode.width);

                        if (result.errcode == Senparc.Weixin.ReturnCode.请求成功)
                        {
                            stream.Position = 0;
                            return DoImage.SaveImage(filepath, stream);

                        }
                    }
                }
                return (false, "获取小程序码失败");
            }
            catch (Exception ex)
            {
                var aaa = ex;
            }

            return (false, "获取小程序码失败");
        }





        /// <summary>
        /// /获取小程序码
        /// </summary>
        /// <param name="qrCode"></param>
        /// <param name="appid"></param>
        /// <param name="appSecret"></param>
        /// <param name="filepath">文件夹+文件名</param>
        /// <returns></returns>
        public static Stream GetSenparcMiniCode(MiniQrCode qrCode, string appid, string appSecret)
        {
            if (qrCode != null)
            {
                //获取accessToken
                var accessToken = Wechat_Info.GetAccessToken(appid, appSecret).token;

                if (!string.IsNullOrEmpty(accessToken))
                {
                    Stream stream = new MemoryStream();
                    var result = Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppApi.GetWxaCodeUnlimit(accessToken, stream, qrCode.scene, qrCode.page, qrCode.width, qrCode.auto_color);
                    if (result.errcode == Senparc.Weixin.ReturnCode.请求成功)
                    {
                        return stream;

                    }
                }
            }
            return null;
        }
        #endregion
    }
}
