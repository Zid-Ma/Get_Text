
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Get_Text
{
	internal class CaptureScreen
	{
		//为屏幕创建一个设备上下文环境，屏幕名为“display”
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszoutput, IntPtr lpdate);
		//指定的源设备环境区域中的像素进行位块（bit_block）转换，以传送到目标设备环境
		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hdcDest, int x, int y, int widht, int hight, IntPtr hdcsrc, int xsrc, int ysrc, System.Int32 dw);
		//		hDestDC：指向目标设备环境的句柄。
		//x：指定目标矩形区域左上角的X轴逻辑坐标。
		//y：指定目标矩形区域左上角的Y轴逻辑坐标。
		//nWidth：指定源在目标矩形区域的逻辑宽度。
		//nHeight：指定源在目标矩形区域的逻辑高度。
		//hSrcDC：指向源设备环境的句柄。
		//xSrc：指定源矩形区域左上角的X轴逻辑坐标。
		//ySrc：指定源矩形区域左上角的Y轴逻辑坐标。
		//dwRop：指定光栅操作代码。这些代码将定义源矩形区域的颜色数据，如何与目标矩形区域的颜色数据组合以完成最后的颜色。

		//创建屏幕图像
		public static bool CreatePicture(int _x, int _y, int _width,int _height,string _path)
		{
			IntPtr dc1 = CreateDC("display", null, null, (IntPtr)null);
			Graphics g1 = Graphics.FromHdc(dc1);
			//MessageBox.Show(_width + "  " + _height.ToString());
			Bitmap my = new Bitmap(_width, _height, g1);//位图
			Graphics g2 = Graphics.FromImage(my);
			IntPtr dc3 = g1.GetHdc();
			IntPtr dc2 = g2.GetHdc();
			BitBlt(dc2, 0, 0, _width, _height, dc3, _x, _y, 13369376);
			g1.ReleaseHdc(dc3);
			g2.ReleaseHdc(dc2);
			g1.Dispose();
			g2.Dispose();
			my.Save(_path);
			my.Dispose();
			GC.Collect();
			return true;
		}
		//获取屏幕图像
		public static Bitmap GetPic(int _x, int _y, int _width, int _height)
		{
            IntPtr dc1 = CreateDC("display", null, null, (IntPtr)null);
            Graphics g1 = Graphics.FromHdc(dc1);
            //MessageBox.Show(_width + "  " + _height.ToString());
            Bitmap my = new Bitmap(_width, _height, g1);//位图
            Graphics g2 = Graphics.FromImage(my);
            IntPtr dc3 = g1.GetHdc();
            IntPtr dc2 = g2.GetHdc();
            BitBlt(dc2, 0, 0, _width, _height, dc3, _x, _y, 13369376);
            g1.ReleaseHdc(dc3);
            g2.ReleaseHdc(dc2);
            g1.Dispose();
            g2.Dispose();
            //my.Save(_path);
            //my.Dispose();
            GC.Collect();
            return my;
        }
		public static bool CreatePicture(int _x, int _y, int _width, int _height, string _path, ScreenShoot _window)
		{
			IntPtr dc1 = CreateDC("display", null, null, (IntPtr)null);
			Graphics g1 = Graphics.FromHdc(dc1);
			//MessageBox.Show(_width + "  " + _height.ToString());
			Bitmap my = new Bitmap(_width, _height, g1);//位图
			Graphics g2 = Graphics.FromImage(my);
			IntPtr dc3 = g1.GetHdc();
			IntPtr dc2 = g2.GetHdc();
			BitBlt(dc2, 0, 0, _width, _height, dc3, _x, _y, 13369376);
			g1.ReleaseHdc(dc3);
			g2.ReleaseHdc(dc2);
			//System.Windows.Media.Imaging.BitmapImage imgSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("location", UriKind.Relative));
			//_window.image.Source = (System.Windows.Media.Imaging.BitmapImage)my;
			my.Save(_path);
			my.Dispose();
			GC.Collect();
			return true;
		}
	}
}
