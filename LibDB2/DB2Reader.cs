using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Collections;

namespace LibDB2
{
    public class DB2Reader
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
        private Dictionary<long, string> stringDic;

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

        /// <summary>
        /// 字段类型字典
        /// </summary>
        private Dictionary<string, Type> fieldsDic;

        public DataTable DT
        {
            get { return this.dt; }
            set { this.dt = value; }
        }

        public Dictionary<string, Type> FieldsDic
        {
            get { return fieldsDic; }
            set { fieldsDic = value; }
        }

        public DB2Reader()
        {
            this.dt = new DataTable();
            this.stringDic = new Dictionary<long, string>();
            this.fieldsDic = new Dictionary<string, Type>();
        }

        public void read(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName + " is not exists!");
            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            try
            {
                byte[] headerByte = br.ReadBytes(4);
                if (db2Flag != Encoding.UTF8.GetString(headerByte))
                    throw new NotSupportedException(fileName + "is invalid DB2 file!");
                this.rowCount = br.ReadInt32();
                this.colCount = br.ReadInt32();
                this.rowSize = br.ReadInt32();
                this.stringSize = br.ReadInt32();
                br.ReadUInt32();
                this.build = br.ReadUInt32();
                br.ReadUInt32();
                if (build > 0x3250)
                {
                    int num2 = br.ReadInt32();
                    int num3 = br.ReadInt32();
                    br.ReadInt32();
                    br.ReadInt32();
                    if (num3 != 0)
                    {
                        int num4 = (num3 - num2) + 1;
                        br.ReadBytes(num4 * 4);
                        br.ReadBytes(num4 * 2);
                    }
                }
                if (this.fieldsDic != null && this.fieldsDic.Count > 0 && this.fieldsDic.Count != colCount)
                    throw new NotSupportedException("The fields count for this db2 file is invalid. Please check your fieldsDic.");
                else if (this.fieldsDic == null || this.fieldsDic.Count == 0)
                {
                    //列类型全部使用int
                    for (int i = 0; i < this.colCount; i++)
                    {
                        this.dt.Columns.Add(i.ToString(), typeof(int));
                    }
                }
                else
                {
                    //根据列描述生成列
                    foreach (KeyValuePair<string, Type> kp in this.fieldsDic)
                    {
                        this.dt.Columns.Add(kp.Key, kp.Value);
                    }
                }
                //数据表的开始位置
                long rowDataPos = br.BaseStream.Position;
                //定位到字符串表
                br.BaseStream.Position += this.rowCount * this.rowSize;
                //字符串表的开始位置
                long strDataPos = br.BaseStream.Position;
                while (br.BaseStream.Position < strDataPos + this.stringSize)
                {
                    this.stringDic.Add(br.BaseStream.Position - strDataPos, readUtilZero(br));
                }
                //定位到数据表的开始位置
                br.BaseStream.Position = rowDataPos;
                for (int i = 0; i < this.rowCount; i++)
                {
                    long rowStartPos = br.BaseStream.Position;
                    DataRow row = this.dt.NewRow();
                    for (int j = 0; j < this.colCount; j++)
                    {
                        row[j] = getData(br, this.dt.Columns[j].DataType);
                    }
                    this.dt.Rows.Add(row);
                    if (br.BaseStream.Position > rowStartPos + this.rowSize)
                        br.BaseStream.Position += br.BaseStream.Position - rowStartPos - this.rowSize;
                }
            }
            //catch (Exception e)
            //{
            //    throw e;
            //}
            finally
            {
                br.Close();
                fs.Close();
            }
            
        }

        private Object getData(BinaryReader br, Type type)
        {
            object obj = DBNull.Value;
            switch (type.Name.ToLower())
            {
                case "long":
                    obj = br.ReadInt64();
                    break;
                case "ulong":
                    obj = br.ReadUInt64();
                    break;
                case "int32":
                    obj = br.ReadInt32();
                    break;
                case "uint32":
                    obj = br.ReadUInt32();
                    break;
                case "short":
                    obj = br.ReadInt16();
                    break;
                case "ushort":
                    obj = br.ReadUInt16();
                    break;
                case "sbyte":
                    obj = br.ReadSByte();
                    break;
                case "byte":
                    obj = br.ReadByte();
                    break;
                case "single":
                    obj = br.ReadSingle();
                    break;
                case "double":
                    obj = br.ReadDouble();
                    break;
                case "string":
                    int p = br.ReadInt32();
                    if (this.stringDic.ContainsKey(p))
                        obj = this.stringDic[p];
                    else
                        obj = string.Empty;
                    break;
                default:
                    obj = "the type of " + type.Name + " is not support!";
                    break;
            }
            return obj;
        }

        private string readUtilZero(BinaryReader br)
        {
            List<byte> list = new List<byte>();
            byte b;
            while ((b = br.ReadByte()) != 0)
            {
                list.Add(b);
            }
            return Encoding.UTF8.GetString(list.ToArray());
        }
    }
}
