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
using System.Windows.Media.Animation;
using System.Threading;
using System.Data.Common;

namespace WowItemMaker2
{
    /// <summary>
    /// Window_ItemInfo.xaml 的交互逻辑
    /// </summary>
    public partial class Window_ItemInfo : Window
    {
        public event EventHandler OnItemChanged;
        private ItemManager manager;
        private ItemProperty[] properties;
        private string itemId;

        public ItemManager Manager
        {
            get { return manager; }
            set { manager = value; }
        }

        public Window_ItemInfo(ItemManager manager)
        {
            InitializeComponent();
            this.manager = manager;
            this.itemId = null;
            btn_ok.IsEnabled = false;
            IMG_loading.Visibility = System.Windows.Visibility.Visible;
            LB_msg.Visibility = System.Windows.Visibility.Visible;
            Thread t = new Thread(new ParameterizedThreadStart(doGenerateProperties));
            t.IsBackground = true;
            t.Start();
        }

        public Window_ItemInfo(ItemManager manager, string itemId)
        {
            InitializeComponent();
            this.manager = manager;
            this.itemId = itemId;
            btn_ok.IsEnabled = false;
            IMG_loading.Visibility = System.Windows.Visibility.Visible;
            LB_msg.Visibility = System.Windows.Visibility.Visible;
            Thread t = new Thread(new ParameterizedThreadStart(doGenerateProperties));
            t.IsBackground = true;
            t.Start(itemId);
        }

        void doGenerateProperties(object obj)
        {
            EventHandler handler = new EventHandler(invokeGenerateUI);
            try
            {
                Item item = obj == null ? null : manager.getItem(obj.ToString());
                ItemProperty[] configProperties = Configuration.getItemProperties();
                ItemProperty[] dbProperties = manager.getItemProperties(Configuration.getItemTableName());
                foreach (ItemProperty dbPro in dbProperties)
                {
                    foreach (ItemProperty configPro in configProperties)
                    {
                        if (dbPro.Name.ToLower() == configPro.Name.ToLower())
                        {
                            dbPro.Child = configPro.Child;
                            dbPro.Data = configPro.Data;
                            dbPro.DisplayName = configPro.DisplayName;
                            dbPro.Parent = configPro.Parent;
                            dbPro.Regex = configPro.Regex;
                            if (configPro.Value != null && configPro.Value.Length > 0)
                                dbPro.Value = configPro.Value;
                            break;
                        }
                    }
                    if (item != null)
                    {
                        foreach (ItemProperty itemPro in item.Properties)
                        {
                            if (dbPro.Name.ToLower() == itemPro.Name.ToLower())
                            {
                                dbPro.Value = itemPro.Value;
                            }
                        }
                    }
                }
                this.Dispatcher.Invoke(handler, new object[] { dbProperties, null });
            }
            catch (Exception err)
            {
                this.Dispatcher.Invoke(handler, new object[] { err, null });
            }

        }

