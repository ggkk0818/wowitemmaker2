using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml;

namespace WowItemMaker2
{
    public static class Configuration
    {
        private static Logger log;
        static Configuration()
        {
            log = new Logger(typeof(Configuration));
        }

        public static int getPageSize()
        {
            int r = 50;
            return r;
        }

        public static string getItemTableName()
        {
            string res = null;
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\conf\\field.xml";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNode node = doc.SelectSingleNode("/fields");
                if (node != null && node.Attributes["tablename"] != null)
                {
                    res = node.Attributes["tablename"].Value;
                }
            }
            catch(Exception e)
            {
                log.error("读取配置文件出错" + filePath);
                log.error(e);
            }
            return res;
        }

        public static string getIdFieldName()
        {
            string res = null;
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\conf\\field.xml";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNode node = doc.SelectSingleNode("/fields");
                if (node != null && node.Attributes["id"] != null)
                {
                    res = node.Attributes["id"].Value;
                }
            }
            catch (Exception e)
            {
                log.error("读取配置文件出错" + filePath);
                log.error(e);
            }
            return res;
        }


        public static ItemProperty[] getItemProperties()
        {
            ArrayList list = new ArrayList();
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\conf\\field.xml";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNodeList nodes = doc.SelectNodes("/fields/field");
                foreach (XmlNode node in nodes)
                {
                    ItemProperty pro = new ItemProperty();
                    pro.Child = node.Attributes["child"].Value;
                    pro.DisplayName = node.Attributes["displayname"].Value;
                    pro.Name = node.Attributes["name"].Value;
                    pro.Parent = node.Attributes["parent"].Value;
                    pro.Regex = node.Attributes["regex"].Value;
                    pro.Value = node.Attributes["default"].Value;
                    pro.Data = node.Attributes["data"].Value;
                    string typeStr = node.Attributes["type"].Value;
                    if (typeStr != null && typeStr.Trim() != string.Empty)
                    {
                        pro.ValueType = Type.GetType(typeStr);
                    }
                    list.Add(pro);
                }
            }
            catch (Exception e)
            {
                log.error("读取配置文件出错" + filePath);
                log.error(e);
            }
            return (ItemProperty[])list.ToArray(typeof(ItemProperty));
        }

        public static ItemFieldData[] getItemFieldData(string fieldName)
        {
            return getItemFieldData(fieldName, string.Empty, string.Empty);
        }

        public static ItemFieldData[] getItemFieldData(string fieldName, string child, string parent)
        {
            ArrayList list = new ArrayList();
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Data\\" + fieldName + child + ".txt";
            if (File.Exists(filePath))
            {
                try
                {
                    StreamReader sr = new StreamReader(filePath);
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        int pos = line.IndexOf(',');
                        if (pos > 0)
                        {
                            string value = line.Substring(0, pos);
                            string name = line.Substring(pos + 1, line.Length - pos - 1);
                            ItemFieldData data = new ItemFieldData();
                            data.Name = name;
                            data.Value = value;
                            data.Parent = parent;
                            list.Add(data);
                        }
                    }
                }
                catch (Exception e)
                {
                    log.error("读取配置文件出错" + filePath);
                    log.error(e);
                }
            }
            return (ItemFieldData[])list.ToArray(typeof(ItemFieldData));
        }
    }
}
