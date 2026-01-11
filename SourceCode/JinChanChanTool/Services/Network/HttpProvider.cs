using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;

namespace JinChanChanTool.Services.Network
{
    // 定义一个拦截器，负责加签名头
    public class SignatureHandler : DelegatingHandler
    {
        public SignatureHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 动态计算当前的签名
            string sign = SignatureHelper.GenerateTimeSign();

            // 添加到请求头 
            if (request.Headers.Contains("X-Time-Sign"))
            {
                request.Headers.Remove("X-Time-Sign");
            }
            request.Headers.Add("X-Time-Sign", sign);

            return base.SendAsync(request, cancellationToken);
        }
    }

    public static class HttpProvider
    {
        private static readonly HttpClient _instance;

        static HttpProvider()
        {
            // 1. 基础 Socket Handler
            var socketsHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                MaxConnectionsPerServer = 100,
                AutomaticDecompression = DecompressionMethods.All,
                SslOptions = new SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = delegate { return true; }
                }
            };

            // 2. 包装上签名拦截器
            var signatureHandler = new SignatureHandler(socketsHandler);

            // 3. 创建 Client
            _instance = new HttpClient(signatureHandler)
            {
                Timeout = TimeSpan.FromSeconds(100),
                DefaultRequestVersion = HttpVersion.Version11,
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
            };

            _instance.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _instance.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        public static HttpClient Client => _instance;
    }
}