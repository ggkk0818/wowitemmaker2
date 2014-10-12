using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WowItemMaker2
{
    /// <summary>
    /// Window_About.xaml 的交互逻辑
    /// </summary>
    public partial class Window_About : Window
    {
        public Window_About()
        {
            InitializeComponent();
            string[] assembly = this.GetType().Assembly.FullName.Split(',');
            LB_version.Content = assembly[1].Substring(9);
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
