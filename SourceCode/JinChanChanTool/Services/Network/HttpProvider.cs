using System;
using System.Net.Http;

namespace JinChanChanTool.Services.Network
{
    /// <summary>
    /// 通过连接复用解决 SSL 握手异常，并支持高并发请求
    /// </summary>
    public static class HttpProvider
    {
        private static readonly HttpClient _instance;

        static HttpProvider()
        {
            var handler = new SocketsHttpHandler
            {
                //  允许连接长期存活，确保高并发时能充分复用已建立的 SSL 隧道
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),

                // 设置标准的闲置超时，30秒内无请求才会回收连接
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),

                // 提升单服务器最大连接上限
                // 这样在多个英雄请求迸发时，底层的 Socket 管道更充裕
                MaxConnectionsPerServer = 50
            };

            _instance = new HttpClient(handler)
            {
                // 设置合理的超时，防止个别请求卡死全局流程
                Timeout = TimeSpan.FromSeconds(30)
            };

            _instance.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        }

        public static HttpClient Client => _instance;
    }
}