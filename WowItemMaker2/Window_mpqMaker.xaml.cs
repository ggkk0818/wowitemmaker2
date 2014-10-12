using System;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Data;
using System.Collections;
using LibDB2;
using LibMPQ;

namespace WowItemMaker2
{
    /// <summary>
    /// Window_mpqMaker.xaml 的交互逻辑
    /// </summary>
    public partial class Window_mpqMaker : Window
    {
        private Logger log = new Logger(typeof(Window_mpqMaker));
        private static string btn_text_generate = "生成";
        private static string btn_text_stop = "停止";
        private ItemManager manager;
        private SaveFileDialog saveFileDialog;
        private Thread myThread;
        public Window_mpqMaker(ItemManager manager)
        {
            InitializeComponent();
            this.manager = manager;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.myThread = null;
            this.saveFileDialog = new SaveFileDialog();
            this.saveFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            this.saveFileDialog.Filter = "*.MPQ|MPQ文件|*.*|所有文件";
            this.saveFileDialog.FileName = "wow-update-14251.MPQ";
            string path = System.Windows.Forms.Application.StartupPath + @"\wow-update-14251.MPQ";
            TB_path.Text = path;
        }

        private void Btn_broswer_Click(object sender, RoutedEventArgs e)
        {
            DialogResult r = this.saveFileDialog.ShowDialog();
            if (r == System.Windows.Forms.DialogResult.OK)
            {
                TB_path.Text = this.saveFileDialog.FileName;
            }
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_Generate_Click(object sender, RoutedEventArgs e)
        {
            if (Btn_Generate.Content.ToString() == btn_text_generate)
            {
                string path = TB_path.Text.Trim();
                //建立保存路径文件夹
                try
                {
                    string dirName = Path.GetDirectoryName(path);
                    if (!Directory.Exists(dirName))
                        Directory.CreateDirectory(dirName);
                }
                catch (Exception err)
                {
                    System.Windows.Forms.MessageBox.Show("获取保存路径信息出错，请确认路径是否正确！" + err.Message, this.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //删除已经存在的文件
                if (File.Exists(path))
                {
                    DialogResult r = System.Windows.Forms.MessageBox.Show("生成问号补丁将会覆盖原来的文件，是否继续？", this.Title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    if (r != System.Windows.Forms.DialogResult.Yes)
                        return;
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception err)
                    {
                        System.Windows.Forms.MessageBox.Show("删除文件失败！" + err.Message, this.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                //开启线程
                this.myThread = new Thread(new ParameterizedThreadStart(doGenerate));
                this.myThread.IsBackground = true;
                this.myThread.Start(path);
                this.Closing += Window_Closing;
                LB_msg.Visibility = System.Windows.Visibility.Visible;
                IMG_loading.Visibility = System.Windows.Visibility.Visible;
                Btn_Generate.Content = btn_text_stop;
            }
            else
            {
                if (this.myThread != null)
                {
                    try
                    {
                        this.myThread.Abort();
                        Btn_Generate.IsEnabled = false;
                    }
                    catch (Exception err)
                    {
                        log.error("abort thread exception:");
                        log.error(err);
                    }
                }
            }
        }

        void doGenerate(object obj)
        {
            EventHandler mi = new EventHandler(invokeGenerate);
            EventHandler miDone = new EventHandler(invokeDone);
            try
            {
                string mpqPath = obj.ToString();
                string itemPath = System.Windows.Forms.Application.StartupPath + "\\Item.db2";
                string itemSparsePath = System.Windows.Forms.Application.StartupPath + "\\Item-sparse.db2";
                string itemSQL = "select entry,class,subclass,unk0,material,displayid,inventorytype,sheath from " + Configuration.getItemTableName();
                string itemSparseSQL = "select entry,quality,flags,flagsextra,buyprice,sellprice,inventorytype,allowableclass,allowablerace,itemlevel,requiredlevel,requiredskill,requiredskillrank,requiredspell,requiredhonorrank,requiredcityrank,requiredreputationfaction,requiredreputationrank,maxcount,stackable,containerslots,stat_type1,stat_type2,stat_type3,stat_type4,stat_type5,stat_type6,stat_type7,stat_type8,stat_type9,stat_type10,stat_value1,stat_value2,stat_value3,stat_value4,stat_value5,stat_value6,stat_value7,stat_value8,stat_value9,stat_value10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,scalingstatdistribution,damagetype,delay,rangedmodrange,spellid_1,spellid_2,spellid_3,spellid_4,spellid_5,spelltrigger_1,spelltrigger_2,spelltrigger_3,spelltrigger_4,spelltrigger_5,spellcharges_1,spellcharges_2,spellcharges_3,spellcharges_4,spellcharges_5,spellcooldown_1,spellcooldown_2,spellcooldown_3,spellcooldown_4,spellcooldown_5,spellcategory_1,spellcategory_2,spellcategory_3,spellcategory_4,spellcategory_5,spellcategorycooldown_1,spellcategorycooldown_2,spellcategorycooldown_3,spellcategorycooldown_4,spellcategorycooldown_5,bonding,name,'','','',description,pagetext,languageid,pagematerial,startquest,lockid,material,sheath,randomproperty,randomsuffix,itemset,maxdurability,area,map,bagfamily,totemcategory,socketcolor_1,socketcolor_2,socketcolor_3,socketcontent_1,socketcontent_2,socketcontent_3,socketbonus,gemproperties,armordamagemodifier,duration,itemlimitcategory,holidayid,0,0,0 from " + Configuration.getItemTableName();
                //生成Item.db2
                this.Dispatcher.Invoke(mi, new object[] { "查询item信息...", null });
                log.debug("Execute SQL:");
                log.debug(itemSQL);
                DataTable dt = this.manager.Db.query(itemSQL);
                this.Dispatcher.Invoke(mi, new object[] { "整理数据结构...", null });
                DataTable dt_item = new DataTable();
                dt_item.Columns.Add(string.Empty, typeof(uint));
                dt_item.Columns.Add(string.Empty, typeof(uint));
                dt_item.Columns.Add(string.Empty, typeof(uint));
                dt_item.Columns.Add(string.Empty, typeof(int));
                dt_item.Columns.Add(string.Empty, typeof(int));
                dt_item.Columns.Add(string.Empty, typeof(uint));
                dt_item.Columns.Add(string.Empty, typeof(uint));
                dt_item.Columns.Add(string.Empty, typeof(uint));
                foreach (DataRow dr in dt.Rows)
                {
                    dt_item.Rows.Add(dr.ItemArray);
                }
                dt.Dispose();
                dt = null;
                DB2Writer writer = new DB2Writer(dt_item);
                writer.Build = 14250;
                this.Dispatcher.Invoke(mi, new object[] { "生成Item.dbc...", null });
                writer.saveTo(itemPath);
                writer.Dispose();
                //生成Item-sparse.db2
                this.Dispatcher.Invoke(mi, new object[] { "查询item-sparse信息...", null });
                log.debug("Execute SQL:");
                log.debug(itemSparseSQL);
                dt = this.manager.Db.query(itemSparseSQL);
                this.Dispatcher.Invoke(mi, new object[] { "整理数据结构...", null });
                DataTable dt_itemSparse = new DataTable();
                for (int i = 0; i < 131; i++)
                {
                    dt_itemSparse.Columns.Add(string.Empty, typeof(int));
                }
                dt_itemSparse.Columns[2].DataType = typeof(uint);
                dt_itemSparse.Columns[3].DataType = typeof(uint);
                dt_itemSparse.Columns[64].DataType = typeof(float);
                dt_itemSparse.Columns[96].DataType = typeof(string);
                dt_itemSparse.Columns[97].DataType = typeof(string);
                dt_itemSparse.Columns[98].DataType = typeof(string);
                dt_itemSparse.Columns[99].DataType = typeof(string);
                dt_itemSparse.Columns[100].DataType = typeof(string);
                dt_itemSparse.Columns[124].DataType = typeof(float);
                dt_itemSparse.Columns[128].DataType = typeof(float);
                foreach (DataRow dr in dt.Rows)
                {
                    object[] values = dr.ItemArray;
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (dt_itemSparse.Columns[i].DataType == typeof(uint))
                        {
                            if (dt.Columns[i].DataType == typeof(int))
                            {
                                values[i] = (uint)((int)values[i] & 0xffffffff);
                            }
                            else if (dt.Columns[i].DataType == typeof(long))
                            {
                                values[i] = (uint)((long)values[i] & 0xffffffff);
                            }
                        }
                    }
                    dt_itemSparse.Rows.Add(values);
                }
                dt.Dispose();
                dt = null;
                writer.process(dt_itemSparse);
                writer.Build = 14250;
                this.Dispatcher.Invoke(mi, new object[] { "生成Item-sparse.db2...", null });
                writer.saveTo(itemSparsePath);
                bool r = false;
                IntPtr hMpq = new IntPtr();
                r = MPQHelper.SFileCreateArchive(mpqPath, MPQHelper.MPQ_CREATE_ATTRIBUTES | MPQHelper.MPQ_CREATE_ARCHIVE_V2, 4096, out hMpq);
                if (!r)
                    throw new Exception("创建mpq文件失败。");
                r = MPQHelper.SFileAddFileEx(hMpq, itemPath, "zhCN\\DBFilesClient\\Item2.db2", 0x80010200, MPQHelper.MPQ_COMPRESSION_ZLIB, 0);
                if (!r)
                    throw new Exception("写入mpq数据失败。");
                r = MPQHelper.SFileAddFileEx(hMpq, itemSparsePath, "zhCN\\DBFilesClient\\Item-sparse.db2", 0x80010200, MPQHelper.MPQ_COMPRESSION_ZLIB, 0);
                if (!r)
                    throw new Exception("写入mpq数据失败。");
                r = MPQHelper.SFileCompactArchive(hMpq, null, true);
                if (!r)
                    throw new Exception("编译mpq失败。");
                r = MPQHelper.SFileCloseArchive(hMpq);
                if (!r)
                    throw new Exception("关闭mpq文件失败。");
                this.Dispatcher.Invoke(miDone, new object[] { "生成成功！", null });
            }
            catch (ThreadAbortException err)
            {
                this.Dispatcher.Invoke(miDone, new object[] { null, null });
            }
            catch (Exception err)
            {
                log.error("制作问号补丁出错：");
                log.error(err);
                this.Dispatcher.Invoke(miDone, new object[] { "制作问号补丁出错。" + err.Message, null });
            }
        }

        void invokeGenerate(object sender, EventArgs e)
        {
            if (sender == null)
                return;
            LB_msg.Content = sender.ToString();
        }

        void invokeDone(object sender, EventArgs e)
        {
            this.Closing -= Window_Closing;
            LB_msg.Visibility = System.Windows.Visibility.Hidden;
            IMG_loading.Visibility = System.Windows.Visibility.Hidden;
            Btn_Generate.Content = btn_text_generate;
            Btn_Generate.IsEnabled = true;
            if (sender != null)
            {
                System.Windows.Forms.MessageBox.Show(sender.ToString(), this.Title, MessageBoxButtons.OK);
            }
            this.myThread = null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

    }
}
