using Get_Text;
using Segment.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Get_Text
{
    /// <summary>
    /// ScreenShoot.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenShoot : Window
    {
        public MainWindow mainWindow = null;
        /// <summary>
        /// 窗体初始化完成后，给窗体添加钩子监听窗口消息
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(myHook);
        }
        public const int WM_LoadFloor = 0x9111;//截鼠标和键盘消息
        public const int WM_HOTKEY = 0x0312;
        public const int WM_KEYDOWN = 0x0100;
        /// <summary>
        /// 钩子
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="msg">消息 ID</param>
        /// <param name="wParam">消息的 wParam 值</param>
        /// <param name="lParam">消息的 lParam 值</param>
        /// <param name="handled">一个值，该值指示是否处理过该消息。 将值设置为 true 如果消息已处理; 否则为 false。</param>
        /// <returns>相应的返回值取决于特定的消息。 请参阅消息文档详细 Win32 正在处理的消息。</returns>
        private IntPtr myHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int)WM_LoadFloor)
            {
                //监听到要拦截的信息做相应的业务处理以及各种响应
            }
            if (msg == (int)WM_HOTKEY)
            {
                int Mod_ALT = 0x0001;
                int MOD_CONTROL = 0x0002;
                MessageBox.Show("t");
                if ((int)lParam == Mod_ALT || (int)lParam == MOD_CONTROL)
                {
                    MessageBox.Show("t");
                }
            }
            return IntPtr.Zero;
        }

        public Hook_XYKS hook_mouse = new Hook_XYKS();
        //委托回调的方法
        IntPtr Hook_Mouse(int code, IntPtr wparam, IntPtr Iparam)
        {
            label.Content = ("鼠标左键" + wparam.ToInt32() + "   " + Iparam.ToString());
            if (code < 0)
            {
                //让后面的程序处理该消息
                return Hook_XYKS.CallNextHookEx(hook_mouse._nextHookPtr1, code, wparam, Iparam);
            }
            //用户输入的是x或者X
            if (wparam.ToInt32() == 514 && TextBox_Shoot.Visibility != Visibility.Visible)//(wparam.ToInt32() == 98 || wparam.ToInt32() == 66)
            {
                //MessageBox.Show("鼠标左键");
                LeftButtonDown();
                //该消息结束
                return (IntPtr)1;
            }
            else
            {
                //让后面的程序处理该消息
                return IntPtr.Zero;
            }
            //return Hook_XYKS.CallNextHookEx(hook_mouse._nextHookPtr1, code, wparam, Iparam);
        }

        public DispatcherTimer timer_screenShoot = new DispatcherTimer();
        public double screenWidth, screenHeight;
        public ScreenShoot()
        {
            InitializeComponent();
            screenWidth = SystemParameters.PrimaryScreenWidth; // 屏幕整体宽度
            screenHeight = SystemParameters.PrimaryScreenHeight; // 屏幕整体高度
            image.Width = screenWidth;
            image.Height = screenHeight;
            timer_screenShoot.Tick += new EventHandler(timer_screenShoot_Tick);
            //设置刷新的间隔时间    
            timer_screenShoot.Interval = TimeSpan.FromSeconds(0.02);   //TimeSpan.FromSeconds(0.1)
            Border_Box.Visibility = Visibility.Hidden;
            Label_Shoot.Visibility = Visibility.Hidden;
            TextBox_Shoot.Visibility = Visibility.Hidden;
            Button_ShootSubmit.Visibility = Visibility.Hidden;
            ScreenLinShi_Active.Visibility = Visibility.Hidden;
            ScreenShoot_Active.Visibility = Visibility.Hidden;
            hook_mouse.hookProc1 = new HookProc(Hook_Mouse);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Escape))
            {
                Window_Hidden();
            }
        }

        public void ImageShoot_Change(object _image, Point _point)
        {
            Image image = (Image)_image;
            Thickness Mov = new Thickness();
            Mov = image.Margin;
            int imageX = 15;
            Mov.Left = (int)_point.X - imageX;
            Mov.Top = (int)_point.Y - imageX;
            Mov.Right = (int)screenWidth - (_point.X + imageX);
            Mov.Bottom = (int)screenWidth - (_point.Y + imageX);
            image.Margin = Mov;
        }

        int w, h;
        int w1, h1;
        Point z_point;
        void timer_screenShoot_Tick(object sender, EventArgs e)
        {
            //进入截图状态
            if (MainWindow.ScreenShoot_Active)
            {
                Mouses.POINT point;
                Mouses.GetCursorPos(out point);
                Point _point = new Point(point.X, point.Y);
                w = (int)(_point.X - MainWindow.screenShoot_point.X);
                h = (int)(_point.Y - MainWindow.screenShoot_point.Y);
                if (w > 0 && h > 0)
                {
                    z_point = new Point(MainWindow.screenShoot_point.X, MainWindow.screenShoot_point.Y);
                    Button_PointChange(Button_Box, z_point, w, h);
                    Border_PointChange(Border_Box, z_point, w, h);
                }
                else if (w < 0 && h > 0)
                {
                    w1 = -w;
                    z_point = new Point(MainWindow.screenShoot_point.X - w1, MainWindow.screenShoot_point.Y);
                    Button_PointChange(Button_Box, z_point, w1, h);
                    Border_PointChange(Border_Box, z_point, w1, h);
                }
                else if (w > 0 && h < 0)
                {
                    h1 = -h;
                    z_point = new Point(MainWindow.screenShoot_point.X, MainWindow.screenShoot_point.Y - h1);
                    Button_PointChange(Button_Box, z_point, w, h1);
                    Border_PointChange(Border_Box, z_point, w, h1);
                }
                else if (w < 0 && h < 0)
                {
                    w1 = -w; h1 = -h;
                    z_point = new Point(MainWindow.screenShoot_point.X - w1, MainWindow.screenShoot_point.Y - h1);
                    Button_PointChange(Button_Box, z_point, w1, h1);
                    Border_PointChange(Border_Box, z_point, w1, h1);
                }
                else
                {
                    z_point = new Point(MainWindow.screenShoot_point.X, MainWindow.screenShoot_point.Y);
                    Button_PointChange(Button_Box, z_point, w, h);
                    Border_PointChange(Border_Box, z_point, w, h);
                }
            }
            if (Keyboard.IsKeyDown(Key.Left) || System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
            {

            }
            if (System.Windows.Input.Mouse.LeftButton != MouseButtonState.Pressed)
            {
                isReleased = true;
            }
            if (iL != 0)
            {
                iL++;
                if (iL > 10)
                {
                    iL = 0; isReleased = true;
                }
            }
            //当任务完成时
            if (task_LinShi != null)
            {
                if (task_LinShi.IsCompleted)
                {
                    //Task_LinShi_Completed();
                    Task_LinShi_Completed_Delegate task_LinShi_Completed_Delegate = new Task_LinShi_Completed_Delegate(Task_LinShi_Completed);
                    task_LinShi_Completed_Delegate.Invoke();
                    task_LinShi = null;
                }
            }
            if(task_ScreenShoot != null)
			{
                if (task_ScreenShoot.IsCompleted)
                {
                    //Task_ScreenShoot_Completed();
                    Task_ScreenShoot_Completed_Delegate task_ScreenShoot_Completed_Delegate = new Task_ScreenShoot_Completed_Delegate(Task_ScreenShoot_Completed);
                    task_ScreenShoot_Completed_Delegate.Invoke();
                    task_ScreenShoot = null;
                }
            }
            if (this.task_ReV != null)
            {
                mainWindow.TextBoxRight.Text = ReR.DiDaiCiShu.ToString();
                if (this.task_ReV.IsCompleted)
                {
                    //MessageBox.Show("OCR迭代更新完成");
                    mainWindow.Label_VVV.Visibility = Visibility.Hidden;
                    this.task_ReV = null;
                }
            }
        }
        bool isReleased = true;
        int iL = 1;//按键冷却
        Task task_LinShi = null;
        bool isTasked_LinShi = false;
        void LeftButtonDown()
        {
            if (iL == 0)
            {
                //初始状态-无框，等待截图中
                if (isReleased && Border_Box.Visibility == Visibility.Hidden && TextBox_Shoot.Visibility == Visibility.Hidden && task_LinShi == null)
                {
                    Mouses.POINT point;
                    Mouses.GetCursorPos(out point);
                    MainWindow.screenShoot_point = new Point(point.X, point.Y);
                    Button_Box.PointToScreen(MainWindow.screenShoot_point);
                    ImageShoot_Change(ImageShoot, MainWindow.screenShoot_point);
                    z_point = new Point(MainWindow.screenShoot_point.X, MainWindow.screenShoot_point.Y);
                    Button_PointChange(Button_Box, z_point, 0, 0);
                    Border_PointChange(Border_Box, z_point, 0, 0);
                    Border_Box.Visibility = Visibility.Visible;
                    Label_Shoot.Visibility = Visibility.Visible;
                }
                //截图状态中，且按键处于释放状态
                else if (isReleased && task_LinShi == null)
                {
                    TextBox_Shoot.Visibility = Visibility.Hidden;
                    Button_ShootSubmit.Visibility = Visibility.Hidden;
                    Border_Box.Visibility = Visibility.Hidden;
                    Label_Shoot.Visibility = Visibility.Hidden;
                    if (true)
                    {
                        //ScreenLinShi_Active.Visibility = Visibility.Visible;
                        task_LinShi = new Task(() =>
                        {
                            System.Threading.Thread.Sleep(30);
                            CaptureScreen.CreatePicture((int)z_point.X - 2, (int)z_point.Y - 2, width, height, Data_Save.getDocumentPath() + "Data/临时图片-识别.png");
                        });
                        task_LinShi.Start();
                    }
                }
                isReleased = false;
                iL = 1;
            }
        }
        string task_LinShi_Name = "";
        private delegate void Task_LinShi_Completed_Delegate();
        void Task_LinShi_Completed()
		{
            //提取图像特征
            string features = GetPNGFeatures(Data_Save.getDocumentPath() + "Data/临时图片-识别.png");
            PNG_Featrues pNG_Featrues = new PNG_Featrues();
            string mes = "";
            //找到匹配值
            mes = pNG_Featrues.Find_Message(features);
            task_LinShi_Name = mes;
            TextBox_Shoot.Text = mes;
            if (task_LinShi_Name == "\t０００００")
                TextBox_Shoot.Text = "";
            Border_Box.Visibility = Visibility.Visible;
            TextBox_Shoot.Visibility = Visibility.Visible;
            Button_ShootSubmit.Visibility = Visibility.Visible;
            Label_Shoot.Visibility = Visibility.Visible;
            //让文本框获取焦点
            TextBox_Shoot.Focus();
            //设置光标的位置到文本尾 
            TextBox_Shoot.Select(TextBox_Shoot.Text.Length, 0);
            //滚动到控件光标处 
            TextBox_Shoot.ScrollToEnd();//.ScrollToCaret();
            MainWindow.ScreenShoot_Active = false;
            //MessageBox.Show("??");
        }
        private void Button_Box_Click(object sender, RoutedEventArgs e)
        {

        }
        Task task_ScreenShoot = null;
        private void Button_ShootSubmit_Click(object sender, RoutedEventArgs e)
        {
            Button_ShootSubmit_Down();
        }
        void Button_ShootSubmit_Down()
		{
            TextBox_Shoot.Visibility = Visibility.Hidden;
            Button_ShootSubmit.Visibility = Visibility.Hidden;
            Border_Box.Visibility = Visibility.Hidden;
            Label_Shoot.Visibility = Visibility.Hidden;
            //刷新窗体
            if (false)
            {
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(new DispatcherOperationCallback(f =>

                {
                    ((DispatcherFrame)f).Continue = false;
                    return null;
                }), DispatcherPriority.Background, frame);
                Dispatcher.PushFrame(frame);
            }
            //ScreenShoot_Active.Visibility = Visibility.Visible;

            //string help = XYKS_dll.XYKS_GetPNGHelp_extern();
            //MessageBox.Show(help);
            string name = TextBox_Shoot.Text;
            TextBox_Shoot.Text = "";
            Clipboard.SetDataObject(name);//复制内容到剪切板
            string path = "Data/WordPNG/";
            for (int i = 0; i < name.Length; i++)
            {
                path += Data_Save.Replace_Word(name[i].ToString()) + "/";
                Data_Save.Create(path);
            }
            for (int i = 0; i < 102400; i++)
            {
                if (!System.IO.File.Exists(Data_Save.getDocumentPath() + path + i.ToString() + ".png"))
                {
                    task_ScreenShoot_Path = Data_Save.getDocumentPath() + path + i.ToString() + ".png";
                    task_ScreenShoot_Name = name;
                    task_ScreenShoot = new Task(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        CaptureScreen.CreatePicture((int)z_point.X - 2, (int)z_point.Y - 2, width, height, Data_Save.getDocumentPath() + path + i.ToString() + ".png");
                    });
                    task_ScreenShoot.Start();
                    break;
                }
            }
        }
        string task_ScreenShoot_Path = "";
        string task_ScreenShoot_Name = "";
        private delegate void Task_ScreenShoot_Completed_Delegate();
        public void Task_ScreenShoot_Completed()
		{
            try
            {
                PNG_Featrues pNG_Featrues = new PNG_Featrues();
                string featrues = GetPNGFeatures(task_ScreenShoot_Path);
                string gf = "";
                //gf = pNG_Featrues.Find_Message(featrues);
                gf = task_LinShi_Name;
                if (gf == "\t０００００")//未找到文件
                {
                    mainWindow.Label_VVV.Visibility = Visibility.Hidden;
                    pNG_Featrues.Set_Message(featrues, task_ScreenShoot_Name);
                }
                else if (gf != task_ScreenShoot_Name)//如果找到的信息和现有信息不同，则证明算法有问题，需要进行迭代
                {
                    //MessageBox.Show(gf + "   ???   " + task_ScreenShoot_Name + "   ???   " + task_LinShi_Name);
                    mainWindow.Label_VVV.Visibility = Visibility.Visible;
                    Data_Save.Save(" ", "OCRLanguage_XYKS.txt");
                    Data_Save.DelectDir(Data_Save.getDocumentPath() + "Data/OCRFeatures");
                    //Data_Save.DelectDir(Data_Save.getPath() + "Data/WordsO");
                    pNG_Featrues.Set_Message(GetPNGFeatures(task_ScreenShoot_Path), task_ScreenShoot_Name);
                    task_ReV = new Task(() =>
                    {
                        ReR reR = new ReR();
                        reR.ReRa(0, Data_Save.getDocumentPath() + "Data/WordPNG");
                        MessageBox.Show("完成:" + ReR.DiDaiCiShu);
                        //this.task_ReV.Dispose();
                    });
                    //MessageBox.Show("开始");
                    //MessageBox.Show("进行迭代更新,请等待");
                    mainWindow.TextBoxLeft.Text = "进行迭代更新,请等待";
                    task_ReV.Start();
                }
                else
                {
                    mainWindow.Label_VVV.Visibility = Visibility.Hidden;
                    pNG_Featrues.Set_Message(featrues, task_ScreenShoot_Name);
                }
            }
            catch(Exception e)
            { 
                MessageBox.Show("出现错误:" + "Task_ScreenShoot_Completed():" + e.ToString());
                Window_Hidden();
            }
            Window_Hidden();
        }
        public Task task_ReV = null;
        //迭代更新算法
        void ReV(int _count)
        {
            if (_count > 20)
			{
                //MessageBox.Show("迭代20次后: 迭代退出");
                return;
            }
            _count++;
            string srcPath = Data_Save.getDocumentPath() + "Data/WordPNG";
            Data_Save.Save(" ", "OCRLanguage_XYKS.txt");
            Data_Save.DelectDir(Data_Save.getDocumentPath() + "Data/OCRFeatures");
            //判断文件夹是否存在
            if (Directory.Exists(srcPath))
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                int ccc = 0;
                foreach (FileSystemInfo _if in fileinfo)
                {
                    mainWindow.TextBoxLeft.Text += ccc;
                    if (_if is DirectoryInfo)
                    {
                        ////判断是否文件夹
                        //DirectoryInfo subdir = new DirectoryInfo(_if.FullName);
                        //subdir.Delete(true);          //删除子目录和文件
                        ReR reR = new ReR();
                        reR.ReRa(0, _if.FullName);
                    }
                    else
                    {
                        ccc++;
                        PNG_Featrues pNG_Featrues = new PNG_Featrues();
                        string name = _if.FullName.Substring(Data_Save.getDocumentPath().Length);
                        string finalName = "";
                        int finalCount = 0;
                        string oldName = "";
                        for (int ixs = 0; ixs < name.Length; ixs++)
                        {
                            if (name[ixs] == '/' || name[ixs] == '\\')
                            {
                                finalName += oldName;
                                finalCount = ixs + 1;
                            }
                            else
                                oldName += name[ixs];
                        }
                        string pf = pNG_Featrues.Find_Message(GetPNGFeatures(_if.FullName));
                        if (pf == "\t０００００")//未找到文件
                            pNG_Featrues.Set_Message(GetPNGFeatures(_if.FullName), finalName);
                        else if (pf == finalName)
                            pNG_Featrues.Set_Message(GetPNGFeatures(_if.FullName), finalName);
                        else
                            ReV(_count);
                    }
                }
                MessageBox.Show(ccc.ToString());
            }
        }
        int width, height;

        private void Window_Closed(object sender, EventArgs e)
        {
            Window_Hidden();
        }

        void Button_PointChange(object _button, Point _point, int _width, int _height)
        {
            Button ClickBtn = (Button)_button;
            Thickness Mov = new Thickness();
            Mov = ClickBtn.Margin;
            int _border = 3;
            Mov.Left = (int)_point.X + _border;
            Mov.Top = (int)_point.Y + _border;
            Mov.Right = (int)screenWidth - (_width + _point.X - _border - _border);
            Mov.Bottom = (int)screenHeight - (_height + _point.Y - _border - _border);
            ClickBtn.Content = _width.ToString() + "   " + _height.ToString();
            ClickBtn.Margin = Mov;
            width = _width;
            height = _height;
            Point image_point = new Point(_point.X + _width, _point.Y + _height);
            ImageShoot_Change(ImageShoot2, image_point);
        }

        void Border_PointChange(object border, Point _point, int _width, int _height)
        {
            Border ClickBtn = (Border)border;
            Thickness Mov = new Thickness();
            Mov = ClickBtn.Margin;
            int _border = 3;
            Mov.Left = (int)_point.X + _border;
            Mov.Top = (int)_point.Y + _border;
            Mov.Right = (int)screenWidth - (_width + _point.X - _border - _border);
            Mov.Bottom = (int)screenHeight - (_height + _point.Y - _border - _border);
            ClickBtn.Margin = Mov;
            if (_width >= 0 && _height >= 0)
            {
                ClickBtn.Width = _width;
                ClickBtn.Height = _height;
            }
            Point point2 = new Point(_point.X - 0, _point.Y - 20);
            Thickness Mov1 = new Thickness();//框大小文字
            Mov1 = Label_Shoot.Margin;
            Mov1.Left = (int)point2.X + _border;
            Mov1.Top = (int)point2.Y + _border;
            Mov1.Right = (int)screenWidth - (100 + point2.X - _border - _border);
            Mov1.Bottom = (int)screenHeight - (100 + point2.Y - _border - _border);
            Label_Shoot.Margin = Mov1;
            Label_Shoot.Content = _width.ToString() + "/" + _height.ToString();
            int ySize = -30;
            if (point2.Y < 0)
                ySize = ySize + 35;
            point2 = new Point(_point.X + 2, _point.Y + ySize);
            Thickness Mov2 = new Thickness();//输入框
            Mov2 = TextBox_Shoot.Margin;
            Mov2.Left = (int)point2.X + _border;
            Mov2.Top = (int)point2.Y + _border;
            Mov2.Right = (int)screenWidth - (150 + point2.X - _border - _border);
            Mov2.Bottom = (int)screenHeight - (150 + point2.Y - _border - _border);
            TextBox_Shoot.Margin = Mov2;
            point2 = new Point(point2.X + 155, point2.Y);
            Thickness Mov3 = new Thickness();//取消按钮
            Mov3 = Button_Cancle.Margin;
            Mov3.Left = (int)point2.X + _border;
            Mov3.Top = (int)point2.Y + _border;
            Mov3.Right = (int)screenWidth - (100 + point2.X - _border - _border);
            Mov3.Bottom = (int)screenHeight - (100 + point2.Y - _border - _border);
            Button_Cancle.Margin = Mov3;
            point2 = new Point(point2.X + 35, point2.Y);
            Thickness Mov4 = new Thickness();//确认按钮
            Mov4 = Button_ShootSubmit.Margin;
            Mov4.Left = (int)point2.X + _border;
            Mov4.Top = (int)point2.Y + _border;
            Mov4.Right = (int)screenWidth - (100 + point2.X - _border - _border);
            Mov4.Bottom = (int)screenHeight - (100 + point2.Y - _border - _border);
            Button_ShootSubmit.Margin = Mov4;
        }

        private void Button_ShootSubmit_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Button_Cancle.Visibility = Button_ShootSubmit.Visibility;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

		private void ScreenShoot_Active_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
            if (ScreenShoot_Active.Visibility == Visibility.Visible)
            {
                ScreenShoot_Active.Visibility = Visibility.Hidden;
            }
        }

		private void ScreenLinShi_Active_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
            if (ScreenLinShi_Active.Visibility == Visibility.Visible)
            {
                ScreenLinShi_Active.Visibility = Visibility.Hidden;
            }
        }

		private void TextBox_Shoot_KeyDown(object sender, KeyEventArgs e)
		{
            if (Keyboard.IsKeyDown(Key.RightCtrl) && Keyboard.IsKeyDown(Key.Enter))
			{
                Button_ShootSubmit_Down();
            }
        }

		private void Button_Cancle_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Shoot.Visibility = Visibility.Hidden;
            Button_ShootSubmit.Visibility = Visibility.Hidden;
            Border_Box.Visibility = Visibility.Hidden;
            Label_Shoot.Visibility = Visibility.Hidden;
            //image.Source = new BitmapImage(new Uri(Data_Save.getPath() + "Data/准星.png"));// UriKind.Relative
            MainWindow.ScreenShoot_Active = false;
            MainWindow.ScreenShoot_Active = true;
        }
        public static string GetPNGFeatures(string _path)
        {
            bool x = XYKS_dll.XYKS_AnalysisPNG_extern(_path);
            //MessageBox.Show(x.ToString());
            int[] data = { XYKS_dll.XYKS_GetPNGColorData_extern(0, 0), XYKS_dll.XYKS_GetPNGColorData_extern(0, 1), XYKS_dll.XYKS_GetPNGColorData_extern(0, 2), XYKS_dll.XYKS_GetPNGColorData_extern(0, 3) };
            int r = data[0];
            //MessageBox.Show("r:" + (r).ToString() + " g:" + (int)data[1] + " b:" + (int)data[2] + " a:" + (int)data[3]);
            int length = XYKS_dll.XYKS_GetPNGColorDataLength_extern();
            //MessageBox.Show(length.ToString());
            if(false)
			{
                XYKS_dll.XYKS_DeletePNGAnalysis_extern();
                return "1;";
            }
            IntPtr intptr = XYKS_dll.XYKS_GetPNGFeatures_extern();//报错(尝试读取或写入受保护的内存。这通常指示其他内存已损坏。)时:用管理员身份运行CMD：netsh winsock reset
            string s = Marshal.PtrToStringAuto(intptr);
            XYKS_dll.XYKS_DeletePNGAnalysis_extern();
            return s;
        }
        public void Window_Visible()
        {
            hook_mouse.SetHook(hook_mouse.hookProc1, HookType.Mouse);
            MainWindow.ScreenShoot_Active = true;
            Button_Cancle.Visibility = Button_ShootSubmit.Visibility;
            this.Show();
            timer_screenShoot.Start();
        }
        public void Window_Hidden()
        {
            TextBox_Shoot.Visibility = Visibility.Hidden;
            Button_ShootSubmit.Visibility = Visibility.Hidden;
            Border_Box.Visibility = Visibility.Hidden;
            Label_Shoot.Visibility = Visibility.Hidden;
            image.Source = new BitmapImage(new Uri(Data_Save.getDocumentPath() + "Data/准星.png"));// UriKind.Relative
            MainWindow.ScreenShoot_Active = false;
            this.Hide();
            this.timer_screenShoot.Stop();
            hook_mouse.UnHook();
        }
    }

    class PNG_Featrues
    {
        ArrayList features;
        public void Set_Message(string _features, string _mes)
		{
            Data_Save.Create("Data/OCRFeatures");
            Decompose(_features);
            string path = "Data/OCRFeatures/";
            for(int i = 0;i<features.Count;i++)
            {
                path += Data_Save.Replace_Word(features[i].ToString()) + "/";
                Data_Save.Create(path);
            }
            path += "mess.txt";
            Data_Save.Save(_mes, path);
        }
        public string Find_Message(string _features)
		{
            Data_Save.Create("Data/OCRFeatures");
            Decompose(_features);
            string path = "Data/OCRFeatures/";
            for (int i = 0; i < features.Count; i++)
            {
                path += Data_Save.Replace_Word(features[i].ToString()) + "/";
                Data_Save.Create(path);
            }
            path += "mess.txt";
            string rs = Data_Save.Read(path);
            if (rs == "\t０００００")
			{

                return "\t０００００";
            }
            return rs;
        }

        //分解特征
        public void Decompose(string _features)
		{
            features = new ArrayList { };
            string s = "";
            for(int i = 0;i< _features.Length;i++)
			{
				if (_features[i] != ';')
				{
                    s += _features[i];
				}
                else
				{
                    features.Add(s);
                    s = "";
				}
			}
		}
    }

    class ReR
    {
        public static int DiDaiCiShu = 0;
        public void ReRa(int _count, string _path)
        {
            //return;
            if (_count > 1)
            {
                //MessageBox.Show("迭代20次后: 迭代退出");
                return;
            }
            _count++;
            DiDaiCiShu++;
            DirectoryInfo dir = new DirectoryInfo(_path);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            int ccc = 0;
            foreach (FileSystemInfo _if in fileinfo)
            {
                if (_if is DirectoryInfo)
                {
                    ////判断是否文件夹
                    //DirectoryInfo subdir = new DirectoryInfo(_if.FullName);
                    //subdir.Delete(true);          //删除子目录和文件
                    //ReR reR = new ReR();
                    //reR.ReRa(0, _if.FullName);
                    new ReR().ReRa(0, _if.FullName);
                }
                else
                {
                    PNG_Featrues pNG_Featrues = new PNG_Featrues();
                    string name = _if.FullName.Substring((Data_Save.getDocumentPath() + "Data/WordPNG/").Length - 1);
                    string finalName = "";
                    int finalCount = 0;
                    string oldName = "";
                    for (int ixs = 0; ixs < name.Length; ixs++)
                    {
                        if (name[ixs] == '/' || name[ixs] == '\\')
                        {
                            finalName += Data_Save.Reverse_Replace_Word(oldName);
                            finalCount = ixs + 1;
                            oldName = "";
                        }
                        else
                            oldName += name[ixs];
                    }
                    //finalName = name.Substring(0, finalCount);
                    //finalName.Replace("/", "");
                    //finalName.Replace("\\", "");
                    string pf = pNG_Featrues.Find_Message(ScreenShoot.GetPNGFeatures(_if.FullName));
                    if (pf == "\t０００００")//未找到文件
                        pNG_Featrues.Set_Message(ScreenShoot.GetPNGFeatures(_if.FullName), finalName);
                    else if (pf == finalName)
                        pNG_Featrues.Set_Message(ScreenShoot.GetPNGFeatures(_if.FullName), finalName);
                    else
                        ReRa(_count, _path);//发现重复时
                        //MessageBox.Show(_if.FullName + "\r\n" + finalName);
                }
            }
        }
        public static string GetPNGFeatures1(string _path)
        {
            bool x = XYKS_dll.XYKS_AnalysisPNG_extern(_path);
            //MessageBox.Show(x.ToString());
            int[] data = { XYKS_dll.XYKS_GetPNGColorData_extern(0, 0), XYKS_dll.XYKS_GetPNGColorData_extern(0, 1), XYKS_dll.XYKS_GetPNGColorData_extern(0, 2), XYKS_dll.XYKS_GetPNGColorData_extern(0, 3) };
            int r = data[0];
            //MessageBox.Show("r:" + (r).ToString() + " g:" + (int)data[1] + " b:" + (int)data[2] + " a:" + (int)data[3]);
            int length = XYKS_dll.XYKS_GetPNGColorDataLength_extern();
            //MessageBox.Show(length.ToString());
            if (false)
            {
                XYKS_dll.XYKS_DeletePNGAnalysis_extern();
                return "1;";
            }
            IntPtr intptr = XYKS_dll.XYKS_GetPNGFeatures_extern();//报错(尝试读取或写入受保护的内存。这通常指示其他内存已损坏。)时:用管理员身份运行CMD：netsh winsock reset
            string s = Marshal.PtrToStringAuto(intptr);
            XYKS_dll.XYKS_DeletePNGAnalysis_extern();
            //MessageBox.Show("TEZHENG:" + s);
            return s;
        }
    }
}