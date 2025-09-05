using System;
using System.Collections.Generic;

namespace JinChanChanTool.Tools.LineUpCodeTools
{
    public static class LineUpParser
    {
        // 硬编码的S15赛季的代码字典
        private static readonly Dictionary<string, int> mode1_d = new Dictionary<string, int>
        {
            {"15a", 10357}, {"168", 10368}, {"16a", 10371}, {"018", 10372},
            {"173", 10380}, {"175", 10383}, {"176", 10384}, {"017", 10391},
            {"17d", 10394}, {"180", 10396}, {"185", 10402}, {"01b", 10409},
            {"18d", 10419}, {"197", 10434}, {"014", 10366}, {"016", 10370},
            {"16c", 10374}, {"16f", 10377}, {"172", 10379}, {"174", 10382},
            {"177", 10385}, {"17b", 10393}, {"184", 10399}, {"00d", 10408},
            {"01a", 10425}, {"192", 10429}, {"193", 10430}, {"15b", 10358},
            {"164", 10362}, {"166", 10365}, {"16e", 10376}, {"19a", 10386},
            {"17e", 10395}, {"01d", 10397}, {"1c1", 10400}, {"013", 10405},
            {"00f", 10410}, {"18c", 10413}, {"18e", 10423}, {"191", 10426},
            {"194", 10431}, {"198", 10435}, {"15c", 10359}, {"15e", 10360},
            {"16d", 10375}, {"170", 10378}, {"019", 10381}, {"171", 10387},
            {"179", 10390}, {"182", 10398}, {"187", 10403}, {"188", 10404},
            {"18a", 10407}, {"199", 10427}, {"196", 10433}, {"163", 10361},
            {"16b", 10373}, {"178", 10388}, {"189", 10406}, {"01c", 10422},
            {"190", 10424}, {"195", 10432}, {"01e", 10436}
        };


        /// <summary>
        /// 将单个3位16进制字符串解密为英雄ID。
        /// </summary>
        /// <param name="hexStr">3位16进制字符串</param>
        /// <param name="mode">模式 (1=S15, 2=S10)</param>
        /// <returns>英雄ID</returns>
        private static int HexDecrypt(string hexStr)
        {
            if (mode1_d.ContainsKey(hexStr))
            {
                return mode1_d[hexStr];
            }
          

            // 如果字典里找不到对应的代码，就抛出一个异常
            throw new ArgumentException($"代码 '{hexStr}' 无法识别，不是有效的英雄代码。");
        }

        /// <summary>
        /// 解析完整的阵容代码字符串，返回英雄ID列表。
        /// </summary>
        /// <param name="tftHexStr">完整的阵容代码</param>
        /// <returns>包含多个英雄ID的列表</returns>
        public static List<int> ParseCode(string tftHexStr)
        {
            if (string.IsNullOrWhiteSpace(tftHexStr))
            {
                return new List<int>();
            }

            // 目前只处理以 TFTSet15 结尾的代码
            if (!tftHexStr.EndsWith("TFTSet15"))
            {
                // 如果需要支持其他赛季，可以修改这里的逻辑
                throw new ArgumentException("当前只支持S15赛季的阵容代码 (以 TFTSet15 结尾)");
            }
            

            // 提取核心的16进制字符串 (去除前2位和后8位)
            // C# 的 Substring(startIndex, length)
            string hexCore = tftHexStr.Substring(2, tftHexStr.Length - 10);

            var heroIdList = new List<int>();
            // 按每3个字符一组进行分割和解析
            for (int i = 0; i < hexCore.Length; i += 3)
            {
                string chunk = hexCore.Substring(i, 3);
                // "000" 是空位，直接跳过
                if (chunk != "000")
                {
                    try
                    {
                        int heroId = HexDecrypt(chunk);
                        heroIdList.Add(heroId);
                    }
                    catch (Exception ex)
                    {
                        // 打印错误信息到控制台，方便调试，然后继续解析下一个
                        Console.WriteLine($"解析块 '{chunk}' 时出错: {ex.Message}");
                    }
                }
            }
            return heroIdList;
        }
    }
}