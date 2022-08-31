using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Druk.Common
{
    /// <summary>
    /// 请求
    /// </summary>
    public static class DoWebRequest
    {
        #region //是否是PC浏览器

        /// <summary>
        /// 判断当前访问是否来自浏览器软件
        /// </summary>
        /// <returns>当前访问是否来自浏览器软件</returns>
        public static bool IsBrowserGet(this Microsoft.AspNetCore.Http.HttpContext http)
        {
            string[] BrowserName = { "ie", "opera", "netscape", "mozilla", "konqueror", "firefox" };
            string curBrowser = http.GetUserAgent();
            for (int i = 0; i < BrowserName.Length; i++)
            {
                if (curBrowser.IndexOf(BrowserName[i]) >= 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region //是否是手机浏览器
        /// <summary>
        /// 是否是手机浏览器
        /// </summary>
        /// <param name="http"></param>
        /// <returns></returns>
        public static bool IsMobileBrowser(this Microsoft.AspNetCore.Http.HttpContext http)
        {
            //regex from http://detectmobilebrowsers.com/
            Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            var userAgent = http.GetUserAgent();
            if ((b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
            {
                return true;
            }

            return false;
        }
        #endregion
         
        #region //获取请求信息

        /// <summary>
        /// 返回指定的服务器变量信息
        /// </summary>
        /// <param name="http">请求上下文</param>
        /// <param name="strName">服务器变量名</param>
        /// <returns>服务器变量信息</returns>
        public static string GetHeaders(this Microsoft.AspNetCore.Http.HttpContext http, string strName)
        {
            return http.Request.Headers[strName].FirstOrDefault();
        }


        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public static string GetIP(this Microsoft.AspNetCore.Http.HttpContext http)
        {
            var IP = http.GetHeaders("X-Forwarded-For");
            if (string.IsNullOrEmpty(IP))
            {
                IP = http.Connection.RemoteIpAddress.ToString();
            }
            return IP;
        }


        /// <summary>
        /// 获取请求的协议 是Http / Https 或者其他
        /// </summary>
        /// <returns></returns>
        public static string GetProtocol(this Microsoft.AspNetCore.Http.HttpContext http)
        {
            var Protocol = http.GetHeaders("X-Original-Proto");
            if (string.IsNullOrEmpty(Protocol))
            {
                Protocol = http.Request.Protocol;
            }
            return Protocol;
        }

        /// <summary>
        /// 获取请求的内容方式
        /// </summary>
        public static string GetContentType(this Microsoft.AspNetCore.Http.HttpContext http)
        {
            return (http.Request.ContentType ?? "").ToLower();
        }

        /// <summary>
        /// 获取请求的方式
        /// </summary>
        /// <returns></returns>
        public static string GetMethod(this Microsoft.AspNetCore.Http.HttpContext http)
        {
            return http.Request.Method.ToUpper();
        }

        /// <summary>
        /// 获取用户代理(浏览器名)
        /// </summary>
        /// <param name="http"></param>
        /// <returns></returns>
        public static string GetUserAgent(this Microsoft.AspNetCore.Http.HttpContext http)
        {
            return http.GetHeaders("User-Agent");
        }
        /// <summary>
        /// 获取请求的完整路径
        /// </summary>
        /// <param name="http"></param>
        /// <returns></returns>
        public static string GetRequestURL(this Microsoft.AspNetCore.Http.HttpContext http)
        {
            return http.GetProtocol() + "://" + http.Request.Host.Value + http.Request.Path.Value;
        }

        #endregion

        #region //获取版本

        private static FileVersionInfo AssemblyFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        /// <summary>
        /// 获得Assembly版本号
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyVersion()
        {
            return string.Format("{0}.{1}.{2}", AssemblyFileVersion.FileMajorPart, AssemblyFileVersion.FileMinorPart, AssemblyFileVersion.FileBuildPart);
        }

        /// <summary>
        /// 获得Assembly产品名称
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyProductName()
        {
            return AssemblyFileVersion.ProductName;
        }

        /// <summary>
        /// 获得Assembly产品版权
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyCopyright()
        {
            return AssemblyFileVersion.LegalCopyright;
        }
        #endregion
         
        #region //编码和解码URL

        /// <summary>
        /// 对Url地址进行 编码
        /// </summary>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 对URL地址进行 解码
        /// </summary>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }
        #endregion

        #region //GetQueryString
        /// <summary>
        /// 根据参数名获取值
        /// </summary>
        /// <param name="http"></param>
        /// <param name="ParamName"></param>
        /// <returns></returns>
        public static string Query(this Microsoft.AspNetCore.Http.HttpContext http, string ParamName)
        {
            var dic = http.GetQueryStringToDic();
            if (dic.ContainsKey(ParamName.ToLower()))
            {
                return UrlDecode(dic[ParamName.ToLower()]);
            }
            return string.Empty;
        }

        /// <summary>
        /// 将QueryString值转换为键值对Dic string,string
        /// </summary>
        /// <param name="http"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetQueryStringToDic(this Microsoft.AspNetCore.Http.HttpContext http)
        {
            var result = new Dictionary<string, string>();
            var query = http.GetQueryString();
            if (!string.IsNullOrEmpty(query))
            {
                var temp = query.Split(new char[] { '&', '?' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                temp.ForEach(tt =>
                {
                    var temp2 = tt.Split('=');
                    if (temp2.Length == 2)
                    {
                        if (!result.ContainsKey(temp2[0].ToLower()))
                        {
                            result.Add(temp2[0], temp2[1]);
                        }
                    }
                });
            }
            return result;
        }

        /// <summary>
        /// 获取URL参数值列表
        /// </summary>
        /// <returns></returns>
        public static string GetQueryString(this Microsoft.AspNetCore.Http.HttpContext http)
        {
            var Reque = http.Request;
            if (Reque != null && Reque.QueryString.HasValue)
            {
                return Reque.QueryString.Value;
            }
            return string.Empty;
        } 
        #endregion

        #region //GetBodyForm

        /// <summary>
        /// 获取Body里的参数
        /// </summary>
        public static string GetBodyForm(this Microsoft.AspNetCore.Http.HttpContext http)
        {
            var Reque = http.Request;

            if (Reque != null)
            {
                var ms = new MemoryStream();
                try
                {
                    Reque.EnableBuffering();
                    Reque.Body.Position = 0;
                    Reque.Body.CopyTo(ms);
                    var postStr = Encoding.UTF8.GetString(ms.ToArray());
                    ms.Position = 0;
                    Reque.Body = ms;
                    var result = postStr.Replace("\n", "");
                    var i = 0; do { result = result.Replace("  ", " ").Replace("  ", " ").Replace("  ", " "); i++; } while (i < 5);
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return string.Empty;
                }
                finally
                {
                    //刻意不关闭,因为ms如果关闭..因为引用传递到了Request.Body .所以Body的流也会关闭
                    //导致进入控制器和下次读取的时候依然是读不到 和 报错 "不能读取已经关闭的流"
                    //ms.Close();
                    //ms.Dispose();
                }
            }
            return null;
        }

        #endregion

        #region //工具 拆分参数字符串到键值对 
        public static Dictionary<string, string> GetDicFromStr(string strParam)
        {
            if (strParam.StartsWith('{')) return strParam.ToObjectFromJson<Dictionary<string, string>>();
            var dic = new Dictionary<string, string>();
            var value = UrlDecode(strParam).ToLower().Split(new char[] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries);
            value.ToList().ForEach(str =>
            {
                var kv = str.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (kv.Length == 2)
                {
                    if (dic.ContainsKey(kv[0])) { dic[kv[0]] = kv[1]; } else { dic.Add(kv[0], kv[1]); }
                }
            });
            return dic;
        }
        #endregion

        #region //模拟发送请求 用于接口调用
        /// <summary>
        /// Post提交Url，并获取返回值
        /// </summary>
        /// <param name="posturl">提交到的页面</param>
        /// <param name="postData">post参数 要拼接在Body中的参数</param>
        /// <param name="getData">url参数(例：name=jim&age=1&height=177)</param>
        /// <param name="encodeStr">编码方式(例："gb2312","utf-8")</param>
        /// <param name="TimeOut">超时时间(单位:毫秒) 可以设置,默认100秒 </param>
        /// <returns>请求返回的数据</returns>
        public static string SendRequest_Post(string posturl, string postData, string getData = "", string ContentType = "application/json", string encodeStr = "UTF-8", int TimeOut = 100 * 1000)
        {
            var encoding = System.Text.Encoding.GetEncoding(encodeStr);
            var data = encoding.GetBytes(postData);
            // 准备请求...
            try
            {
                getData = string.IsNullOrEmpty(getData) ? string.Empty : (getData.StartsWith("?") ? getData : "?" + getData);
                // 设置参数
                var request = WebRequest.Create(posturl + getData) as HttpWebRequest;
                var cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = ContentType;// "application/x-www-form-urlencoded ; charset=UTF-8";
                request.ContentLength = data.Length;
                request.Timeout = TimeOut;
                var outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据
                var response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                var instream = response.GetResponseStream();
                var sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                var content = sr.ReadToEnd();
                var err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToSimple());
                return string.Empty;
            }
        }


        public static string SendRequest_Post(string url, Stream postStream, Dictionary<string, string> head = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "image/jpeg";
            request.ContentLength = postStream != null ? postStream.Length : 0;
            request.UserAgent = "Mozilla/4.0";
            try
            {
                if (head != null)
                {
                    foreach (var key in head.Keys)
                    {
                        request.Headers[key] = head[key];
                    }
                }

                if (postStream != null)
                {
                    //上传文件流
                    Stream requestStream = request.GetRequestStream();

                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }

                    postStream.Close();//关闭文件访问
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();


                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
                    {
                        string retString = myStreamReader.ReadToEnd();
                        return retString;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToSimple());
                return string.Empty;
            }
        }

        /// <summary>
        /// Get提交Url，并获取返回值
        /// </summary>
        /// <param name="url">远程访问的地址</param>
        /// <param name="data">参数</param>
        /// <param name="head">head</param>
        /// <returns>远程页面调用结果</returns>
        public static string SendRequest_Get(string url, string data, Dictionary<string, string> head)
        {
            HttpWebRequest request = null;
            url = url + (data.Length > 0 ? "?" + data : "");
            request = WebRequest.Create(url) as HttpWebRequest;
            if (head != null)
            {
                foreach (var key in head.Keys)
                {
                    request.Headers[key] = head[key];

                }
            }

            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamIn = response.GetResponseStream();

            StreamReader reader = new StreamReader(streamIn);
            string result = reader.ReadToEnd();
            reader.Close();
            streamIn.Close();
            response.Close();

            return result;
        }


        /// <summary>
        /// Get提交Url，并获取返回值
        /// </summary>
        /// <param name="posturl">提交到的页面</param>
        /// <param name="postData">url参数(例：name=jim&age=1&height=177)</param>
        /// <param name="encodeStr">编码方式(例："gb2312","utf-8")</param>
        /// <returns>请求返回的数据</returns>
        public static string SendRequest_Get(string posturl, string postData, string encodeStr = "UTF-8", string ContentType = "application/json", Dictionary<string, string> head = null)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Accept", ContentType);
                    httpClient.DefaultRequestHeaders.Add("AcceptCharset", encodeStr);
                    if (head != null)
                    {
                        foreach (var key in head.Keys)
                        {
                            httpClient.DefaultRequestHeaders.Add(key, head[key]);
                        }
                    }
                    // response
                    var response = httpClient.GetAsync(posturl + (postData.Length > 0 ? "?" + postData : "")).Result;
                    var data = response.Content.ReadAsStringAsync().Result;
                    return data;//接口调用成功获取的数据
                }
            }
            catch (Exception ex)
            {
                //return string.Empty;
                return ex.ToString();
            }
        }

        /// <summary>
        /// post请求数据（注意，此方法只支持json传参）
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="requestJson">json参数</param>
        /// <param name="head"></param>
        /// <returns>返回json格式的字符串</returns>
        public static string PostAsync(string url, string requestJson, Dictionary<string, string> head = null)
        {
            string result = "";
            try
            {
                Uri postUrl = new Uri(url);

                using (HttpContent httpContent = new StringContent(requestJson))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    if (head != null)
                    {
                        foreach (var key in head.Keys)
                        {
                            httpContent.Headers.Add(key, head[key]);
                        }
                    }
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.Timeout = new TimeSpan(0, 0, 60);
                        result = httpClient.PostAsync(url, httpContent).Result.Content.ReadAsStringAsync().Result;
                    }

                }

            }
            catch (Exception ex)
            {
                result = "接口调用出错！[" + ex.Message + "]";
            }
            return result;
        }

        public static string PostDataTaskAsync(string url, string data, Dictionary<string, string> head = null)
        {
            try
            {
                var postData = Encoding.UTF8.GetBytes(data ?? "");
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Content-Type", "application/json");
                if (head != null)
                {
                    foreach (var item in head)
                    {
                        webClient.Headers.Add(item.Key, item.Value);
                    }
                }
                byte[] responseData = webClient.UploadDataTaskAsync(url, "POST", postData).Result;//得到返回字符流  
                string srcString = Encoding.UTF8.GetString(responseData);//解码  
                return srcString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetData(string url, string data, Dictionary<string, string> head = null)
        {
            try
            {
                var postData = Encoding.UTF8.GetBytes(data ?? "");
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Content-Type", "application/json");
                if (head != null)
                {
                    foreach (var item in head)
                    {
                        webClient.Headers.Add(item.Key, item.Value);
                    }
                }
                byte[] responseData = webClient.UploadData(url, "GET", postData);//得到返回字符流  
                string srcString = Encoding.UTF8.GetString(responseData);//解码  
                return srcString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region form处理
        /// <summary>
        /// 通过字典来获取参数拼接
        /// </summary>
        /// <param name="formDic">参数列表</param>
        /// <param name="trimFirstAndSymbol">是否去除头部的&符号</param>
        /// <returns></returns>
        public static string GetFormParamStr(Dictionary<string, string> formDic, bool trimFirstAndSymbol = false)
        {
            var strToReturn = "";
            foreach (var formStr in formDic)
            {
                strToReturn += string.Format("&{0}={1}", formStr.Key, UrlEncode(formStr.Value));
            }
            if (trimFirstAndSymbol)
                return strToReturn.TrimStart('&');
            return strToReturn;
        }

        public static string SendPostForm(string url, Dictionary<string, object> dic, Dictionary<string, string> heads)
        {
            try
            {
                HttpClient _httpClient = new HttpClient(); 
                var postContent = new MultipartFormDataContent(); 
                string boundary = string.Format("--{0}", DateTime.Now.Ticks.ToString("x"));
                postContent.Headers.Add("ContentType", $"multipart/form-data, boundary={boundary}"); 
                if (heads != null)
                {
                    foreach (var keyValueHeads in heads)
                    {
                        if (keyValueHeads.Value != null)
                        {
                            _httpClient.DefaultRequestHeaders.Add(keyValueHeads.Key, keyValueHeads.Value);
                        }
                    }
                } 
                foreach (var keyValuePair in dic)
                {
                    if (keyValuePair.Value != null)
                    {
                        postContent.Add(new StringContent(keyValuePair.Value.ToString()), keyValuePair.Key);
                    }
                }
                var re = _httpClient.PostAsync(url, postContent);
                return re.Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                return "请求异常:" + ex.Message;
            }
        }

        public static string PostFrom(string url, string data)
        {
            string htmlAll = "";
            try
            {
                string SendMessageAddress = url;//请求链接
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SendMessageAddress);
                request.Method = "POST";
                request.AllowAutoRedirect = true;
                request.Timeout = 20 * 1000;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("charset", "utf-8");
                //string PostData = "a=1&b=2";//请求参数格式
                string PostData = data;//请求参数
                byte[] byteArray = Encoding.Default.GetBytes(PostData);
                request.ContentLength = byteArray.Length;
                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                    newStream.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream rspStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(rspStream, Encoding.UTF8))
                {
                    htmlAll = reader.ReadToEnd();
                    rspStream.Close();
                }
                response.Close();
            }
            catch (Exception ex)
            {

                string s = ex.Message;
            }
            return htmlAll;
        }


        /// <summary>
        /// 模拟请求数据（注意，此方法只支持json传参）
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="requestJson">json参数</param>
        /// <param name="headers">headers参数</param>
        /// <returns>返回json格式的字符串</returns>
        public static (HttpStatusCode statusCode, string content) HttpRequestByForm(string url, string requestJson, Dictionary<string, string> headers = null)
        {
            #region MyRegion
            HttpStatusCode statusCode;
            string content;
            try
            {
                using HttpContent httpContent = new StringContent(requestJson);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                using var httpClient = new HttpClient();
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                    }
                }
                httpClient.Timeout = new TimeSpan(0, 0, 100);
                HttpResponseMessage httpResult = httpClient.PostAsync(url, httpContent).Result;
                statusCode = httpResult.StatusCode;
                content = httpResult.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                return (0, e.Message);
            }
            return (statusCode, content);
            #endregion
        }
        #endregion
    }
}
