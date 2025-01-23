

namespace 金铲铲助手
{
    internal class AppConfig
    {
        public required string HotKey1 { get; set; }
        public required string HotKey2 { get; set; }
        public required string HotKey3 { get; set; }
        public int intervalTime_GetCard { get; set; }
        public int intervalTime_StoreRefresh { get; set; }
        public int intervalTime_RefreshStore { get; set; }

        public int countOfMoney_StartRefreshStore { get; set; }
        public int countOfMoney_StopRefreshStore { get; set; }
       
        public int startPoint_CardScreenshotX1 { get; set; }
        public int startPoint_CardScreenshotX2 { get; set; }
        public int startPoint_CardScreenshotX3 { get; set; }
        public int startPoint_CardScreenshotX4 { get; set; }
        public int startPoint_CardScreenshotX6 { get; set; }
        public int startPoint_CardScreenshotY6 { get; set; }
        public int width_CardScreenshot { get; set; }
        public int height_CardScreenshot { get; set; }
        public int startPoint_MoneyScreenshotX { get; set; }
        public int startPoint_MoneyScreenshotY { get; set; }
        public int width_MoneyScreenshot { get; set; }
        public int height_MoneyScreenshot { get; set; }
        public int startPoint_MutationScreenshotX { get; set; }
        public int startPoint_MutationScreenshotY { get; set; }
        public int width_MutationScreenshot { get; set; }
        public int height_MutationScreenshot { get; set; }
        public int Point_RefreshMutationX { get; set; }
        public int Point_RefreshMutationY { get; set; }
        public int Point_EvolutionMutationX { get; set; }
        public int Point_EvolutionMutationY { get; set; }
        public int Point_RefreshStoreX { get; set; }
        public int Point_RefreshStoreY { get; set; }
    }
}
