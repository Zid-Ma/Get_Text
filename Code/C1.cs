using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Threading;

namespace Get_Text
{
    class C1
    {
        public static String appStartupPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        //E:/Resource/C++/ClearRubbish/Ex_ClipBoard_Get/Debug/

        //引入外部dll文件
        /// <summary>
        /// 初始化获取程序
        /// </summary>
        /// <returns></returns>
        [DllImport("DLL/Ex_ClipBoard_Get.dll")]
        public static extern bool First(int final, string path);

        /// <summary>
        /// 用于获取剪切板内容
        /// </summary>
        /// <returns></returns>
        [DllImport(("DLL/Ex_ClipBoard_Get.dll"))]
        public static extern string ClipBoard_Get();

        /// <summary>
        /// 用于获取剪切板内容
        /// </summary>
        /// <returns></returns>
        [DllImport(("DLL/Ex_ClipBoard_Get.dll"))]
        public static extern string ClipBoard_Get_A();

        /// <summary>
        /// 用于获取内存中的信息
        /// </summary>
        /// <returns></returns>
        [DllImport(("DLL/Ex_ClipBoard_Get.dll"))]
        public static extern string Get_MessageForMemory();
    }
    class XYKS_dll
    { 
        /// <summary>
        /// 用于获取帮助信息
        /// </summary>
        /// <returns></returns>
        [DllImport(("DLL/XYKS_PNG_Analysis.dll"))]
        public static extern string XYKS_GetPNGHelp_extern();

        /// <summary>
        /// 用于解析PNG图像(前置项)
        /// </summary>
        /// <returns></returns>
        [DllImport(("DLL/XYKS_PNG_Analysis.dll"))]
        public static extern bool XYKS_AnalysisPNG_extern(string _path);

        /// <summary>
        /// 用于获取解析好的颜色数据
        /// </summary>
        /// <returns></returns>
        [DllImport(("DLL/XYKS_PNG_Analysis.dll"))]
        public static extern Int16 XYKS_GetPNGColorData_extern(Int32 _seek, int _count);

        /// <summary>
        /// 设置数据保存路径
        /// </summary>
        /// <param name="_path"></param>
        [DllImport(("DLL/XYKS_PNG_Analysis.dll"))]
        public static extern void XYKS_AnalysisPNG_SetPath_extern(string _path);

        /// <summary>
        /// 用于获取解析的颜色数据长度(记得*4)
        /// </summary>
        /// <returns></returns>
        [DllImport(("DLL/XYKS_PNG_Analysis.dll"))]
        public static extern Int32 XYKS_GetPNGColorDataLength_extern();

        /// <summary>
        /// 用于提取图像特征
        /// </summary>
        /// <returns></returns>
        [DllImport(("DLL/XYKS_PNG_Analysis.dll"), CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr XYKS_GetPNGFeatures_extern();

        /// <summary>
        /// 用于删除已解析好的数据
        /// </summary>
        /// <returns></returns>
        [DllImport(("DLL/XYKS_PNG_Analysis.dll"))]
        public static extern void XYKS_DeletePNGAnalysis_extern();

    }
}
