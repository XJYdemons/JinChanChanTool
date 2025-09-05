using System.Text.RegularExpressions;
using PaddleOCRSharp;

namespace JinChanChanTool.Tools.OCRTools
{
    public static class CPU_OCRTool
    {        
        /// <summary>
        /// OCR设置对象
        /// </summary>
      static OCRModelConfig config = new OCRModelConfig()
      {
          det_infer = Path.Combine(Application.StartupPath, "inference", "ch_PP-OCRv4_det_infer"), 
          cls_infer = Path.Combine(Application.StartupPath,"inference", "ch_ppocr_mobile_v2.0_cls_infer"),
          rec_infer = Path.Combine(Application.StartupPath, "inference", "ch_PP-OCRv4_rec_infer"),         
          keys = Path.Combine(Application.StartupPath, "inference", "ppocr_keys.txt")
      };

        /// <summary>
        /// OCR参数对象，默认参数
        /// </summary>
      static OCRParameter oCRParameter = new OCRParameter();

        /// <summary>
        /// OCR识别结果对象
        /// </summary>
      static OCRResult ocrResult = new OCRResult();
        
        /// <summary>
        /// OCR识别引擎
        /// </summary>
      static PaddleOCREngine engine = new PaddleOCREngine(config, oCRParameter);

        /// <summary>
        /// OCR识别
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>        
        public static string OCRRecognition(Bitmap image)
        {         
            ocrResult = engine.DetectText(image);                       
            return ocrResult.ToString();                          
        }

        /// <summary>
        /// OCR识别，仅输出数字，识别金币专用。
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string OCRRecognitionForFigure(Bitmap image)
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
    }
}
