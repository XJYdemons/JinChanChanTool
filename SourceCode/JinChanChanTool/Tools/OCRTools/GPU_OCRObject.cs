using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing.Imaging;
using Windows.Media.Capture;

namespace JinChanChanTool.Tools.OCRTools
{
    public class GPU_OCRObject
    {
        //private FullOcrModel _model;
        //private PaddleOcrAll _ocr;
        //public PaddleOcrAll OcrInstance => _ocr;
        
        //public GPU_OCRObject(int version)
        //{
        //    FileDetectionModel detectionModel;
        //    FileRecognizationModel recognizaionModel;
        //     // 添加这个属性以暴露内部OCR实例
    
        //    switch (version)
        //    {               
        //        case 4:
        //             detectionModel = new FileDetectionModel(Path.Combine(Application.StartupPath, "Resources", "Models", "ch_PP-OCRv4_det_infer"), ModelVersion.V4);
        //            recognizaionModel = new FileRecognizationModel(Path.Combine(Application.StartupPath, "Resources", "Models", "ch_PP-OCRv4_rec_infer"), Path.Combine(Application.StartupPath, "Resources", "Models", "ppocr_keys.txt"), ModelVersion.V4);
        //            break;
        //        case 5: 
        //             detectionModel = new FileDetectionModel(Path.Combine(Application.StartupPath, "Resources", "Models", "PP-OCRv5_mobile_det_infer"), ModelVersion.V5);
        //             recognizaionModel = new FileRecognizationModel(Path.Combine(Application.StartupPath, "Resources", "Models", "PP-OCRv5_mobile_rec_infer"), Path.Combine(Application.StartupPath, "Resources", "Models", "ppocr_keys_v5.txt"), ModelVersion.V5);
        //            break;
        //        default:
        //        detectionModel = new FileDetectionModel(Path.Combine(Application.StartupPath, "Resources", "Models", "ch_PP-OCRv4_det_infer"), ModelVersion.V4);
        //        recognizaionModel = new FileRecognizationModel(Path.Combine(Application.StartupPath, "Resources", "Models", "ch_PP-OCRv4_rec_infer"), Path.Combine(Application.StartupPath, "Resources", "Models", "ppocr_keys.txt"), ModelVersion.V4);
        //        break;
        //    }
        //    _model = new FullOcrModel(detectionModel, recognizaionModel);
        //    // 创建OCR引擎
        //    _ocr = new PaddleOcrAll(_model, PaddleDevice.Gpu());
        //    _ocr.AllowRotateDetection = true;
        //}

        //public string RecognizeBitmap(Bitmap bitmap)
        //{
        //    using(Mat src = bitmap.ToMat())
        //    {
        //        PaddleOcrResult result = _ocr.Run(src);
        //        return result.Text;
        //    }
        //}
        //public void Dispose()
        //{
        //    _ocr?.Dispose();
        //}
    }
}
