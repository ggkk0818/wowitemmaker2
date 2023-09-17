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
using System.Xml;
using System.IO;

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
            // 字符编码选项
            DataTable dt = new DataTable();
            dt.Columns.Add("charset");
            dt.Rows.Add(new object[] { "gbk" });
            dt.Rows.Add(new object[] { "utf8" });
            CB_charset.DisplayMemberPath = "charset";
            CB_charset.ItemsSource = dt.DefaultView;
            // 字段配置选项
            string[] configFileArr = Configuration.getConfigFileList();
            dt = new DataTable();
            dt.Columns.Add("name");
            foreach (string fileName in configFileArr)
            {
                dt.Rows.Add(new object[] { fileName });
            }
            CB_configFile.DisplayMemberPath = "name";
            CB_configFile.ItemsSource = dt.DefaultView;
            CB_configFile.Text = configFileArr.Length > 0 ? configFileArr[0] : string.Empty;
            // 加载保存的信息
            try
            {
                this.loadConnInfoXml();
            }
            catch { }
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
                // 设置数据库字段配置文件
                if (CB_configFile.Text != null && CB_configFile.Text != string.Empty)
                    Configuration.setConfigFileName(CB_configFile.Text);
                if (this.OnServerConnected != null)
                    this.OnServerConnected(sender, null);
                // 保存连接信息
                this.saveConnInfoXml();
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
        /// <summary>
        /// 将连接信息保存到文件
        /// </summary>
        private void saveConnInfoXml()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Data\\ConnInfo.cfg";
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                if (CB_saveInfo.IsChecked == true)
                {
                    string pwdEncrypted = string.Empty;
                    if (TB_password.Password.Length > 0)
                    {
                        pwdEncrypted = Util.Encrypt(TB_password.Password);
                    }
                    XmlWriter xmlr = XmlWriter.Create(filePath);
                    xmlr.WriteStartElement("WOWItemMaker");
                    xmlr.WriteElementString("HostName", TB_host.Text.Trim());
                    xmlr.WriteElementString("Port", TB_port.Text.Trim());
                    xmlr.WriteElementString("UserName", TB_username.Text.Trim());
                    xmlr.WriteElementString("Password", pwdEncrypted);
                    xmlr.WriteElementString("DataBase", CB_database.Text.Trim());
                    xmlr.WriteElementString("Charset", CB_charset.Text.Trim());
                    xmlr.WriteElementString("ConfigFile", CB_configFile.Text.Trim());
                    xmlr.WriteEndElement();
                    xmlr.Flush();
                    xmlr.Close();
                }
            }
            catch (Exception e)
            {
                log.warn("保存连接信息出错");
                log.warn(e);
            }
        }
        /// <summary>
        /// 读取回显连接信息
        /// </summary>
        private void loadConnInfoXml()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Data\\ConnInfo.cfg";
            if (File.Exists(filePath))
            {
                XmlReader xmlr = XmlReader.Create(filePath);
                xmlr.Read();
                xmlr.ReadToNextSibling("WOWItemMaker");
                xmlr.ReadToDescendant("HostName");
                TB_host.Text = xmlr.ReadString();
                xmlr.ReadToNextSibling("Port");
                TB_port.Text = xmlr.ReadString();
                xmlr.ReadToNextSibling("UserName");
                TB_username.Text = xmlr.ReadString();
                xmlr.ReadToNextSibling("Password");
                string pwd = xmlr.ReadString();
                if (pwd != string.Empty)
                {
                    try
                    {
                        pwd = Util.Decrypt(pwd);
                    }
                    catch(Exception e)
                    {
                        pwd = string.Empty;
                        log.warn("解析密码出错");
                        log.warn(e);
                    }
                }
                TB_password.Password = pwd;
                xmlr.ReadToNextSibling("DataBase");
                CB_database.Text = xmlr.ReadString();
                xmlr.ReadToNextSibling("Charset");
                CB_charset.Text = xmlr.ReadString();
                xmlr.ReadToNextSibling("ConfigFile");
                CB_configFile.Text = xmlr.ReadString();
                xmlr.Close();
                CB_saveInfo.IsChecked = true;
            }
        }

    }
}
