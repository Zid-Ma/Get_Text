using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Get_Text;

namespace Get_Text
{
    class YouDaoAPI
    {
        private static string maxLength;
        public static int MaxLength
		{
            get
            {
                try
                {
                    return int.Parse(maxLength);
                }
                catch (Exception e)
                {
                    //MessageBox.Show(maxLength);
                    return 128;
                }
            }
			set
			{
                try
                {
                    maxLength = value.ToString();
                }catch(Exception e)
				{
                    //MessageBox.Show(e.Message);
                    maxLength = "128";
				}
			}
		}
        private static string AppKey, AppSecret, Source, Target;
        private static string newSource,newTarget;
        public static string Find(string mess)
        {
            Disassemble(mess);
            string localMess = Data_Save.ReadWord(mess);
            //当在本地读取到文本时, 返回读取到的值
            if (localMess != "\t０００００")
            {
                return localMess;
            }
            newSource = VerificationSource(mess);
            mess = SourceProcessing(mess);
            newTarget = Target;
            if (newSource == "en")
            {
                newTarget = "zh-CHS";
            }
            else if (newSource == "ja")
            {
                newTarget = "zh-CHS";
            }
            else if (newSource == "zh-CHS")
            {
                newTarget = "en";
            }
            if(false)
                newSource = Source;
            //MessageBox.Show(newSource);
            ReadMessage();
            //用于记录查询内容与查询模式
            Dictionary<String, String> dic = new Dictionary<String, String>();
            //接入API的网址
            string url = "https://openapi.youdao.com/api";
            //待输入的文字
            string q = mess;
            //您的应用ID
            string appKey = AppKey;//"01280fa4e8e2c804";
            //您的应用密钥
            string appSecret = AppSecret;//"3iLMI60CzsdRVPahVX6P1bNLdcQd6sdL";
            string salt = DateTime.Now.Millisecond.ToString();
            //源语言,中文zh-CHS,英文en,日文ja
            dic.Add("from", newSource);//"en"
            //目标语言
            dic.Add("to", newTarget);//"zh-CHS"
            dic.Add("signType", "v3");
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            long millis = (long)ts.TotalMilliseconds;
            string curtime = Convert.ToString(millis / 1000);
            //是否返回音频文件
            //dic.Add("ext","mp3");
            //当前时间
            dic.Add("curtime", curtime);
            string signStr = appKey + Truncate(q) + salt + curtime + appSecret; ;
            string sign = ComputeHash(signStr, new SHA256CryptoServiceProvider());
            dic.Add("q", System.Web.HttpUtility.UrlEncode(q));
            dic.Add("appKey", appKey);
            dic.Add("salt", salt);
            dic.Add("sign", sign);
            //您的用户词表ID
            //dic.Add("vocabId", "您的用户词表ID");
            return Post(url, dic);
        }

        protected static string ComputeHash(string input, HashAlgorithm algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }

        /// <summary>
        /// 向服务端发送请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        protected static string Post(string url, Dictionary<String, String> dic)
        {
            string result = "";
            //创建一个Http网页链接
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            //将byte数组转为string类型
            //return System.Text.Encoding.UTF8.GetString(data);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            if (resp.ContentType.ToLower().Equals("audio/mp3"))
            {
                //合成的音频存储路径
                SaveBinaryFile(resp, C1.appStartupPath);
            }
            else
            {
                Stream stream = resp.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                Console.WriteLine(result);
            }
            return result;
        }

        protected static string Truncate(string q)
        {
            if (q == null)
            {
                return null;
            }
            int len = q.Length;
            return len <= 20 ? q : (q.Substring(0, 10) + len + q.Substring(len - 10, 10));
        }

        /// <summary>
        /// 储存音频文件
        /// </summary>
        /// <param name="response"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private static bool SaveBinaryFile(WebResponse response, string FileName)
        {
            string FilePath = FileName + DateTime.Now.Millisecond.ToString() + ".mp3";
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                if (File.Exists(FilePath))
                    File.Delete(FilePath);
                Stream outStream = System.IO.File.Create(FilePath);
                Stream inStream = response.GetResponseStream();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();
            }
            catch
            {
                Value = false;
            }
            return Value;
        }

