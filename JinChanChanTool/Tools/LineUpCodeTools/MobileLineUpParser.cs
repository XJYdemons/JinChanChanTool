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
        private static readonly Dictionary<int, int> MobileToPcIdMap = new Dictionary<int, int>
        {
            // --- 1费 ---
            { 1, 10357 },   // 亚托克斯
            { 9, 10372 },   // 纳尔
            { 13, 10371 },  // 盖伦
            { 29, 10402 },  // 芮尔
            { 33, 10396 },  // 纳亚菲利
            { 37, 10394 },  // 墨菲特
            { 45, 10419 },  // 辛德拉
            { 49, 10391 },  // 卢锡安
            { 53, 10384 },  // 凯南
            { 61, 10380 },  // 卡莉斯塔
            { 65, 10409 },  // 希维尔
            { 69, 10368 },  // 伊泽瑞尔
            { 73, 10383 },  // 凯尔

            // --- 2费 ---
            { 21, 10434 },  // 扎克
            { 85, 10370 },  // 普朗克
            { 89, 10408 },  // 慎
            { 97, 10385 },  // 可酷伯
            { 101, 10399 }, // 洛
            { 105, 10430 }, // 赵信
            { 109, 10425 }, // 蔚
            { 113, 10366 }, // 蒙多
            { 121, 10379 }, // 卡莎
            { 125, 10382 }, // 卡特琳娜
            { 129, 10429 }, // 霞
            { 133, 10377 }, // 烬
            { 137, 10393 }, // 拉克丝
            { 141, 10374 }, // 迦娜
            
            // --- 3费 ---
            { 149, 10426 }, // 佛耶戈
            { 153, 10423 }, // 乌迪尔
            { 169, 10413 }, // 斯维因
            { 177, 10431 }, // 亚索
            { 181, 10376 }, // 杰斯
            { 185, 10397 }, // 妮蔻
            { 193, 10358 }, // 阿狸
            { 197, 10405 }, // 塞纳
            { 201, 10435 }, // 吉格斯
            { 205, 10362 }, // 凯特琳
            { 209, 10365 }, // 德莱厄斯
            { 213, 10395 }, // 玛尔扎哈
            { 217, 10392 }, // 璐璐
            { 221, 10386 }, // 克格莫
            { 225, 10400 }, // 拉莫斯
            { 229, 10410 }, // 斯莫德

            // --- 4费 ---
            { 233, 10390 }, // 蕾欧娜
            { 237, 10407 }, // 瑟提
            { 241, 10359 }, // 阿卡丽
            { 245, 10398 }, // 波比
            { 249, 10375 }, // 嘉文四世
            { 253, 10427 }, // 沃利贝尔
            { 257, 10387 }, // 奎桑提
            { 261, 10360 }, // 艾希
            { 265, 10404 }, // 莎弥拉
            { 269, 10403 }, // 瑞兹
            { 273, 10381 }, // 卡尔玛
            { 277, 10433 }, // 悠米
            { 281, 10378 }, // 金克丝

            // --- 5费 ---
            { 285, 10361 }, // 布隆
            { 289, 10373 }, // 格温
            { 293, 10432 }, // 永恩
            { 297, 10422 }, // 崔斯特
            { 301, 10406 }, // 萨勒芬妮
            { 305, 10436 }, // 婕拉
            { 309, 10424 }, // 韦鲁斯
            { 313, 10388 }, // 李青 (裁决)
            { 317, 10388 }, // 李青 (决斗)
            { 321, 10388 }, // 李青 (主宰)
            { 381, 10388 }  // 李青 (普通)
        };

        public static List<int> Parse(string fullCode)
        {
            byte[] decompressedBytes = DecompressCode(fullCode);
            string dataString = Encoding.UTF8.GetString(decompressedBytes);

            var pcHeroIds = new List<int>();

            // 1. 读取英雄数量 (第31-32位)
            int heroCount = int.Parse(dataString.Substring(30, 2)); // 位置从0开始，所以是30
            if (heroCount == 0) return pcHeroIds;

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
                foreach (var entry in MobileToPcIdMap)
                {
                    int baseId = entry.Key;
                    // 检查动态ID是否在 [基础ID, 基础ID+2] 的范围内
                    if (dynamicId >= baseId && dynamicId <= baseId + 2)
                    {
                        pcHeroIds.Add(entry.Value);
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

            return pcHeroIds.Distinct().ToList();
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