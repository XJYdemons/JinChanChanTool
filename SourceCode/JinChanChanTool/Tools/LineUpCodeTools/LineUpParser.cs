using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JinChanChanTool.Tools.LineUpCodeTools
{
    public static class LineUpParser
    {
        // 硬编码的S16赛季的代码字典
        private static readonly Dictionary<string, string> codeToNameMap = new Dictionary<string, string>
        {
            {"33e", "艾尼维亚"},{"34a", "布里茨"},{"32f", "贝蕾亚"},{"349", "凯特琳"},{"32c", "俄洛伊"},
            {"338", "嘉文四世"},{"374", "烬"},{"355", "克格莫"},{"2e0", "璐璐"},{"02c", "奇亚娜"},
            {"321", "兰博"}, {"351", "慎"}, {"33a", "娑娜"}, {"024", "佛耶戈"}, {"335", "厄斐琉斯"},
            {"33f", "艾希"}, {"018", "巴德"}, {"354", "科加斯"}, {"015", "艾克"}, {"027", "格雷福斯"},
            {"343", "妮蔻"}, {"02b", "奥莉安娜"}, {"36f", "波比"}, {"353", "雷克塞"}, {"32e", "赛恩"},
            {"320", "提莫"}, {"2df", "崔丝塔娜"}, {"011", "泰达米尔"}, {"323", "崔斯特"}, {"34b", "蔚"},
            {"010", "赵信"}, {"34d", "亚索"}, {"00f", "约里克"}, {"34f", "阿狸"}, {"36b", "德莱厄斯"},
            {"331", "德莱文"}, {"02f", "蒙多医生"}, {"324", "普朗克"}, {"01d", "格温"}, {"348", "金克丝"},
            {"370", "凯南"}, {"035", "可酷伯与悠米"}, {"017", "乐芙兰"}, {"334", "蕾欧娜"}, {"020", "洛里斯"},
            {"352", "玛尔扎哈"}, {"342", "米利欧"}, {"322", "诺提勒斯"}, {"361", "瑟庄妮"}, {"004", "薇恩"},
            {"333", "佐伊"}, {"332", "安蓓萨"}, {"366", "卑尔维斯"}, {"340", "布隆"}, {"023", "黛安娜"},
            {"36e", "菲兹"}, {"33c", "盖伦"}, {"01b", "卡莎"}, {"01e", "卡莉丝塔"}, {"341", "丽桑卓"},
            {"33d", "拉克丝"}, {"32d", "厄运小姐"}, {"022", "内瑟斯"}, {"014", "奈德丽"}, {"01c", "雷克顿"},
            {"016", "峡谷先锋"}, {"34c", "萨勒芬妮"}, {"367", "辛吉德"}, {"01a", "斯卡纳"}, {"025", "斯维因"},
            {"336", "塔里克"}, {"369", "维迦"}, {"36d", "沃里克"}, {"350", "孙悟空"}, {"36c", "永恩"},
            {"02a", "芸阿娜"}, {"372", "亚托克斯"}, {"356", "安妮"}, {"368", "奥瑞利安索尔"}, {"359", "阿兹尔"},
            {"36a", "纳什男爵"}, {"363", "岩宝"}, {"35b", "费德提克"}, {"35f", "加里奥"}, {"358", "千珏"},
            {"034", "卢锡安与赛娜"}, {"019", "梅尔"}, {"357", "奥恩"}, {"013", "瑞兹"}, {"362", "瑟提"},
            {"35d", "希瓦娜"}, {"012", "塞拉斯"}, {"360", "塔姆"}, {"365", "海克斯霸龙"}, {"021", "锤石"},
            {"373", "沃利贝尔"}, {"01f", "泽拉斯"}, {"030", "亚恒"}, {"371", "吉格斯"}, {"35a", "基兰"}
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

            // 目前只处理以 TFTSet16 结尾的代码
            if (!tftHexStr.EndsWith("TFTSet16"))
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
            string lineupCode = "02" + hexCore + "TFTSet16";

            return lineupCode;
        }      
    }
}