        static bool first = true;
        /// <summary>
        /// 读取硬盘中的数据
        /// </summary>
        /// <returns>读取情况</returns>
        private static string ReadMessage()
		{
            if (!first)
                return "";
            first = false;
            maxLength = "";
            AppSecret = AppKey = Source = Target = "";
            string final = "";
            int model = 0;
            string message = Data_Save.Read("Data/message.txt");
            int length = message.Length;
            for (int i =0;i< length; i++)
			{
                if (length - i > 10 && model == 0)
                {
                    //AppKey, AppSecret, Source, Target;
                    if (message.Substring(i, 10) == "AppSecret:")
                    {
                        i += 10 + 1;
                        model = 1;
                    }
                    else if (message.Substring(i, 7) == "AppKey:")
                    {
                        i += 7 + 1;
                        model = 2;
                    }
                    else if (message.Substring(i, 7) == "Source:")
                    {
                        i += 7 + 1;
                        model = 3;
                    }
                    else if (message.Substring(i, 7) == "Target:")
                    {
                        i += 7 + 1;
                        model = 4;
                    }
                    else if (message.Substring(i, 10) == "MaxLength:")
					{
                        i += 10 + 1;
                        model = 5;
                    }
                }
                if(model != 0)
                {
                    switch (model)
                    {
                        case 1:
                            if (message.Substring(i, 1) != "\r")
                                AppSecret += message.Substring(i, 1);
                            else
                                model = 0;
                            break;
                        case 2:
                            if (message.Substring(i, 1) != "\r")
                                AppKey += message.Substring(i, 1);
                            else
                                model = 0;
                            break;
                        case 3:
                            if (message.Substring(i, 1) != "\r")
                                Source += message.Substring(i, 1);
                            else
                                model = 0;
                            break;
                        case 4:
                            if (message.Substring(i, 1) != "\r")
                                Target += message.Substring(i, 1);
                            else
                                model = 0;
                            break;
                        case 5:
                            if (message.Substring(i, 1) != "\r")
                                maxLength += message.Substring(i, 1);
                            else
                                model = 0;
                            break;
                    }
                }
            }
            //MessageBox.Show(maxLength);
            //MessageBox.Show(AppSecret+ "/" + AppKey + "/" + Source + "/" + Target);
            final = message;
            return final;
		}

        /// <summary>
        /// 验证源语言
        /// </summary>
        /// <param name="_source"></param>
        /// <returns></returns>
        public static string VerificationSource(string _source)
        {
            //1.GBK(GB2312 / GB18030)
            // x00 -/ xff  GBK双字节编码范围
            // x20 -/ x7f  ASCII
            // xa1 -/ xff  中文
            // x80 -/ xff  中文
            //2. UTF-8 (Unicode)
            // u4e00 -/ u9fa5(中文)
            // x3130 -/ x318F(韩文
            // xAC00 -/ xD7A3(韩文) /[\uac00-\ud7ff]/
            // u0800 -/ u4e00(日文)  U+3040–U+309F: Hiragana  U + 30A0–U + 30FF: Katakana  U + 4E00–U + 9FBF: Kanji
            Regex regChina = new Regex("^[\x80-\xff\u4e00-\u9fa5]$");//\u4e00-\u9fa5
            Regex regEnglish = new Regex("^[a-zA-Z]$");
            Regex regJap = new Regex("^[\u0800-\u4e00\u31F0-\u31FF\u3040-\u309F\u30A0-\u30FFァ-ヶーあ-んが-ピが-ぼぱ-ぽア-ンガ-ボパ-ポき-りキ-リ]$");//\u309f-\u3040\u30ff-\u30a0\u9fbf-\u4e00     //\u309f-\u3040\u30ff-\u30a0\u9fbf-\u4e00
            int engCount = 0;
            for (int i = 0; i < _source.Length; i++)
            {
                if (regEnglish.IsMatch(_source.Substring(i, 1)))
                {
                    engCount++;
                }
            }
            int japCount = 0;
            for(int i = 0;i<_source.Length;i++)
			{
                if(regJap.IsMatch(_source.Substring(i,1)))
				{
                    japCount++;
				}
			}
            if (regEnglish.IsMatch(_source) || engCount > _source.Length / 3)
            {
                return "en";
            }
            else if (regJap.IsMatch(_source.Substring(0,1)) || japCount > _source.Length / 3)//.Substring(0,1)
                return "ja";
            else if (regChina.IsMatch(_source))
                return "zh-CHS";
            else
                return "zh-CHS";
        }

