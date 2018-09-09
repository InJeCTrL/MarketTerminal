using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;
using AForge.Controls;
using ZXing;
using System.Drawing;

namespace Market
{
    /// <summary> 调用摄像头，识别BarCode，联网查询BarCode对应商品信息
    /// </summary>
    class CheckGoods
    {
        /// <summary> 输入视频设备列表
        /// </summary>
        private FilterInfoCollection VideoDevices;
        /// <summary> 用于获取输入的视频设备
        /// </summary>
        private VideoCaptureDevice VideoSource;
        /// <summary> 读取条码
        /// </summary>
        private BarcodeReader CodeReader;
        /// <summary> 初始化商品扫描，初始化条码扫描类
        /// </summary>
        public CheckGoods()
        {
            CodeReader = new BarcodeReader();//初始化条码扫描类
            CodeReader.Options.CharacterSet = "UTF-8";//设置条码字符集类型为UTF-8
        }
        /// <summary> 检查视频输入设备是否正常
        /// </summary>
        /// <returns>返回 true：正常找到 false：异常</returns>
        public Boolean CheckVideoDevice()
        {
            VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);//枚举所有视频输入设备
            if (VideoDevices.Count == 0)//未找到可用的视频输入设备
                return false;//返回false
            else
                return true;//返回true
        }
        /// <summary> 设置视频输入源
        /// </summary>
        public void SetVideoSource()
        {
            VideoSource = new VideoCaptureDevice(VideoDevices[0].MonikerString);//获取视频输入源，默认连接第一个输入设备
            VideoSource.VideoResolution = VideoSource.VideoCapabilities[0];//指定视频输出配置
        }
        /// <summary> 获得视频输入源
        /// </summary>
        /// <returns>返回 视频输入源</returns>
        public VideoCaptureDevice GetVideoSource()
        {
            return VideoSource;//返回视频输入源
        }
        /// <summary> 检查图片中存在的商品条码
        /// </summary>
        /// <param name="ScreenShot">摄像头截图</param>
        /// <returns>返回 String：条码值 null：图片中不存在条码</returns>
        public String CheckBarCode(Bitmap ScreenShot)
        {
            Result barcode = CodeReader.Decode(ScreenShot);//使用BarCodeReader解码
            if (barcode == null)
                return null;//若条码为空则返回空
            return barcode.Text;//返回条码对应字符串
        }
    }
}
