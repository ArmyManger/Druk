using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Druk.Common.Service
{
    /// <summary>
    /// HTTP客户端服务
    /// </summary>
    public static class HttpClientServices
    {
        /// <summary>
        /// 添加服务集合
        /// </summary>
        /// <param name="services"></param>
        /// <param name="clientList"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClientServiceList(this IServiceCollection services, List<Client> clientList)
        {
            if (clientList != null && clientList.Count > 0)
            {
                foreach (var item in clientList)
                {
                    var builder = services.AddHttpClient(item.clientName).ConfigureHttpClient(x =>
                    {
                        if (!string.IsNullOrEmpty(item.baseAddress))
                            x.BaseAddress = new Uri(item.baseAddress);
                        x.DefaultRequestHeaders.Connection.Add("keep-alive");
                    }
                     ).ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler()
                     {
                         AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                         ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                     });
                    if (item.lifetime > 0)
                        builder.SetHandlerLifetime(TimeSpan.FromMinutes(item.lifetime));
                }
            } 
            return services;
        } 
    }

    /// <summary>
    /// 客户端相关类
    /// </summary>
    public class Client
    {
        public string clientName { get; set; }

        public string baseAddress { get; set; }

        /// <summary>
        /// 存活时间（分钟）
        /// </summary>
        public int lifetime { get; set; } = 0; 
    }
}