        /// <summary>
        /// 处理需要翻译的文本
        /// </summary>
        /// <param name="_source"></param>
        /// <returns></returns>
        private static string SourceProcessing(string _source) // 处理
        {
            string ss = "";
            ss += _source[0];
            //遍历到最后一位字符之前
            for (int i = 1; i < _source.Length - 1; i++)
            {
                Regex regEnglish_Big = new Regex("^[A-Z]");//正则表达式验证大小写
                Regex regEnglish_Small = new Regex("^[a-z]");//正则表达式验证大小写
                //当找到大写字符时,在大写字符之前加入空格
                if (regEnglish_Big.IsMatch(_source[i].ToString()))
                {
                    //如果当前字符不是最后一位
                    if (i + 1 < _source.Length)
                    {
                        //判断后一位是否是小写字符，如果是小写字符，则在当前大写字符之前先加上一个空格
                        if (regEnglish_Small.IsMatch(_source[i + 1].ToString()))
                        {
                            ss += " ";
                        }
                    }
                    //如果当前字符是最后一位
                    else
                    {
                        ss += " ";
                        //ss += _source[i];
                    }
                }
                ss += _source[i];
            }
            //判断最后一位字符是否为空格
            if (_source.Substring(_source.Length - 1,1) != " ")
                //最后一位字符不是空格，则加入需要的字符变量中
                ss += _source[_source.Length -1];
            //MessageBox.Show(ss + "?");//MessageBox 
            return ss;
        }

        /// <summary>
        /// 拆分句子中的词，并逐个翻译，记录
        /// </summary>
        /// <param name="mess"></param>
        /// <returns></returns>
        static string Disassemble(string mess)
        {
            mess += " ";
            string s = "";
            for(int i =0;i<mess.Length;i++)
			{
                if (V_disassemble(mess.Substring(i,1)))
				{
                    if (s != "")
                    {
                        Record(s);
                        s = "";
                    }
                }
                else
				{
                    s += mess.Substring(i, 1);
                }
			}
            
            return "";
        }

        /// <summary>
        /// 验证拆分
        /// </summary>
        /// <returns></returns>
        static bool V_disassemble(string mess)
		{
            string[] s = { " ", "`","~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "-", "_", "=", "+", "[", "{", "]", "}", ";", ":", "'", "\"", ",", "<", ".", ">", "/", "?", "\r", "\n", "\t"};
            for(int i = 0;i<s.Length;i++)
			{
                if(mess == s[i])
				{
                    return true;
				}
			}
            return false;
		}

        /// <summary>
        /// 记录词和翻译后的意思
        /// </summary>
        /// <param name="mess"></param>
        static void Record(string mess)
		{
            //当在本地读取到文本时, 结束当前函数
            if(Data_Save.ReadWord(mess) != "\t０００００")
			{
                //MessageBox.Show("找到已有翻译文本:" + mess);
                return;
			}
            //MessageBox.Show("拆分后的词:" + mess);
            newSource = VerificationSource(mess);
            mess = SourceProcessing(mess);
            newTarget = Target;
            if (newSource == "en")
            {
                newTarget = "zh-CHS";
            }
            else if (newSource == "ja")
            {
                newTarget = "zh-CHS";
            }
            else if (newSource == "zh-CHS")
            {
                newTarget = "en";
            }
            if (false)
                newSource = Source;
            //MessageBox.Show(newSource);
            ReadMessage();
            //用于记录查询内容与查询模式
            Dictionary<String, String> dic = new Dictionary<String, String>();
            //接入API的网址
            string url = "https://openapi.youdao.com/api";
            //待输入的文字
            string q = mess;
            //您的应用ID
            string appKey = AppKey;//"01280fa4e8e2c804";
            //您的应用密钥
            string appSecret = AppSecret;//"3iLMI60CzsdRVPahVX6P1bNLdcQd6sdL";
            string salt = DateTime.Now.Millisecond.ToString();
            //源语言,中文zh-CHS,英文en,日文ja
            dic.Add("from", newSource);//"en"
            //目标语言
            dic.Add("to", newTarget);//"zh-CHS"
            dic.Add("signType", "v3");
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            long millis = (long)ts.TotalMilliseconds;
            string curtime = Convert.ToString(millis / 1000);
            //当前时间
            dic.Add("curtime", curtime);
            string signStr = appKey + Truncate(q) + salt + curtime + appSecret; ;
            string sign = ComputeHash(signStr, new SHA256CryptoServiceProvider());
            dic.Add("q", System.Web.HttpUtility.UrlEncode(q));
            dic.Add("appKey", appKey);
            dic.Add("salt", salt);
            dic.Add("sign", sign);
            //您的用户词表ID
            //dic.Add("vocabId", "您的用户词表ID");
            string right = Post(url, dic);
            //注册键值
            Data_Save.Register(mess, right, DateTime.Now.ToString());
        }
    }
}

