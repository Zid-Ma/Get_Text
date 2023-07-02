using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;  //引入的命名空间
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Get_Text
{
    public enum HookType
    {
        Mouse = 7,
        Keyboard = 2,   //键盘钩子类型
        WM_HOTKEY = 0x0312,
        WM_KEYDOWN = 0x0100,
        WM_LBUTTONDOWN = 0x201,// 鼠标左键按下
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    //定义委托,该委托的参数是固定这样写
    public delegate IntPtr HookProc(int code, IntPtr wparam, IntPtr lparam);

    public class Hook_XYKS
    {
        //记录下一个Hook编号
        static IntPtr _nextHookPtr;
        public IntPtr _nextHookPtr1;
        static HookProc hookProc;
        public HookProc hookProc1;

        //获取当前线程编号
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        //卸载钩子
        [DllImport("User32.dll")]
        public static extern void UnhookWindowsHookEx(IntPtr handle);

        //安装钩子
        [DllImport("User32.dll")]
        public extern static IntPtr SetWindowsHookEx(int idHook, [MarshalAs(UnmanagedType.FunctionPtr)] HookProc Ipfn, IntPtr hanstance, int threadID);


        //获取下一个钩子
        [DllImport("User32.dll")]
        public extern static IntPtr CallNextHookEx(IntPtr handle, int code, IntPtr wparam, IntPtr Iparam);


        //委托回调的方法
        IntPtr MyHookProc(int code, IntPtr wparam, IntPtr Iparam)
        {
            MessageBox.Show("键盘:" + wparam.ToInt32());
            if (code < 0)
            {
                //让后面的程序处理该消息
                return CallNextHookEx(_nextHookPtr, code, wparam, Iparam);
            }
            //用户输入的是b或者B
            if (wparam.ToInt32() == 98 || wparam.ToInt32() == 66)
            {
                //设置文本输入框为a
                MessageBox.Show("我拦截到B或者b");
                //该消息结束
                return (IntPtr)1;
            }
			else
			{
				//让后面的程序处理该消息
				return IntPtr.Zero;
			}
			//return CallNextHookEx(_nextHookPtr, code, wparam, Iparam);
        }

        /// <summary>
        /// 从外部调用设置钩子
        /// </summary>
        public void SetHook()
        {
            //已经设置过钩子了，不能重复设置
            if (_nextHookPtr != IntPtr.Zero)
            {
                return;
            }

            //设置钩子委托回调函数(委托方法注册)
            hookProc = new HookProc(MyHookProc);
            IntPtr lockHwnd = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);
            //把该钩子加到Hook链中
            _nextHookPtr = SetWindowsHookEx((int)HookType.Keyboard, hookProc, lockHwnd, GetCurrentThreadId());
        }

        public void SetHook(HookProc _myhookProc, HookType _hookType)
		{
            if(_nextHookPtr1 != IntPtr.Zero)
			{
                return;
			}
            IntPtr lockHwnd = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);
            _nextHookPtr1 = SetWindowsHookEx((int)_hookType, _myhookProc, lockHwnd, GetCurrentThreadId());
        }


        /// <summary>
        /// 从外部调用卸载钩子
        /// </summary>
        public void UnHook()
        {
            if (_nextHookPtr != IntPtr.Zero)
            {
                //从Hook链中取消
                UnhookWindowsHookEx(_nextHookPtr);
            }
            if(_nextHookPtr1 != IntPtr.Zero)
                UnhookWindowsHookEx(_nextHookPtr1);
            _nextHookPtr = IntPtr.Zero;
            _nextHookPtr1 = IntPtr.Zero;
        }
    }
}