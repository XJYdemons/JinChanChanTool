using System;
using System.Security.Cryptography;
using System.Text;

namespace JinChanChanTool.Services.Network
{
    public static class SignatureHelper
    {
        private const string SECRET_SALT = "JinChanChan_Salt_v1_998244353";

        public static string GenerateTimeSign()
        {
            try
            {
                // 1. 获取当前 UTC 时间戳
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 60;

                // 2. 拼接密钥
                string raw = $"{timestamp}-{SECRET_SALT}";

                // 3. 计算 SHA256
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw));

                    // 4. 转 Hex 字符串 (小写)
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}