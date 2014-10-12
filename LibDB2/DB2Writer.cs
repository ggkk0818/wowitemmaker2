using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace LibDB2
{
    public class DB2Writer
    {
        /// <summary>
        /// db2文件头
        /// </summary>
        private static string db2Flag = "WDB2";

        /// <summary>
        /// 数据表
        /// </summary>
        private DataTable dt;

        /// <summary>
        /// 字符串表
        /// </summary>
        private Dictionary<string, int> stringDic;

        /// <summary>
        /// 行数
        /// </summary>
        private int rowCount;

        /// <summary>
        /// 列数
        /// </summary>
        private int colCount;

        /// <summary>
        /// 行大小
        /// </summary>
        private int rowSize;

        /// <summary>
        /// 字符串表长度
        /// </summary>
        private int stringSize;

        /// <summary>
        /// 文件版本号
        /// </summary>
        private uint build;

        public uint Build
        {
            get { return build; }
            set { build = value; }
        }

        public DB2Writer()
        {
            this.dt = new DataTable();
        }

        public DB2Writer(DataTable dt)
        {
            this.process(dt);
        }

        public void process(DataTable dt)
        {
            if (dt == null)
                return;
            this.dt = dt;
            this.colCount = dt.Columns.Count;
            this.rowCount = dt.Rows.Count;
            this.rowSize = getRowSize(this.dt.Columns);
            this.stringDic = getStringDic(this.dt, out this.stringSize);
        }

        public void saveTo(string filepath)
        {
            string dirPath = Path.GetDirectoryName(filepath);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            FileStream fs = null;
            try
            {
                fs = new FileStream(filepath, FileMode.Create);
                #region 写文件头
                fs.Write(Encoding.UTF8.GetBytes(db2Flag), 0, db2Flag.Length);
                fs.Write(Convert.getBytes(this.rowCount), 0, 4);
                fs.Write(Convert.getBytes(this.colCount), 0, 4);
                fs.Write(Convert.getBytes(this.rowSize), 0, 4);
                fs.Write(Convert.getBytes(this.stringSize), 0, 4);
                fs.Write(Convert.getBytes(0u), 0, 4);
                fs.Write(Convert.getBytes(this.build), 0, 4);
                fs.Write(Convert.getBytes(0u), 0, 4);
                if (build > 0x3250)
                {
                    fs.Position += 16;
                    //fs.Write(Convert.getBytes(0), 0, 4);
                    //fs.Write(Convert.getBytes(0), 0, 4);
                    //fs.Write(Convert.getBytes(0), 0, 4);
                    //fs.Write(Convert.getBytes(0), 0, 4);
                }
                #endregion
                #region 写数据表
                for (int i = 0; i < this.dt.Rows.Count; i++)
                {
                    for (int j = 0; j < this.dt.Columns.Count; j++)
                    {
                        object value = this.dt.Rows[i][j];
                        Type dataType = this.dt.Columns[j].DataType;
                        if (dataType == typeof(string) || !isSupport(dataType))
                        {
                            dataType = typeof(int);
                            if (this.stringDic.ContainsKey(value.ToString()))
                            {
                                value = this.stringDic[value.ToString()];
                            }
                            else
                            {
                                value = 0;
                            }
                        }
                        byte[] data = Convert.getBytes(value, dataType);
                        fs.Write(data, 0, data.Length);
                    }
                }
                #endregion
                #region 写字符串表
                foreach (KeyValuePair<string, int> kp in this.stringDic)
                {
                    byte[] data = Encoding.UTF8.GetBytes(kp.Key);
                    fs.Write(data, 0, data.Length);
                    fs.WriteByte(0);
                }
                #endregion

            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }

        }

        private int getRowSize(DataColumnCollection dcc)
        {
            int r = 0;
            foreach (DataColumn col in dcc)
            {
                switch (col.DataType.Name.ToLower())
                {
                    case "long":
                        r += 8;
                        break;
                    case "ulong":
                        r += 8;
                        break;
                    case "int32":
                        r += 4;
                        break;
                    case "uint32":
                        r += 4;
                        break;
                    case "short":
                        r += 2;
                        break;
                    case "ushort":
                        r += 2;
                        break;
                    case "sbyte":
                        r += 1;
                        break;
                    case "byte":
                        r += 1;
                        break;
                    case "single":
                        r += 4;
                        break;
                    case "double":
                        r += 8;
                        break;
                    case "string":
                        r += 4;
                        break;
                    default:
                        r += 4;
                        break;
                }
            }
            return r;
        }

        private Dictionary<string, int> getStringDic(DataTable dt, out int stringSize)
        {
            Dictionary<string, int> stringDic = new Dictionary<string, int>();
            stringDic.Add(string.Empty, 0);
            int strSize = 1;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Columns[j].DataType == typeof(string) || !isSupport(dt.Columns[j].DataType))
                    {
                        string value = dt.Rows[i][j].ToString();
                        if (!stringDic.ContainsKey(value))
                        {
                            stringDic.Add(value, strSize);
                            strSize += Encoding.UTF8.GetByteCount(value) + 1;
                        }
                    }
                }
            }
            stringSize = strSize;
            return stringDic;
        }

        private bool isSupport(Type type)
        {
            return type == typeof(long) ? true : 
                type == typeof(ulong) ? true : 
                type == typeof(int) ? true : 
                type == typeof(uint) ? true :
                type == typeof(short) ? true :
                type == typeof(ushort) ? true :
                type == typeof(sbyte) ? true :
                type == typeof(byte) ? true :
                type == typeof(float) ? true :
                type == typeof(double) ? true :
                type == typeof(string) ? true :
                false;
        }

        public void Dispose()
        {
            this.stringDic = null;
            this.dt = null;
        }
    }
}
