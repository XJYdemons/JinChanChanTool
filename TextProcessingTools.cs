
using Emgu.CV.Dnn;

namespace 金铲铲助手
{
    internal class TextProcessingTools
    {

        /// <summary>
        /// 获取TextBox中以某个固定符号分隔的文本数组
        /// </summary>
        /// <param name="textbox"></param>
        /// <returns></returns>
        public static string[] GetTextArray(TextBox textbox)
        {
            string 用户输入 = textbox.Text;  // 从 TextBox 获取用户输入
            string[] 数组 = 用户输入.Split('|');  // 以 | 分隔符分割字符串并存储到数组中
            return 数组;
        }


        /// <summary>
        /// 判断识别结果与异常突变名称的相似度
        /// </summary>
        /// <param name="识别结果"></param>
        /// <param name="异常突变名称"></param>
        /// <returns></returns>
        public static bool DetermineSimilarityLevenshtein_MutationName(string 识别结果, string 异常突变名称)
        {

            if (string.IsNullOrWhiteSpace(识别结果) || string.IsNullOrWhiteSpace(异常突变名称)||异常突变名称==""||识别结果=="")
            {
                return false;
            }
            string shortString = (异常突变名称.Length >= 识别结果.Length) ? 识别结果 : 异常突变名称;
            string longString = (异常突变名称.Length >= 识别结果.Length) ? 异常突变名称 : 识别结果;
            if(longString.Contains(shortString))
            {
                return true;    
            }
            // 检查目标字符串中有多少内容出现在结果字符串中
            int matchCount = 0;
            foreach (char c in shortString)
            {
                if (longString.Contains(c))
                {
                    matchCount++;
                }
            }
            // 计算匹配率
            double matchRate = (double)matchCount / shortString.Length;

            // 如果匹配率大于或等于阈值，判定为匹配
            return matchRate >= 0.6;

        }


        /// <summary>
        /// 使用 Levenshtein 距离来判断相似度
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static int LevenshteinDistance(string A, string B)
        {

            int[,] dp = new int[A.Length + 1, B.Length + 1];

            // 初始化dp数组
            for (int i = 0; i <= A.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= B.Length; j++) dp[0, j] = j;

            // 填充dp数组
            for (int i = 1; i <= A.Length; i++)
            {
                for (int j = 1; j <= B.Length; j++)
                {
                    int cost = A[i - 1] == B[j - 1] ? 0 : 1;
                    dp[i, j] = Math.Min(Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1), dp[i - 1, j - 1] + cost);
                }
            }

            return dp[A.Length, B.Length];
        }
        /// <summary>
        /// 判断识别结果与奕子名称的相似度
        /// </summary>
        /// <param name="识别结果"></param>
        /// <param name="奕子名称"></param>
        /// <returns></returns>
        public static bool DetermineSimilarityLevenshtein_CardName(string 识别结果, string 奕子名称)
        {

            if (string.IsNullOrWhiteSpace(识别结果) || string.IsNullOrWhiteSpace(奕子名称))
            {
                return false;
            }
           
            int distance = LevenshteinDistance(识别结果, 奕子名称);
                  
                      
            // 判断Levenshtein距离是否小于最大长度的40%（例如：60%以上的相似度）
            return (float)(奕子名称.Length - distance) / 奕子名称.Length >= 0.6;
        }
        public static string ConvertResult_CardName(string result)
        {
            // 去除空格和问号
           result = result.Replace(" ", "").Replace("?", "");
            switch (result)
            {
                case "梦欧娜":
                case "套欧娜":
                case "营欧娜":
                    return "蕾欧娜";
                case "卡宝尔":
                case "卡室尔":
                    return "卡密尔";
                case "克格奠":
                    return "克格莫";
                case "桂妮":
                case "崔妮":
                case "茬妮":
                    return "荏妮";
                case "前":
                case "前一":
                    return "蔚";
                case "奠德凯激":
                case "美德凯撒":
                case "美德凯激":
                case "美德凯摄":
                    return "莫德凯撒";
                case "国奇":
                    return "图奇";
                case "奠甘娜":
                    return "莫甘娜";
                case "黑联丁格":
                case "黑跃丁格":
                    return "黑默丁格";
                case "度腾":
                    return "魔腾";
               

            }
            return result;
        }
        public static string ConvertResult_Mutation(string result)
        {
            // 去除空格和问号
            result = result.Replace(" ", "").Replace("?", "");
            switch (result)
            {
                case "小我多多女主力,":
                case "小我多多女主力，":
                    return "小我多多";
                case "特强凌弱":
                    return "恃强凌弱";
                case "地下拳王致主力，":
                case "地下拳王致主力,":
                    return "地下拳王";
                case "窗法训练":
                    return "魔法训练";
            }
            return result;
        }

    }

}
