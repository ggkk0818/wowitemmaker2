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
using System.Threading;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace WowItemMaker2
{
    /// <summary>
    /// Window_Conn.xaml 的交互逻辑
    /// </summary>
    public partial class Window_Conn : Window
    {
        private Logger log = new Logger(typeof(Window_Conn));
        public event EventHandler OnServerConnected;
        public Window_Conn()
        {
            InitializeComponent();
            DataTable dt = new DataTable();
            dt.Columns.Add("charset");
            dt.Rows.Add(new object[] { "gbk" });
            dt.Rows.Add(new object[] { "utf8" });
            CB_charset.DisplayMemberPath = "charset";
            CB_charset.ItemsSource = dt.DefaultView;
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            string host = TB_host.Text.Trim();
            string username = TB_username.Text.Trim();
            string password = TB_password.Password;
            string database = CB_database.Text.Trim();
            string charset = CB_charset.Text.Trim();
            string portStr = TB_port.Text.Trim();
            uint port = 3306;
            if (host == string.Empty || username == string.Empty || database == string.Empty)
            {
                MessageBox.Show("请填写连接信息。", "连接", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (!uint.TryParse(portStr, out port))
            {
                MessageBox.Show("请正确填写端口。", "连接", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            DBAccess db = new DBAccess(host, username, password, database, charset, port);
            Thread t = new Thread(new ParameterizedThreadStart(doConn));
            t.IsBackground = true;
            t.Start(db);
            LB_msg.Visibility = System.Windows.Visibility.Visible;
            IMG_loading.Visibility = System.Windows.Visibility.Visible;
            btn_close.IsEnabled = false;
            btn_ok.IsEnabled = false;
            this.Closing += Window_Closing;
        }

        void doConn(object obj)
        {
            EventHandler mi = new EventHandler(invokeConnDone);
            DBAccess db = obj as DBAccess;
            try
            {
                db.open();
                db.close();
                this.Dispatcher.Invoke(mi, new object[] { db, null });
            }
            catch (Exception err)
            {
                log.error(err);
                this.Dispatcher.Invoke(mi, new object[] { err, null });
            }
        }

        void invokeConnDone(object sender, EventArgs e)
        {
            if(sender == null)
                return;
            this.Closing -= Window_Closing;
            LB_msg.Visibility = System.Windows.Visibility.Hidden;
            IMG_loading.Visibility = System.Windows.Visibility.Hidden;
            btn_close.IsEnabled = true;
            btn_ok.IsEnabled = true;
            if (sender.GetType() == typeof(DBAccess))
            {
                if (this.OnServerConnected != null)
                    this.OnServerConnected(sender, null);
                this.Close();
            }
            else if (sender.GetType().BaseType == typeof(SystemException))
            {
                Exception err = sender as Exception;
                MessageBox.Show("发生错误。\r\n" + err.Message, "连接", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (sender.GetType().BaseType == typeof(DbException))
            {
                Exception err = sender as Exception;
                MessageBox.Show("连接失败。\r\n" + err.Message, "连接", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            object obj = e.Data.GetData(System.Windows.DataFormats.FileDrop);
            if (obj != null)
            {
                string[] arr_files = obj as String[];
                foreach (string filePath in arr_files)
                {

                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        void doGetDbList(object obj)
        {
            EventHandler mi = new EventHandler(invokeDbCombox);
            DBAccess db = obj as DBAccess;
            try
            {
                DataTable dt = db.query("show databases");
                this.Dispatcher.Invoke(mi, new object[] { dt, null });
            }
            catch
            {
                this.Dispatcher.Invoke(mi, new object[] { null, null });
            }
        }

        void invokeDbCombox(object sender, EventArgs e)
        {
            IMG_loading.Visibility = System.Windows.Visibility.Hidden;
            if (sender != null && sender.GetType() == typeof(DataTable))
            {
                DataTable dt = sender as DataTable;
                CB_database.DisplayMemberPath = dt.Columns[0].ColumnName;
                CB_database.ItemsSource = dt.DefaultView;
            }
        }

        private void CB_database_GotFocus(object sender, RoutedEventArgs e)
        {
            string host = TB_host.Text.Trim();
            string username = TB_username.Text.Trim();
            string password = TB_password.Password;
            string charset = CB_charset.Text.Trim();
            string portStr = TB_port.Text.Trim();
            uint port = 3306;
            if (host == string.Empty || username == string.Empty)
            {
                return;
            }
            if (!uint.TryParse(portStr, out port))
            {
                return;
            }
            DBAccess db = new DBAccess(host, username, password, null, charset, port);
            Thread t = new Thread(new ParameterizedThreadStart(doGetDbList));
            t.IsBackground = true;
            t.Start(db);
            IMG_loading.Visibility = System.Windows.Visibility.Visible;
        }

    }
}
