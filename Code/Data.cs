using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Get_Text
{
    /// <summary>
    /// 用于在硬盘中储存服务端数据
    /// </summary>
    class Data_Save
    {
        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="data">需要传入的数据</param>
        public void SaveSet(string data = null)
        {
            Save(data, "SetData.txt");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialization()
        {
            //创建文件夹Data
            Create("Data");
            Create("Data/TWords");
            Create("Data/Words");
        }

        /// <summary>
        /// 生成一个txt文本
        /// </summary>
        /// <param name="path"></param>
        /// <param name="information"></param>
        public static void Save(string information, string path)
        {
            Save_Absolute(information, getDocumentPath() + path);
        }
        public static void Save_Absolute(string information, string path)
        {
            try
            {
                ////MessageBox.Show(AppDomain.CurrentDomain.BaseDirectory);
                ////FileStream aFile = new FileStream(@"" + Path, FileMode.OpenOrCreate);
                //FileStream aFile = new FileStream(getPath() + path, FileMode.OpenOrCreate);
                //StreamWriter sw = new StreamWriter(aFile);
                //sw.Write(information);
                //sw.Close();
                //sw.Dispose();

                FileStream fs = new FileStream(path, FileMode.Create);
                //获得字节数组
                byte[] data = System.Text.Encoding.Default.GetBytes(information);
                //开始写入

                fs.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
                fs.Flush();
                fs.Close();
            }
            catch (FileNotFoundException e)
            {

            }
        }

        /// <summary>
        /// 读取一个txt文本;
        /// 返回值："\t０００００"找不到相应文本，"\t００００１"读取异常
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Read(string path)
        {
            return Read_Absolute(getDocumentPath() + path);
        }
        public static string Read_Absolute(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    FileStream aFile = new FileStream(path, FileMode.Open);
                    StreamReader sw = new StreamReader(aFile);
                    string a = "";
                    //while(sw.);
                    {
                        a = sw.ReadToEnd();
                    }
                    sw.Close();
                    sw.Dispose();
                    return a;
                }
                else
                {
                    return "\t０００００";
                }
            }
            catch (FileNotFoundException e)
            {

                return "\t０００００";
            }
        }

        /// <summary>
        /// 读取回车符后的字符
        /// </summary>
        /// <param name="s">需要截取的字符串</param>
        /// <param name="i">读取的回车符个数（作为起始点）</param>
        /// <param name="length">需要截取的长度;为0时,则截取到下一个回车符前</param>
        /// <returns></returns>
        public static string Read_enter(string s, int i, int length)
        {
            //计数
            int count_i = 0, count_length = 0;
            //存储截取后的字符
            string return_s = null;
            //尝试读取信息
            try
            {
                //遍历检测
                foreach (char ss in s)
                {
                    //找到相应回车符
                    if (count_i == i)
                    {
                        //找到结尾，跳出循环
                        if (ss == Data.getEnter_First())
                        {
                            break;
                        }
                        return_s += ss;
                        count_length++;
                        //达到截取标准,跳出循环
                        if (length != 0 && count_length >= length)
                        {
                            break;
                        }
                    }
                    //每次找到回车符
                    if (ss == Data.getEnter_First())
                    {
                        //回车符计数加一
                        count_i++;
                        //已跳过相应回车符时
                        if (count_i > i)
                        {
                            break;
                        }
                    }
                }
                if (return_s.Length > 0)
                {
                    //当第一位字符为换行符时
                    if (return_s.Substring(0, 1) == "\n")
                    {
                        return return_s.Substring(1);
                    }
                }
                else
                {

                }
            }
            catch
            {

            }
            return return_s;
        }

        /// <summary>
        /// 创建一个文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns></returns>
        public static string Create(string path)
        {
            return Create_Absolute(getDocumentPath() + path);
        }
        public static string Create_Absolute(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    return ("文件夹已存在");
                }
                else
                {
                    Directory.CreateDirectory(path);
                    return ("创建成功");
                }
            }
            catch (DirectoryNotFoundException e)
            {

                return ("异常");
            }
        }

        //删除文件夹下所有文件
        public static void DelectDir(string srcPath)
        {
            try
            {
                //判断文件夹是否存在
                if (Directory.Exists(srcPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(srcPath);
                    FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                    foreach (FileSystemInfo i in fileinfo)
                    {
                        if (i is DirectoryInfo)
                        {
                            //判断是否文件夹
                            DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                            subdir.Delete(true);          //删除子目录和文件
                        }
                        else
                        {
                            File.Delete(i.FullName);      //删除指定文件
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        /// <summary>
        /// 判断字符串是大写还是小写
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>是大写返回true</returns>
        static bool hasUpperCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            return Regex.IsMatch(str, "[A-Z]");
        }
        /// <summary>
        /// 存入注册后的初始化数据
        /// </summary>
        /// <param name="_word"></param>
        /// <param name="_Tword">translate word</param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string Register(string _word, string _Tword, string p)
        {
            //去除空格及空字符
            {
                //_word = Replace_Word(_word);
            }
            //创建以用户名为关键字的文件夹
            {
                try
                {
                    //用于存储路径值
                    string z = "Data/Words/";
                    for (int i = 0; i < _word.Length; i++)
                    {
                        string s = Replace_Word(_word[i].ToString());
                        z = z + "/" + s;
                        Create(z);
                    }
                    z += "/trans.txt";
                    //保存文件
                    Save(_Tword, z);
                }
                catch
                {
                    return "false";
                }
            }
            //创建以账号为关键字的文件夹
            //{
            //    try
            //    {
            //        //用于存储路径值
            //        string z = "Data/TWords/";
            //        foreach (char s in _word)
            //        {
            //            z = z + "/" + s;
            //            Create(z);
            //        }
            //        //保存文件
            //        Save(_word + Data.getEnter() + p, z);
            //    }
            //    catch
            //    {
            //        return "false";
            //    }
            //}
            return "true";
        }

        /// <summary>
        /// 通过词(键)，读取值
        /// </summary>
        /// <param name="_word"></param>
        /// <returns></returns>
        public static string ReadWord(string _word)
        {
            //string word = Replace_Word(_word);
            //用于存储路径值
            string z = "Data/Words/";
            for (int i = 0; i < _word.Length; i++)
            {
                string s = Replace_Word(_word[i].ToString());
                z = z + "/" + s;
                Create(z);
            }
            z += "/trans.txt";
            string xs = Read(z);
            if (xs != "\t０００００")
                return xs;
            else
            {
                _word = Replace_Word_Old(_word);
                string zo = "Data/WordsO/";
                for (int i = 0; i < _word.Length; i++)
                {
                    zo += "/" + _word[i];
                    Create(zo);
                }
                zo += "/trans.txt";
                return Read(zo);
            }

        }

        public static string Register(string _word, string _OCRword)
        {
            //去除空格及空字符
            {
                //_word = Replace_Word(_word);
            }
            //创建以用户名为关键字的文件夹
            {
                try
                {
                    //用于存储路径值
                    string z = "Data/OCRWords/";
                    for (int i = 0; i < _word.Length; i++)
                    {
                        string s = Replace_Word(_word[i].ToString());
                        z = z + "/" + s;
                        Create(z);
                    }
                    z += "/trans.txt";
                    //保存文件
                    Save(_OCRword, z);
                }
                catch
                {
                    return "false";
                }
            }
            return "true";
        }//OCR Word
        public static string ReadOCRWord(string _word)
        {
            //_word = Replace_Word(_word);
            //用于存储路径值
            string z = "Data/OCRWords/";
            for (int i = 0; i < _word.Length; i++)
            {
                string s = Replace_Word(_word[i].ToString());
                z = z + "/" + s;
                Create(z);
            }
            z += "/trans.txt";
            return Read(z);
        }

        //更改空格及空字符、特殊字符
        public static string Replace_Word_Old(string _word)
        {
            _word = _word.Replace(" ", "");//犇　品　bēn
                                           //_Tword = _Tword.Replace(" ", "");//猋　品　biāo
                                           //p = p.Replace(" ", "");
            _word = _word.Replace("\t", "");//骉　品　biāo
            _word = _word.Replace("\r", "");
            _word = _word.Replace("\n", "");
            //_Tword = _Tword.Replace("\t", "");
            //p = p.Replace("\t", "");
            _word = _word.Replace("\\", "");
            _word = _word.Replace("/", "");
            _word = _word.Replace("*", "");
            _word = _word.Replace("?", "");
            _word = _word.Replace("\"", "");
            _word = _word.Replace("<", "");
            _word = _word.Replace(">", "");
            _word = _word.Replace("|", "");
            return _word;
        }

        //更改空格及空字符、特殊字符
        public static string Replace_Word(string _word)
        {
            for (int i = 0; i < _word.Length; i++)
            {
                string s = _word[i].ToString();
                if (hasUpperCase(s))
                {
                    //s = "UC" + s;
                    _word = _word.Replace(s, "U" + s);
                    break;
                }
            }
            _word = _word.Replace(" ", "空格");//錛 ben
                                             //_Tword = _Tword.Replace(" ", "");//猋　品　biāo
                                             //p = p.Replace(" ", "");
            _word = _word.Replace("\t", "反斜杠t");//骉　品　biāo
            _word = _word.Replace("\r", "反斜杠r");//曌 【zhao】
            _word = _word.Replace("\n", "反斜杠n");//赟【yun1】
                                                //_Tword = _Tword.Replace("\t", "");
                                                //p = p.Replace("\t", "");
            _word = _word.Replace("\\", "反斜杠");//觱 bi
            _word = _word.Replace("/", "斜杠");//龘【dá】
            _word = _word.Replace("*", "星号");//蕈【xun】
            _word = _word.Replace("?", "问号");//耄【mào
            _word = _word.Replace("\"", "冒号");//耋dié】
            _word = _word.Replace("<", "小于");//躞 xiâ】
            _word = _word.Replace(">", "大于");//驫【piāo】
            _word = _word.Replace("|", "竖线");//蹕 bi
            return _word;
        }
        public static string Reverse_Replace_Word(string _word)
        {
            if (_word.Length > 1)
            {
                if (_word.Substring(0, 1) == "U" && _word.Length <= 2)
                    _word = _word.Substring(1, 1);
            }


            _word = _word.Replace("空格", " ");//錛 ben
                                             //_Tword = _Tword.Replace(" ", "");//猋　品　biāo
                                             //p = p.Replace(" ", "");
            _word = _word.Replace("反斜杠t", "\t");//骉　品　biāo
            _word = _word.Replace("反斜杠r", "\r");//曌 【zhao】
            _word = _word.Replace("反斜杠n", "\n");//赟【yun1】
                                                //_Tword = _Tword.Replace("\t", "");
                                                //p = p.Replace("\t", "");
            _word = _word.Replace("反斜杠", "\\");//觱 bi
            _word = _word.Replace("斜杠", "/");//龘【dá】
            _word = _word.Replace("星号", "*");//蕈【xun】
            _word = _word.Replace("问号", "?");//耄【mào
            _word = _word.Replace("冒号", "\"");//耋dié】
            _word = _word.Replace("小于", "<");//躞 xiâ】
            _word = _word.Replace("大于", ">");//驫【piāo】
            _word = _word.Replace("竖线", "|");//蹕 bi
            return _word;
        }

        public static string ReadMessage(string _key, string _path)
        {
            string final = "";
            int model = 0;
            string message = Data_Save.Read_Absolute(_path);
            int length = message.Length;
            for (int i = 0; i < length; i++)
            {
                if (length - i > _key.Length && model == 0)
                {
                    //AppKey, AppSecret, Source, Target;
                    if (message.Substring(i, _key.Length) == _key)
                    {
                        i += _key.Length + 1;
                        model = 1;
                    }
                }
                if (model != 0)
                {
                    switch (model)
                    {
                        case 1:
                            if (message.Substring(i, 1) != "\r" || message.Substring(i, 1) == "\t")
                                final += message.Substring(i, 1);
                            else
                            {
                                model = 0;
                                return final;
                            }
                            break;
                    }
                }
            }
            return final;
        }

        /// <summary>
        /// 获取当前应用程序路径
        /// </summary>
        /// <returns></returns>
        public static string getPath()
        {
            return (AppDomain.CurrentDomain.BaseDirectory).Replace("\\", "/") + "/";
        }
        private static string documentPath = "";
        public static string getDocumentPath()
        {
            //documentPath = "";
            if (documentPath == "")
            {
                documentPath = ReadMessage("DocumentPath:", getPath() + "setting.txt");
                if (documentPath == "")
                {
                    documentPath = getPath();
                }
                if (documentPath == "")
                {
                    documentPath = "错误";
                }
                if (documentPath == "错误" || documentPath == getPath())
                    MessageBox.Show(documentPath);
                XYKS_dll.XYKS_AnalysisPNG_SetPath_extern(documentPath);
            }
            return documentPath;
        }
    }
    static class Data
    {
        /// <summary>
        /// 获得换行
        /// </summary>
        /// <returns></returns>
        public static string getEnter()
        {
            return System.Environment.NewLine;
            return "\r";
        }

        /// <summary>
        /// 获得第一个换行符号
        /// </summary>
        /// <returns></returns>
        public static char getEnter_First()
        {
            return '\r';
            return System.Environment.NewLine.Substring(0, 1).ToCharArray()[0];
        }

        /// <summary>
        /// 去掉相应字符
        /// </summary>
        /// <returns></returns>
        public static string removeChar(string s = " ", char c = ' ')
        {
            string z = null;
            try
            {
                foreach (char a in s)
                {
                    if (a != c)
                    {
                        z += a;
                    }
                }
            }
            catch
            {

            }
            return z;
        }
    }
}
