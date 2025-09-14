using System;
using System.Collections.Generic;

namespace JinChanChanTool.Tools.LineUpCodeTools
{
    public static class LineUpParser
    {       
        // 硬编码的S15赛季的代码字典
        private static readonly Dictionary<string, string> codeToNameMap = new Dictionary<string, string>
        {
            {"15a", "亚托克斯"}, {"168", "伊泽瑞尔"}, {"16a","盖伦"}, {"018", "纳尔"},
            {"173", "卡莉丝塔"}, {"175",  "凯尔"}, {"176", "凯南"}, {"017", "卢锡安"},
            {"17d",  "墨菲特"}, {"180", "纳亚菲利"}, {"185", "芮尔"}, {"01b", "希维尔"},
            {"18d", "辛德拉"}, {"197", "扎克"}, {"014", "蒙多医生"}, {"016", "普朗克"},
            {"16c", "迦娜"}, {"16f", "烬"}, {"172", "卡莎"}, {"174", "卡特琳娜"},
            {"177",  "可酷伯"}, {"17b","拉克丝"}, {"184",  "洛"}, {"00d", "慎"},
            {"01a",  "蔚"}, {"192", "霞"}, {"193", "赵信"}, {"15b", "阿狸"},
            {"164", "凯特琳"}, {"166", "德莱厄斯"}, {"16e", "杰斯"}, {"19a", "克格莫"},
            {"17e", "玛尔扎哈"}, {"01d", "妮蔻"}, {"1c1", "拉莫斯"}, {"013", "赛娜"},
            {"00f", "斯莫德"}, {"18c", "斯维因"}, {"18e", "乌迪尔"}, {"191", "佛耶戈"},
            {"194", "亚索"}, {"198", "吉格斯"}, {"15c", "阿卡丽"}, {"15e", "艾希"},
            {"16d", "嘉文四世"}, {"170", "金克丝"}, {"019", "卡尔玛"}, {"171","奎桑提"},
            {"179", "蕾欧娜"}, {"182", "波比"}, {"187", "瑞兹"}, {"188", "莎弥拉"},
            {"18a", "瑟提"}, {"199", "沃利贝尔"}, {"196","悠米"}, {"163","布隆"},
            {"16b", "格温"}, {"178", "李青"}, {"189", "萨勒芬妮"}, {"01c", "崔斯特"},
            {"190", "韦鲁斯"}, {"195", "永恩"}, {"01e", "婕拉"}
        };

        /// <summary>
        /// 将单个3位16进制字符串解密为英雄ID。
        /// </summary>
        /// <param name="hexStr">3位16进制字符串</param>
        /// <param name="mode">模式 (1=S15, 2=S10)</param>
        /// <returns>英雄ID</returns>
        private static string HexDecrypt(string hexStr)
        {
            if (codeToNameMap.ContainsKey(hexStr))
            {
                return codeToNameMap[hexStr];
            }
          

            // 如果字典里找不到对应的代码，就抛出一个异常
            throw new ArgumentException($"代码 '{hexStr}' 无法识别，不是有效的英雄代码。");
        }

        /// <summary>
        /// 解析完整的阵容代码字符串，返回英雄ID列表。
        /// </summary>
        /// <param name="tftHexStr">完整的阵容代码</param>
        /// <returns>包含多个英雄ID的列表</returns>
        public static List<string> ParseCode(string tftHexStr)
        {
            if (string.IsNullOrWhiteSpace(tftHexStr))
            {
                return new List<string>();
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

            List<string> heroes = new List<string>();
            // 按每3个字符一组进行分割和解析
            for (int i = 0; i < hexCore.Length; i += 3)
            {
                string chunk = hexCore.Substring(i, 3);
                // "000" 是空位，直接跳过
                if (chunk != "000")
                {
                    try
                    {
                        string heroName = HexDecrypt(chunk);
                        heroes.Add(heroName);
                    }
                    catch (Exception ex)
                    {
                        // 打印错误信息到控制台，方便调试，然后继续解析下一个
                        Console.WriteLine($"解析块 '{chunk}' 时出错: {ex.Message}");
                    }
                }
            }
            return heroes;
        }
    }
}