        void invokeGenerateUI(object sender, EventArgs e)
        {
            btn_ok.IsEnabled = true;
            IMG_loading.Visibility = System.Windows.Visibility.Hidden;
            LB_msg.Visibility = System.Windows.Visibility.Hidden;
            if (sender.GetType() == typeof(ItemProperty[]))
            {
                ItemProperty[] properties = sender as ItemProperty[];
                this.properties = properties;
                generateUI();
            }
            else if (sender.GetType().BaseType == typeof(SystemException))
            {
                Exception err = sender as Exception;
                MessageBox.Show("发生错误。\r\n" + err.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
            else if (sender.GetType().BaseType == typeof(DbException))
            {
                Exception err = sender as Exception;
                MessageBox.Show("数据库异常。\r\n" + err.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }

        }

        private void generateUI()
        {
            double width = this.Width;
            double height = this.Height;
            for (int i = 0; i < this.properties.Length; i++)
            {
                ItemProperty p = this.properties[i];
                Label lb = new Label();
                lb.Name = "LB_" + p.Name;
                lb.Opacity = 0;
                lb.Content = p.Name;
                if (p.DisplayName != null && p.DisplayName.Trim().Length > 0)
                    lb.Content = p.DisplayName;
                lb.Height = 28;
                lb.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                lb.Margin = new Thickness(0, 0, 0, 4);

                ComboBox cb = new ComboBox();
                cb.Name = "CB_" + p.Name;
                cb.IsEditable = true;
                cb.Opacity = 0;
                cb.Text = p.Value;
                cb.Height = 23;
                cb.SelectionChanged += new SelectionChangedEventHandler(cb_SelectionChanged);
                cb.MouseEnter += new MouseEventHandler(cb_MouseEnter);
                cb.MouseLeave += new MouseEventHandler(cb_MouseLeave);
                cb.Margin = new Thickness(0, 0, 5, 4);
                ItemFieldData[] items = null;
                string fieldName = p.Name;
                if (p.Data != null && p.Data.Trim().Length > 0)
                    fieldName = p.Data;
                if (p.Parent != null && p.Parent.Trim().Length > 0)
                {
                    // 读取父属性值对应数据文件
                    ItemProperty parent = this.properties.Where(parentProp => parentProp.Name.ToUpper() == p.Parent.Trim().ToUpper()).FirstOrDefault();
                    items = Configuration.getItemFieldData(fieldName.ToLower() + (parent != null ? parent.Value : string.Empty));
                }
                else
                    items = Configuration.getItemFieldData(fieldName.ToLower());
                if (items.Length > 0)
                {
                    cb.ItemsSource = items;
                    cb.DisplayMemberPath = "Name";
                    cb.SelectedValuePath = "Value";
                    cb.SelectedValue = p.Value;
                }

                Storyboard s = new Storyboard();
                this.Resources.Add(Guid.NewGuid().ToString(), s);
                DoubleAnimation da = new DoubleAnimation();
                Storyboard.SetTarget(da, lb);
                Storyboard.SetTargetProperty(da, new PropertyPath("Opacity", new object[] { }));
                da.From = 0;
                da.To = 1;
                da.FillBehavior = FillBehavior.HoldEnd;
                da.SpeedRatio = 1f;
                s.BeginTime = TimeSpan.FromSeconds(i * 0.02f);
                s.Duration = new Duration(TimeSpan.FromSeconds(1f));
                s.Children.Add(da);
                s.Completed += new EventHandler(s_Completed);
                s.Begin();

                Storyboard s1 = new Storyboard();
                this.Resources.Add(Guid.NewGuid().ToString(), s1);
                DoubleAnimation da1 = new DoubleAnimation();
                Storyboard.SetTarget(da1, cb);
                Storyboard.SetTargetProperty(da1, new PropertyPath("Opacity", new object[] { }));
                da1.From = 0;
                da1.To = 1;
                da1.FillBehavior = FillBehavior.HoldEnd;
                da1.SpeedRatio = 1f;
                s1.BeginTime = TimeSpan.FromSeconds(i * 0.02f);
                s1.Duration = new Duration(TimeSpan.FromSeconds(1f));
                s1.Children.Add(da1);
                s1.Completed += new EventHandler(s1_Completed);
                s1.Begin();
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(30);
                this.mainGrid.RowDefinitions.Add(row);
                this.mainGrid.Children.Add(cb);
                this.mainGrid.Children.Add(lb);
                cb.SetValue(Grid.ColumnProperty, 1);
                cb.SetValue(Grid.RowProperty, i);
                lb.SetValue(Grid.ColumnProperty, 0);
                lb.SetValue(Grid.RowProperty, i);
            }
        }

        void cb_MouseLeave(object sender, MouseEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            foreach (UIElement element in this.mainGrid.Children)
            {
                Label label = element as Label;
                if (label != null && label.Name.ToLower() == "lb_" + cb.Name.Substring(cb.Name.IndexOf('_') + 1).ToLower())
                {
                    label.Background = new SolidColorBrush(Colors.White);
                    cb.Background = label.Background;
                    break;
                }
            }
        }

        void cb_MouseEnter(object sender, MouseEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            foreach (UIElement element in this.mainGrid.Children)
            {
                Label label = element as Label;
                if (label != null && label.Name.Substring(3) == cb.Name.Substring(3))
                {
                    label.Background = new SolidColorBrush(Colors.AliceBlue);
                    cb.Background = label.Background;
                }
            }
        }

        void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            ItemFieldData data = cb.SelectedItem as ItemFieldData;
            ItemProperty pro = null;
            ItemProperty proChild = null;
            foreach (ItemProperty p in this.properties)
            {
                if (cb.Name.Substring(cb.Name.IndexOf('_') + 1).ToLower() == p.Name.ToLower())
                {
                    pro = p;
                    break;
                }
            }
            foreach (ItemProperty p in this.properties)
            {
                if (pro.Child == p.Name)
                {
                    proChild = p;
                    break;
                }
            }
            if (data == null || pro == null || proChild == null || pro.Child == null || pro.Child.Trim().Length == 0)
                return;
            foreach (UIElement element in this.mainGrid.Children)
            {
                ComboBox cbChild = element as ComboBox;
                if (cbChild != null && cbChild.Name.ToLower() == "cb_" + proChild.Name.ToLower())
                {
                    cbChild.ItemsSource = Configuration.getItemFieldData(pro.Child.ToLower(), data.Value, pro.Name);
                    cbChild.SelectedValue = proChild.Value;
                }
            }
        }

        void s1_Completed(object sender, EventArgs e)
        {
            ClockGroup group = sender as ClockGroup;
            Storyboard sb = group.Timeline as Storyboard;
            ComboBox cb = Storyboard.GetTarget((DoubleAnimation)sb.Children[0]) as ComboBox;
            string name = cb.Name;
            cb.Opacity = 1;
            sb.Remove();
            
        }

        void s_Completed(object sender, EventArgs e)
        {
            ClockGroup group = sender as ClockGroup;
            Storyboard sb = group.Timeline as Storyboard;
            Label lb = Storyboard.GetTarget((DoubleAnimation)sb.Children[0]) as Label;
            lb.Opacity = 1;
            sb.Remove();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            Item item = new Item();
            foreach (ItemProperty p in this.properties)
            {
                foreach (UIElement element in this.mainGrid.Children)
                {
                    ComboBox cb = element as ComboBox;
                    if (cb != null && cb.Name.Substring(3).ToLower() == p.Name.ToLower())
                    {
                        ItemProperty pro = p.Clone();
                        pro.Value = cb.SelectedValue == null ? cb.Text : cb.SelectedValue.ToString();
                        item.addProperty(pro);
                        break;
                    }
                }
            }
            IMG_loading.Visibility = System.Windows.Visibility.Visible;
            LB_msg.Visibility = System.Windows.Visibility.Visible;
            btn_ok.IsEnabled = false;
            btn_close.IsEnabled = false;
            Thread t = new Thread(new ParameterizedThreadStart(doExecute));
            t.IsBackground = true;
            t.Start(item);
        }

        void doExecute(object obj)
        {
            EventHandler handler = new EventHandler(invokeExecute);
            Item item = obj as Item;
            try
            {
                if (this.itemId == null)
                    this.manager.addItem(item);
                else
                    this.manager.updateItem(item);
                this.Dispatcher.Invoke(handler, new object[] { item, null });
            }
            catch (Exception err)
            {
                this.Dispatcher.Invoke(handler, new object[] { err, null });
            }
        }

        void invokeExecute(object sender, EventArgs e)
        {
            IMG_loading.Visibility = System.Windows.Visibility.Hidden;
            LB_msg.Visibility = System.Windows.Visibility.Hidden;
            btn_ok.IsEnabled = true;
            btn_close.IsEnabled = true;
            if (sender.GetType() == typeof(Item))
            {
                if (this.OnItemChanged != null)
                    this.OnItemChanged(sender, e);
                this.Close();
            }
            else if (sender.GetType().BaseType == typeof(SystemException))
            {
                Exception err = sender as Exception;
                MessageBox.Show("发生错误。\r\n" + err.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (sender.GetType().BaseType == typeof(DbException))
            {
                Exception err = sender as Exception;
                MessageBox.Show("数据库异常。\r\n" + err.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
