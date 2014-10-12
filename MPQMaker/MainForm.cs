using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LibDB2;
using LibMPQ;
using System.IO;

namespace MPQTester
{
    public partial class MainForm : Form
    {
        private DB2Reader reader;
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.reader = new DB2Reader();
            //reader.FieldsDic = FieldsManager.getFieldsDic("Item-sparse.db2", 12984);
            DateTime start = DateTime.Now;
            reader.read(Application.StartupPath + @"\Item-sparse.db2");
            TimeSpan cost = DateTime.Now - start;
            Console.WriteLine("read cost:" + cost.TotalSeconds + " sec");
            dataGridView1.DataSource = reader.DT;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DB2Writer writer = new DB2Writer(this.reader.DT);
            writer.Build = 14250;
            DateTime start = DateTime.Now;
            writer.saveTo(Application.StartupPath + @"\myItem.db2");
            TimeSpan cost = DateTime.Now - start;
            Console.WriteLine("write cost:" + cost.TotalSeconds + " sec");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            IntPtr hMpq = new IntPtr();
            string savePath = Application.StartupPath + @"\wow-update-14251.MPQ";
            string szFileName = Application.StartupPath + @"\Item.db2";
            string szFileName1 = Application.StartupPath + @"\Item-sparse.db2";
            string listfile = Application.StartupPath + @"\listfile";
            File.Delete(savePath);
            bool r = false;
            //r = MPQHelper.SFileOpenArchive(savePath, 0, 0, out hMpq);
            r = MPQHelper.SFileCreateArchive(savePath, MPQHelper.MPQ_CREATE_ATTRIBUTES | MPQHelper.MPQ_CREATE_ARCHIVE_V2, 4096, out hMpq);
            Console.WriteLine(r);
            r = MPQHelper.SFileAddFileEx(hMpq, szFileName, "zhCN\\DBFilesClient\\Item2.db2", 0x80010200, MPQHelper.MPQ_COMPRESSION_ZLIB, 0);
            Console.WriteLine(r);
            r = MPQHelper.SFileAddFileEx(hMpq, szFileName1, "zhCN\\DBFilesClient\\Item-sparse.db2", 0x80010200, MPQHelper.MPQ_COMPRESSION_ZLIB, 0);
            Console.WriteLine(r);
            //int i = MPQHelper.SFileAddListFile(hMpq, listfile);
            //Console.WriteLine(i);
            r = MPQHelper.SFileCompactArchive(hMpq, null, true);
            Console.WriteLine(r);
            r = MPQHelper.SFileCloseArchive(hMpq);
            Console.WriteLine(r);
        }
    }
}
