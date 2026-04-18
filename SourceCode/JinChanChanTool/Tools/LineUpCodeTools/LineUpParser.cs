using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JinChanChanTool.Tools.LineUpCodeTools
{
    public static class LineUpParser
    {
        // 硬编码的S17赛季的代码字典
        private static readonly Dictionary<string, string> codeToNameMap = new Dictionary<string, string>
        {
          {"01d","亚托克斯"},{"00e","贝蕾亚"},{"01b","凯特琳"},{"045","科加斯"},{"03e","伊泽瑞尔"},
          {"042","蕾欧娜"},{"046","丽桑卓"},{"034","内瑟斯"},{"01a","波比"},{"068","雷克塞"},
          {"02a","泰隆"},{"033","提莫"},{"02b","崔斯特"},{"016","维迦"},{"00d","阿卡丽"},
          {"00f","卑尔维斯"},{"017","纳尔"},{"031","古拉加斯"},{"035","格温"},{"014","小木灵"},
          {"02c","贾克斯"},{"012","金克丝"},{"03d","米利欧"},{"04e","莫德凯撒"},{"025","潘森"},
          {"02d","派克"},{"041","佐伊"},{"010","阿萝拉"},{"043","黛安娜"},{"015","菲兹"},
          {"011","俄洛伊"},{"020","卡莎"},{"030","璐璐"},{"01e","茂凯"},{"001","厄运小姐"},
          {"036","奥恩"},{"021","拉亚斯特"},{"032","莎弥拉"},{"024","厄加特"},{"02e","维克托"},
          {"026","奥瑞利安索尔"},{"019","库奇"},{"027","超级机甲"},{"022","卡尔玛"},{"01f","千珏"},
          {"044","乐芙兰"},{"02f","易"},{"037","娜美"},{"039","努努和威朗普"},{"018","拉莫斯"},
          {"03c","锐雯"},{"04f","塔姆"},{"03f","霞"},{"01c","巴德"},{"038","布里茨"},
          {"013","菲奥娜"},{"050","格雷福斯"},{"023","烬"},{"058","莫甘娜"},{"03b","慎"},
          {"029","娑娜"},{"03a","薇古丝"},{"047","劫"}
        };

        // 从英雄名到代码的逆映射
        private static readonly Dictionary<string, string> nameToCodeMap;

        static LineUpParser()
        {
            // 初始化逆映射字典
            nameToCodeMap = new Dictionary<string, string>();
            foreach (var pair in codeToNameMap)
            {
                if (!nameToCodeMap.ContainsKey(pair.Value))
                {
                    nameToCodeMap[pair.Value] = pair.Key;
                }
            }
        }

        /// <summary>
        /// 将单个3位16进制字符串解密为英雄ID。
        /// </summary>
        /// <param name="hexStr">3位16进制字符串</param>
        /// <returns>英雄名</returns>
        private static string HexDecrypt(string hexStr)
        {
            if (codeToNameMap.ContainsKey(hexStr))
            {
                return codeToNameMap[hexStr];
            }
            Debug.WriteLine($"代码 '{hexStr}' 无法识别，不是有效的英雄代码。");
            return "";
        }

        /// <summary>
        /// 将英雄名加密为3位16进制字符串
        /// </summary>
        /// <param name="heroName">英雄名</param>
        /// <returns>3位16进制字符串，如果英雄不在字典中则返回null</returns>
        private static string? HexEncrypt(string heroName)
        {
            if (nameToCodeMap.TryGetValue(heroName, out string? code))
            {
                return code;
            }

            // 字典中找不到的英雄（如提伯斯等宠物）返回null，由调用方跳过
            return null;
        }

        /// <summary>
        /// 解析完整的阵容代码字符串，返回英雄名列表。
        /// </summary>
        /// <param name="tftHexStr">完整的阵容代码</param>
        /// <returns>包含多个英雄名的列表</returns>
        public static List<string> ParseCode(string tftHexStr)
        {
            if (string.IsNullOrWhiteSpace(tftHexStr))
            {
                return new List<string>();
            }

            // 目前只处理以 TFTSet17 结尾的代码
            if (!tftHexStr.EndsWith("TFTSet17"))
            {
                Debug.WriteLine("解析失败，阵容码不正确！");
                return new List<string>();
            }

            // 提取核心的16进制字符串 (去除前2位和后8位)
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
                        if(heroName!="")
                        {
                            heroes.Add(heroName);
                        }                        
                    }
                    catch (Exception ex)
                    {
                        // 打印错误信息到控制台，方便调试，然后继续解析下一个
                        Debug.WriteLine($"解析块 '{chunk}' 时出错: {ex.Message}");
                    }
                }
            }
            return heroes;
        }

        /// <summary>
        /// 根据英雄名列表生成阵容代码
        /// </summary>
        /// <param name="heroNames">英雄名列表</param>
        /// <param name="maxHeroCount">阵容最大英雄数量(默认9个，包括空位)</param>
        /// <returns>阵容代码字符串</returns>
        public static string GenerateCode(List<string> heroNames)
        {
            if (heroNames == null || heroNames.Count == 0)
            {
                throw new ArgumentException("英雄名列表不能为空");
            }

            List<string> hexCodes = new List<string>();

            // 如果英雄数量超过最大数量，只取前10个
            int effectiveCount = Math.Min(heroNames.Count, 10);

            // 转换英雄名为十六进制代码（只转换前10个）
            for (int i = 0; i < effectiveCount; i++)
            {
                string? hexCode = HexEncrypt(heroNames[i]);
                // 跳过字典中不存在的英雄（如提伯斯等宠物）
                if (hexCode != null)
                {
                    hexCodes.Add(hexCode);
                }
            }

            // 用"000"填充剩余位置
            while (hexCodes.Count < 10)
            {
                hexCodes.Add("000");
            }

            // 组合所有十六进制代码
            string hexCore = string.Join("", hexCodes);

            // 添加前缀和后缀
            string lineupCode = "02" + hexCore + "TFTSet17";

            return lineupCode;
        }      
    }
}