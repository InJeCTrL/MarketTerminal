using System;
using System.Text;
using AForge.Video.DirectShow;
using AForge.Controls;
using ZXing;
using System.Drawing;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

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
        /// <param name="vSource">视频输入控件</param>
        public void SetVideoSource(VideoSourcePlayer vSource)
        {
            VideoSource = new VideoCaptureDevice(VideoDevices[0].MonikerString);//获取视频输入源，默认连接第一个输入设备
            VideoSource.VideoResolution = VideoSource.VideoCapabilities[0];//指定视频输出配置
            vSource.VideoSource = VideoSource;//将收银窗体中视频控件与视频输入设备关联
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
        /// <summary> 启动摄像头
        /// </summary>
        /// <param name="vSource">摄像头显示控件</param>
        public void Start(VideoSourcePlayer vSource)
        {
            vSource.Start();//启动摄像头
        }
        /// <summary> 关闭摄像头
        /// </summary>
        /// <param name="vSource">摄像头显示控件</param>
        public void Stop(VideoSourcePlayer vSource)
        {
            vSource.Stop();//关闭摄像头
        }
        /// <summary> 获取商品信息集
        /// </summary>
        /// <param name="_GoodsNo"></param>
        /// <returns>返回 String[]：成功 null：失败</returns>
        public String[] GetGoodsInfo(String _GoodsNo)
        {
            CookieContainer Cookie = GetCookie("http://search.anccnet.com/writeSession.aspx?responseResult=check_ok");//尝试获取Cookie
            if (Cookie == null)
                return null;
            else
            {
                String PageText = GetPageText(@"http://search.anccnet.com/searchResult2.aspx?keyword=" + _GoodsNo, "http://search.anccnet.com", Cookie);//尝试获得商品简略信息网页源代码
                if (PageText == null)//获取失败
                    return new String[] { _GoodsNo, "", "", "", "", "", "", "" };
                else
                {
                    PageText = Regex.Replace(PageText, @"\s", "");//去空格
                    String RegStr = @"<dt>商标：</dt><dd>(\S+?)</dd><dt>\S+<dt>名称：</dt><dd>(\S+?)</dd><dt>";//匹配商品名称
                    Match MatchResult = Regex.Match(PageText, RegStr);//尝试匹配
                    return new String[] { _GoodsNo, MatchResult.Groups[2].Value, "", "", "", "", MatchResult.Groups[1].Value, "" };
                }
            }
        }
        /// <summary> 获取指定网页源代码
        /// </summary>
        /// <param name="TargetURL">目标网页地址</param>
        /// <param name="Referer">伪造的先前页面</param>
        /// /// <param name="Referer">伪造的Cookie</param>
        /// <returns>返回 String：网页源代码 null：失败</returns>
        private String GetPageText(String TargetURL,String Referer,CookieContainer Cookie)
        {
            try
            {
                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(TargetURL);//确认请求地址
                Request.Method = "GET";//GET方法获得网页内容
                Request.Referer = Referer;//伪造referer绕过验证
                Request.CookieContainer = Cookie;//伪造Cookie绕过验证
                Request.Timeout = 5000;//超时时间5秒
                Request.ReadWriteTimeout = 5000;//读写超时时间5秒
                HttpWebResponse PageInfo = (HttpWebResponse)Request.GetResponse();//获取网页信息
                StreamReader Reader = new StreamReader(PageInfo.GetResponseStream(), Encoding.Default);//获取网页信息到流中
                String PageText = Reader.ReadToEnd();//流读入字符串中
                Reader.Close();//关闭流
                return PageText;//返回网页源代码
            }
            catch (Exception)
            {
                return null;//获取失败
            }
        }
        /// <summary> 获取Cookie
        /// </summary>
        /// <param name="TargetURL">目标地址</param>
        /// <returns>返回 Cookie：成功 null：失败</returns>
        private CookieContainer GetCookie(String TargetURL)
        {
            try
            {
                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(TargetURL);//指定目标网址
                CookieContainer Cookie = new CookieContainer();//初始化CookieContainer
                Request.CookieContainer = Cookie;//请求Cookie
                Request.Timeout = 5000;//超时时间5秒
                Request.ReadWriteTimeout = 5000;//读写超时时间5秒
                HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();//请求
                Stream Reader = Response.GetResponseStream();
                Reader.Close();//关闭流
                return Cookie;//返回Cookie
            }
            catch (Exception)
            {
                return null;//失败
            }
        }
    }
}
