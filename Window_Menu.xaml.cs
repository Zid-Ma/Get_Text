using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Get_Text
{
	/// <summary>
	/// Window_Menu.xaml 的交互逻辑
	/// </summary>
	public partial class Window_Menu : Window
	{
		public Window_Menu()
		{
			InitializeComponent();
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			if (button.Content == "启动")
			{
				button.Content = "关闭";
				MainWindow.active = true;
			}
			else
			{
				button.Content = "启动";
				MainWindow.active = false;
			}
			this.Hide();
			MainWindow.Menu_IsVisible = true;
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			button.Content = "启动";
			MainWindow.active = false;
			if ((string)button1.Content == "本地翻译>")
			{
				button1.Content = "在线翻译>";
				MainWindow.translate_mode = 1;
			}
			else if ((string)button1.Content == "在线翻译>")
			{
				button1.Content = "录入模式>";
				MainWindow.translate_mode = 2;
			}
			else if ((string)button1.Content == "录入模式>")
			{
				button1.Content = "本地翻译>";
				MainWindow.translate_mode = 0;
			}
			else
			{
				button1.Content = "本地翻译>";
				MainWindow.translate_mode = 0;
			}
		}

		private void button2_Click(object sender, RoutedEventArgs e)
		{
			if((string)button2.Content == "有道在线>")
			{
				button2.Content = "百度在线>";
				MainWindow.online_mode = 1;
			}
			else
			{
				button2.Content = "有道在线>";
				MainWindow.online_mode = 0;
			}
		}

		private void button4_Click(object sender, RoutedEventArgs e)
		{
			this.Hide();
			//MainWindow.Menu_IsVisible = true;
		}

		private void Window_LostFocus(object sender, RoutedEventArgs e)
		{
			this.Hide();
			//MainWindow.Menu_IsVisible = true;
		}

		private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			//this.Hide();
			//MainWindow.Menu_IsVisible = true;
		}
	}
}
