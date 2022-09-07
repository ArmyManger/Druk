using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Druk.Common
{
    /// <summary>
    /// Http请求操作帮助类
    /// </summary>
    public static class DoNet
    {
        /// <summary>
        /// 通过HttpClient发起Get请求
        /// <para>键值对参数拼接在url上，后台使用[FromQuery]</para>
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="url">请求地址</param>
        /// <param name="query">请求参数</param>
        /// <returns>JSON字符串</returns>
        public static async Task<(bool isSuccess, string json)> GetAsync(HttpClient client, string url, object query)
        {
            var api = $"{url}{DoHttpContent.BuildParam(DoHttpContent.ToKeyValuePair(query))}";
            var response = await client.GetAsync(api);
            var jsonString = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, jsonString);
        }

        /// <summary>
        /// 通过HttpClient发起Post请求
        /// <para><see cref="HttpContext"/>区别：</para>
        /// <para><see cref="MultipartFormDataContent"/>、<see cref="FormUrlEncodedContent"/>、<see cref="StreamContent"/>、<see cref="ByteArrayContent"/>后台使用[FromForm]接受参数</para>
        /// <para><see cref="StringContent"/>后台使用[FromBody]接受参数</para>
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="url">请求地址</param>
        /// <param name="query"></param>
        /// <param name="content">请求参数</param>
        /// <returns>JSON字符串</returns>
        public static async Task<(bool isSuccess, string json)> PostAsync(HttpClient client, string url, object query, HttpContent content)
        {
            var api = $"{url}{DoHttpContent.BuildParam(DoHttpContent.ToKeyValuePair(query))}";
            var response = await client.PostAsync(api, content);
            var jsonString = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, jsonString);
        }

        /// <summary>
        /// 将HTTP请求作为异步操作发送
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="url">请求地址</param>
        /// <param name="method">请求方法</param>
        /// <param name="query"></param>
        /// <param name="content">请求内容</param>
        /// <returns></returns>
        public static async Task<(bool isSuccess, string json)> SendAsync(HttpClient client, string url, HttpMethod method, object query, HttpContent content)
        {
            var api = $"{url}{DoHttpContent.BuildParam(DoHttpContent.ToKeyValuePair(query))}";
            var httpRequestMessage = new HttpRequestMessage(method, api)
            {
                Content = content
            };
            var response = await client.SendAsync(httpRequestMessage);
            var jsonString = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, jsonString);
        }
        /// <summary>
        /// 创建 HttpClient 实例
        /// </summary>
        /// <returns></returns>
        public static HttpClient CreateClient()
        {
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            return client;
        }
        /// <summary>
        /// 设置 Bearer 授权
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="token"></param>
        public static void SetBearerHeader(this HttpClient httpClient, string token)
        {
            if (httpClient.IsNotNull() && token.IsNotNullOrEmpty())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
