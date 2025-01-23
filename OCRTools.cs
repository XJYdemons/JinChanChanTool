using System.Text.RegularExpressions;
using PaddleOCRSharp;
namespace 金铲铲助手
{
    public static class OCRTools
    {
        
      static   PaddleOCRSharp.OCRModelConfig config = new PaddleOCRSharp.OCRModelConfig()
      {
          det_infer = @"inference\ch_PP-OCRv4_det_infer",
          cls_infer = @"inference\ch_ppocr_mobile_v2.0_cls_infer",
          rec_infer = @"inference\ch_PP-OCRv4_rec_infer",
          //keys = @"inference\ppocr_keys_order.txt"
          keys = @"inference\ppocr_keys.txt"
      };
        //使用默认参数
        static PaddleOCRSharp.OCRParameter oCRParameter = new PaddleOCRSharp.OCRParameter();
        //识别结果对象
      static   PaddleOCRSharp.OCRResult ocrResult = new PaddleOCRSharp.OCRResult();
        
      static   PaddleOCRSharp.PaddleOCREngine engine = new PaddleOCRSharp.PaddleOCREngine(config, oCRParameter);



        //static PaddleOCRSharp.OCRModelConfig config1 = new PaddleOCRSharp.OCRModelConfig()
        //{
        //    det_infer = @"inference\ch_PP-OCRv4_det_infer",
        //    cls_infer= @"inference\ch_ppocr_mobile_v2.0_cls_infer",
        //    rec_infer= @"inference\ch_PP-OCRv4_rec_infer",
        //    keys= @"inference\ppocr_keys_order.txt"
        //};

        //static PaddleOCRSharp.OCRParameter oCRParameter1 = new PaddleOCRSharp.OCRParameter();
      

        ////识别结果对象
        //static PaddleOCRSharp.OCRResult ocrResult1 = new PaddleOCRSharp.OCRResult();

        //static PaddleOCRSharp.PaddleOCREngine engine1 = new PaddleOCREngine(config1, oCRParameter1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="模式">0:识别奕子名称的算法；1：识别异常突变的算法</param>
        public static string OCRRecognition(int 模式, Bitmap image)
        {
            switch (模式)
            {
                case 0:
                    {
                        ocrResult = engine.DetectText(image);
                       
                       return ocrResult.ToString(); 
                    }

                case 1:
                    {



                        //ocrResult1 = engine1.DetectText(image);
                        //return ocrResult1.ToString();
                        return "";
                    }
                default:
                    {
                        return "未识别";

                    }

            }

        }
       
        /// <summary>
        /// OCR识别，仅输出数字，识别金币专用
        /// </summary>
        /// <param name="模式"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string OCRRecognitionForFigure(int 模式,Bitmap image)
        {
            switch (模式)
            {
                case 0:
                    {
                        ocrResult = engine.DetectText(image);
                       

                        if (ocrResult != null)
                        {
                            // 使用正则表达式仅提取数字
                            string digitsOnly = Regex.Replace(ocrResult.Text, "[^0-9]", "");
                            return digitsOnly.ToString();
                        }
                        else
                        {
                            return "";
                        }
                       
                    }

                case 1:
                    {

                        //ocrResult1 = engine1.DetectText(image);

                        //if (ocrResult1 != null)
                        //{
                        //    // 使用正则表达式仅提取数字
                        //    string digitsOnly = Regex.Replace(ocrResult1.Text, "[^0-9]", "");
                        //    return digitsOnly.ToString();
                        //}
                        //else
                        //{
                        //    return "";
                        //}
                        return "";
                    }
                default:
                    {
                        return "未识别";

                    }

            }

        }

    
    }
}
