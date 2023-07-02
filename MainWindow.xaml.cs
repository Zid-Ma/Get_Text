using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using Get_Text;

namespace Get_Text
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string s;
        static string message = " ";
        public static bool active = false;
        public static int translate_mode = 0;
        public static int online_mode = 0;
        /// <summary>
        /// 引入Timer控件
        /// </summary>
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer timer_key = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            if (false)
            {
                CaptureAudio captureAudio = new CaptureAudio();
                captureAudio.GetAudioDevices();
                captureAudio.StartAudioData(1);
            }
            Data_Save.Initialization();
            {
                //更新窗体控件尺寸
                {
                    this.Height = 180;
                    this.Width = 300;
                }
                //更新子窗体位置
                if (true)
                {
                    window_Menu.Show();
                    //Mouses.POINT point;
                    //Mouses.GetCursorPos(out point);
                    //window_Menu.Left = point.X;
                    //window_Menu.Top = point.Y;
                    window_Menu.Left = this.Left + 10;
                    window_Menu.Top = this.Top + 30 + 22;
                    window_Menu.Hide();
                    screenShoot.Show();
                    screenShoot.Left = this.Left + 10;
                    screenShoot.Top = this.Top + 30 + 22;
                    screenShoot.Hide();
                }
            }
            screenShoot.mainWindow = this;
            this.Label_VVV.Visibility = Visibility.Hidden;
            s = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            //       //创建一个任务,并执行
            //       Task a = Task.Run(() =>
            //       {
            //           String appStartupPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            //           try
            //           {
            //               bool _c1 = C1.First(0, appStartupPath + "/Message.txt");
            //               if(!_c1)
            //{
            //                   MessageBox.Show("初始化dll失败!");
            //}
            //           }
            //           catch
            //           {
            //               //C1.First(0, appStartupPath + "/Message.txt");
            //               MessageBox.Show("Error");
            //           }
            //           MessageBox.Show("新建的线程结束");
            //       });
            //hook_keyboard.SetHook();
            hook_keyboard.hookProc1 = new HookProc(Hook_Keyboard);
            hook_keyboard.SetHook(hook_keyboard.hookProc1, HookType.Keyboard);
            timer.Tick += new EventHandler(timer_Tick);
            //设置刷新的间隔时间    
            timer.Interval = TimeSpan.FromSeconds(1);   //TimeSpan.FromSeconds(0.1)
            timer.Start();
            timer_key.Tick += new EventHandler(timer_key_Tick);
            //设置刷新的间隔时间    
            timer_key.Interval = TimeSpan.FromSeconds(0.1);   //TimeSpan.FromSeconds(0.1)
            timer_key.Start();
        }
        int iKeyDown = 0;
        bool IsKeyDown_Ctrl = false;
        bool IsKeyDown_Alt = false;
        bool IsKeyDown_X = false;
        void timer_key_Tick(object sender, EventArgs e)
        {
            if(IsKeyDown_Ctrl && IsKeyDown_Alt && IsKeyDown_X)
			{
                //MessageBox.Show("键盘X");
                Shoot();
                IsKeyDown_Ctrl = IsKeyDown_Alt = IsKeyDown_X = false;
			}
            if(IsKeyDown_Ctrl || IsKeyDown_Alt || IsKeyDown_X)
			{
                iKeyDown++;
                if (iKeyDown > 5)
                {
                    iKeyDown = 0;
                    IsKeyDown_Ctrl = IsKeyDown_Alt = IsKeyDown_X = false;
                }
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.X))
            {
                Shoot();
            }
        }
        int iL = 0;
        void Shoot()
		{
            if (CaptureScreen.CreatePicture(0, 0, (int)screenShoot.screenWidth, (int)screenShoot.screenHeight, Data_Save.getDocumentPath() + "Data/临时图像" + iL + ".png"))
            {
                if (screenShoot.task_ReV == null)
                {
                    screenShoot.image.Source = new BitmapImage(new Uri(Data_Save.getDocumentPath() + "Data/临时图像" + iL + ".png"));// UriKind.Relative
                    screenShoot.Window_Visible();
                    iL++;
                    if (iL > 10)
                        iL = 0;
                }
				else
				{
                    TextBoxLeft.Text += "\r\n尚在进行迭代更新中，请等待";
				}
            }
        }
        public static Hook_XYKS hook_keyboard = new Hook_XYKS();
        //委托回调的方法
        IntPtr Hook_Keyboard(int code, IntPtr wparam, IntPtr Iparam)
        {
            //MessageBox.Show("键盘:" + wparam.ToInt32());
            if (code < 0)
            {
                //让后面的程序处理该消息
                return Hook_XYKS.CallNextHookEx(hook_keyboard._nextHookPtr1, code, wparam, Iparam);
            }
            if(screenShoot.Visibility == Visibility.Visible)
			{
                //让后面的程序处理该消息
                return IntPtr.Zero;
            }
            //用户输入的是x或者X
            if (wparam.ToInt32() == 17 )
			{
                //MessageBox.Show("键盘:" + wparam.ToInt32());
                IsKeyDown_Ctrl = true;
                return (IntPtr)1;
            }
             else if (wparam.ToInt32() == 18)
			{
                //MessageBox.Show("键盘:" + wparam.ToInt32());
                IsKeyDown_Alt = true;
                return (IntPtr)1;
            }
            else if(wparam.ToInt32() == 88 || wparam.ToInt32() == 125)//(wparam.ToInt32() == 98 || wparam.ToInt32() == 66)
            {
                //MessageBox.Show("键盘:" + wparam.ToInt32());
                IsKeyDown_X = true;
                return Hook_XYKS.CallNextHookEx(hook_keyboard._nextHookPtr1, code, wparam, Iparam);
                //该消息结束
                return (IntPtr)1;
            }
            else
            {
                //让后面的程序处理该消息
                return IntPtr.Zero;
            }
            //return Hook_XYKS.CallNextHookEx(hook_keyboard._nextHookPtr1, code, wparam, Iparam);
        }
        void timer_Tick(object sender, EventArgs e)
        {
            HeartBeat heart = new HeartBeat();
            heart.OneBeat();

            Last_IsVisible = window_Menu.IsVisible;
            if (!active)
                return;
            if(translate_mode == 0)
			{
                Local_Translate();
            }
            else if (translate_mode == 1)
            {
                //String appStartupPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                //C1.First(0, appStartupPath + "/Message.txt");
                Online_Translate();
            }
			else if(translate_mode == 2)
			{
                Online_Translate();
     
            }
        }

        void Local_Translate()
		{
            string message = Clipboard.GetText();
            if (message.Length > YouDaoAPI.MaxLength)
                return;
            if (message != TextBoxLeft.Text && message != " ")
            {
                TextBoxLeft.Text = message;
                TextBoxRight.Text = Data_Save.ReadWord(message);
            }
        }

        void Online_Translate()
		{
            switch(online_mode)
			{
                case 0:
                    YouDao_Translate();
                    break;
			}
		}

        void YouDao_Translate()
		{
            try
            {
                //更新文本框内容
                //TextBoxLeft.Text = C1.ClipBoard_Get();
                //string s = Read("Message.txt");
                string message = Clipboard.GetText(); //C1.ClipBoard_Get_A(); 
                //message.Replace(" ", "");
                if (message.Length > YouDaoAPI.MaxLength)
                    return;
                //MessageBox.Show(message);
                //返回翻译结果 tranlation
                if (message != TextBoxLeft.Text && message != " ")
                {
                    TextBoxLeft.Text = message;
                    try
                    {
                        //是否存入数据到硬盘
                        bool isLocalSave = true;
                        //清除空内容与换行
                        string ss = YouDaoAPI.Find(message.Replace("\r\n", string.Empty));
                        TextBoxRight.Text = ss;
                        if(ss == "Error")
                        {
                            isLocalSave = false;
                            Local_Translate();
                            TextBoxRight.Text = "<Error-YouDao_Translate>\r\n" + TextBoxRight.Text;
                        }
                        else if (ss != "")
                        {
                            //获取转换后的值（译文）
                            string trans = Readjson_Mess(ss, "translation");
                            //TextBoxRight.Text += "\r\n语音： " + Readjson_Mess(ss, "tspeakUrl") + "\r\n";//speakUrl源语言发音地址 //tspeakUrl翻译结果发音地址
                            try
                            {
                                string sss = Readjson_Mess(ss, "basic");
                                if(trans == "Error")// sss == "Error"
                                {
                                    isLocalSave = false;
                                    Local_Translate();
                                    TextBoxRight.Text = "<Error-Readjson_Mess>\r\n" + ss + "\r\n" + TextBoxRight.Text;
                                }
                                else
                                {
                                    TextBoxRight.Text = trans;
                                    isLocalSave = true;
                                    for (int a = 0; a < sss.Length; a++)
                                    {
                                        if (sss.Substring(a, 2) == "],")
                                        {
                                            string ssss = sss.Substring(a + 1);
                                            sss = ssss;
                                            TextBoxRight.Text += sss;
                                        }
                                    }
                                    //TextBoxRight.AppendText(Readjson_Mess(ss, "basic"));
                                    TextBoxRight.AppendText("\r\n" + Readjson_Mess(ss, "web"));
                                }
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            isLocalSave = false;
                            Local_Translate();
                            TextBoxRight.Text = "<Local_Translate>\r\n" + TextBoxRight.Text;
                        }
                        if (isLocalSave)
                            Local_InputWords(true);
                    }
                    catch
                    {
                        Local_Translate();
                        TextBoxRight.Text = "<Error-YouDao_Translate>\r\n" + TextBoxRight.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        static string LastL = "", LastR = ""; 
        /// <summary>
        /// 将数据存放到本地硬盘
        /// </summary>
        /// <param name="IsInputMode">是否清除文本框内容</param>
        void Local_InputWords(bool IsInputMode)
		{
            if (TextBoxLeft.Text.Length > 50)
                return;
            string l = TextBoxLeft.Text;
            string r = TextBoxRight.Text;
            if (LastL == l && LastR == r)
                return;
            string ss = Data_Save.Register(l, r, DateTime.Now.ToString());
            if (!IsInputMode)
            {
                TextBoxLeft.Text = "";
                TextBoxRight.Text = "";
            }
            if(ss == "false")
            {
                TextBoxRight.Text = "<Error-Data_Save.Register>\r\n" + TextBoxRight.Text;
            }
            LastL = l;
            LastR = r;
        }

        /// <summary>
        /// 保存文件,储存地址为应用程序下
        /// </summary>
        /// <param name="Path">相对路径</param>
        /// <param name="information">需要存储的信息</param>
        public void Save(string Path, string information)
        {
            try
            {
                //FileStream aFile = new FileStream(@"" + Path, FileMode.OpenOrCreate);
                String appStartupPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                //创建文件到当前运行应用程序的路径下
                FileStream aFile = new FileStream(appStartupPath + "/" + Path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(aFile);
                sw.Write(information);
                sw.Close();
                sw.Dispose();
            }
            catch //(FileNotFoundException e)
            {

            }
        }//生成一个txt文本


        /// <summary>
        /// 读取文件,读取路径为当前应用程序文件夹下
        /// </summary>
        /// <param name="Path">相对路径</param>
        /// <returns></returns>
        public string Read(string Path)
        {
            String appStartupPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            if (File.Exists(appStartupPath + "/" + Path))
            {
                FileStream aFile = new FileStream(appStartupPath + "/" + Path, FileMode.Open, FileAccess.Read);
                StreamReader sw = new StreamReader(aFile);
                string a = sw.ReadLine();
                sw.Close();
                sw.Dispose();
                return a;
            }
            else
            {
                return "00000";
            }
        }//读取一个txt文本

        /// <summary>
        /// 读取JSON文件
        /// </summary>
        /// <param name="key">JSON文件中的key值</param>
        /// <returns>JSON文件中的value值</returns>
        public static string Readjson(string Path, string key)
        {
            //JSON文件路径
            string jsonfile = Path;

            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
                    var value = o[key].ToString();
                    return value;
                }
            }
        }

        public static string Readjson_Mess(string mess,string key)
        {
            try
            {
                JObject o = (JObject)JsonConvert.DeserializeObject(mess);
                string value;
                if (o != null && o[key] != null)
                {
                    value = o[key].ToString();
                }
                else
                {
                    value = "Error";
                }
                return value;
            }
            catch
            {
                return "Error";
            }
        }

        public static bool Menu_IsVisible = false, Last_IsVisible = false;
        Window_Menu window_Menu = new Window_Menu();
        private void button_menu_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void button_menu_Click(object sender, RoutedEventArgs e)
		{
            button_menu_Click_();
        }

        public void button_menu_Click_()
		{
            Mouses.POINT point;
            Mouses.GetCursorPos(out point);
            //Point _point = new Point(point.X,point.Y);
            //window_Menu.Left = point.X;
            //window_Menu.Top = point.Y;
            window_Menu.Left = this.Left + 10;
            window_Menu.Top = this.Top + 30 + 22;
            if (window_Menu.IsVisible && Menu_IsVisible)
            {
                window_Menu.Hide();//window_Menu.Hide();
                Menu_IsVisible = false;
                Last_IsVisible = false;
            }
            else if(!window_Menu.IsVisible && !Menu_IsVisible)
            {
                window_Menu.Show();
                Menu_IsVisible = true;
                Last_IsVisible = true;
            }
			else
			{
                if(!Last_IsVisible)
				{
                    window_Menu.Show();
                    Menu_IsVisible = true;
                }
                else if(window_Menu.IsVisible)
				{
                    window_Menu.Show();
				}
			}
            //window_Menu.Left = point.X;
            //window_Menu.Top = point.Y;
            window_Menu.Left = this.Left + 10;
            window_Menu.Top = this.Top + 30 + 22;
        }

        private void button_submit_Click(object sender, RoutedEventArgs e)
        {
            Local_InputWords(false);
        }

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
            window_Menu.Hide();
        }

		private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
		{
            window_Menu.Hide();
        }

        ScreenShoot screenShoot = new ScreenShoot();
        public static bool ScreenShoot_Active = false;
        public static Point screenShoot_point;
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Escape))
            {
                ScreenShoot_Active = false;
                screenShoot.Hide();
                screenShoot.timer_screenShoot.Stop();
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {

        }

		private void Window_Closed(object sender, EventArgs e)
		{
            hook_keyboard.UnHook();
        }

		private void Window_LocationChanged(object sender, EventArgs e)
        {
            window_Menu.Left = this.Left + 10;
            window_Menu.Top = this.Top + 30 + 22;
            if (window_Menu.IsVisible && Menu_IsVisible)
            {
                window_Menu.Hide();//window_Menu.Hide();
                Menu_IsVisible = false;
                Last_IsVisible = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            window_Menu.Close();
            screenShoot.Close();
		}
	}

}

public class Mouses
{
    /// <summary>   
    /// 获取鼠标的坐标   
    /// </summary>   
    /// <param name="lpPoint">传址参数，坐标point类型</param>   
    /// <returns>获取成功返回真</returns>   
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool GetCursorPos(out POINT pt);

    ///<summary>   
    /// 设置鼠标的坐标   
    /// </summary>   
    /// <param name="x">横坐标</param>   
    /// <param name="y">纵坐标</param>   
    [DllImport("User32")]
    public extern static void SetCursorPos(int x, int y);
    public struct POINT
    {
        public int X;
        public int Y;
        public POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
    //POINT p = new POINT();
    //if (GetCursorPos(out p))//API方法
    //还有WPF中的方法
    //Point p = Mouse.GetPosition(e.Source as FrameworkElement);
    //Point p = (e.Source as FrameworkElement).PointToScreen(pp);
}


