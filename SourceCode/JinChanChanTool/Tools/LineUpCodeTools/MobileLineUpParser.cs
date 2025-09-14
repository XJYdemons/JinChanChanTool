using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JinChanChanTool.Tools.LineUpCodeTools
{
    public static class MobileLineUpParser
    {      
        // 金铲铲ID -> 云顶Id 映射表
        private static readonly Dictionary<int, string> CodeToNameMap = new Dictionary<int, string>
        {
            // --- 1费 ---
            { 001, "亚托克斯" },   // 亚托克斯
            { 033, "纳尔" },   // 纳尔
            { 013, "盖伦" },  // 盖伦
            { 009, "芮尔" },  // 芮尔
            { 029, "纳亚菲利" },  // 纳亚菲利
            { 037, "墨菲特" },  // 墨菲特
            { 045, "辛德拉" },  // 辛德拉
            { 049, "卢锡安" },  // 卢锡安
            { 053, "凯南" },  // 凯南
            { 061, "卡莉丝塔" },  // 卡莉斯塔
            { 065, "希维尔" },  // 希维尔
            { 069, "伊泽瑞尔" },  // 伊泽瑞尔
            { 073, "凯尔" },  // 凯尔
            { 021, "扎克" },  // 扎克

            // --- 2费 ---
            { 085, "普朗克" },  // 普朗克
            { 089, "慎" },  // 慎
            { 097, "可酷伯" },  // 可酷伯
            { 101, "洛" }, // 洛
            { 105, "赵信" }, // 赵信
            { 109, "蔚" }, // 蔚
            { 113, "蒙多医生" }, // 蒙多
            { 121, "卡莎" }, // 卡莎
            { 125, "卡特琳娜" }, // 卡特琳娜
            { 129, "霞" }, // 霞
            { 133, "烬" }, // 烬
            { 137, "拉克丝" }, // 拉克丝
            { 141, "迦娜" }, // 迦娜
            
            // --- 3费 ---
            { 149, "佛耶戈" }, // 佛耶戈
            { 153, "乌迪尔" }, // 乌迪尔
            { 169, "斯维因" }, // 斯维因
            { 177, "亚索" }, // 亚索
            { 181, "杰斯" }, // 杰斯
            { 185, "妮蔻" }, // 妮蔻
            { 193, "阿狸" }, // 阿狸
            { 197, "赛娜" }, // 赛娜
            { 201, "吉格斯" }, // 吉格斯
            { 205, "凯特琳" }, // 凯特琳
            { 209, "德莱厄斯" }, // 德莱厄斯
            { 213, "玛尔扎哈" }, // 玛尔扎哈
            { 217, "璐璐" }, // 璐璐
            { 221, "克格莫" }, // 克格莫
            { 225, "拉莫斯" }, // 拉莫斯
            { 229, "斯莫德" }, // 斯莫德

            // --- 4费 ---
            { 233, "蕾欧娜" }, // 蕾欧娜
            { 237, "瑟提" }, // 瑟提
            { 241, "阿卡丽" }, // 阿卡丽
            { 245, "波比" }, // 波比
            { 249, "嘉文四世" }, // 嘉文四世
            { 253, "沃利贝尔" }, // 沃利贝尔
            { 257, "奎桑提" }, // 奎桑提
            { 261, "艾希" }, // 艾希
            { 265, "莎弥拉" }, // 莎弥拉
            { 269, "瑞兹" }, // 瑞兹
            { 273, "卡尔玛" }, // 卡尔玛
            { 281, "悠米" }, // 悠米
            { 277, "金克丝" }, // 金克丝

            // --- 5费 ---
            { 285, "布隆" }, // 布隆
            { 289, "格温" }, // 格温
            { 293, "永恩" }, // 永恩
            { 297, "崔斯特" }, // 崔斯特
            { 301, "萨勒芬妮" }, // 萨勒芬妮
            { 305, "婕拉" }, // 婕拉
            { 309, "韦鲁斯" }, // 韦鲁斯
            { 313, "李青" }, // 李青 (裁决)
            { 317, "李青" }, // 李青 (决斗)
            { 321, "李青" }, // 李青 (主宰)
            { 381, "李青" }  // 李青 (普通)
        };
        public static List<string> Parse(string fullCode)
        {
            byte[] decompressedBytes = DecompressCode(fullCode);
            string dataString = Encoding.UTF8.GetString(decompressedBytes);

            var heros = new List<string>();

            // 1. 读取英雄数量 (第31-32位)
            int heroCount = int.Parse(dataString.Substring(30, 2)); // 位置从0开始，所以是30
            if (heroCount == 0) return heros;

            // 2. 英雄数据从第33个字符开始
            int currentPosition = 32; // 位置从0开始，所以是32

            // 3. 严格按照英雄数量，循环解析每个不定长的数据块
            for (int i = 0; i < heroCount; i++)
            {
                // 确保剩余字符串足够长，能容纳一个最小的英雄数据块 (6个字符: ID+Pos+EquipCount)
                if (currentPosition + 6 > dataString.Length) break;

                // a. 读取3位动态ID
                string dynamicIdStr = dataString.Substring(currentPosition, 3);
                int dynamicId = int.Parse(dynamicIdStr);
                currentPosition += 3;

                // b. 将动态ID解码为基础ID，并找到对应的PC版ChessId
                foreach (var entry in CodeToNameMap)
                {
                    int baseId = entry.Key;
                    // 检查动态ID是否在 [基础ID, 基础ID+2] 的范围内
                    if (dynamicId >= baseId && dynamicId <= baseId + 2)
                    {
                        heros.Add(entry.Value);
                        break; // 找到后就跳出内层循环
                    }
                }

                // c. 跳过2位位置
                currentPosition += 2;

                // d. 读取1位装备数量，这是计算游标跳跃距离
                int equipCount = int.Parse(dataString.Substring(currentPosition, 1));
                currentPosition += 1;

                // e. 根据装备数量，精准地跳过装备ID数据 (每个装备ID占3个字符)
                currentPosition += (equipCount * 3);
            }

            return heros.Distinct().ToList();
        }

        private static byte[] DecompressCode(string fullCode)
        {
            Match roughMatch = Regex.Match(fullCode, @"@@(.*?)##");
            if (!roughMatch.Success) throw new ArgumentException("格式错误: 未找到 '@@...##' 结构。");
            string dirtyData = roughMatch.Groups[1].Value;

            int startIndex = dirtyData.IndexOf("H4sIA");
            if (startIndex == -1) throw new ArgumentException("格式错误: 未找到核心数据 'H4sIA'。");

            string dataWithTail = dirtyData.Substring(startIndex);
            string cleanedBase64 = Regex.Replace(dataWithTail, @"[^A-Za-z0-9\+/=]", "");
            string pureBase64 = cleanedBase64.TrimEnd('=');

            int padding = pureBase64.Length % 4;
            if (padding > 0) pureBase64 += new string('=', 4 - padding);

            byte[] compressedBytes = Convert.FromBase64String(pureBase64);

            using (var compressedStream = new MemoryStream(compressedBytes))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}