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
        private static string configFileName;
        private static XmlDocument configFileDoc;
        static Configuration()
        {
            log = new Logger(typeof(Configuration));
            configFileName = "field.xml";
        }

        public static int getPageSize()
        {
            int r = 50;
            return r;
        }

        public static string getConfigFileName() { 
            return configFileName;
        }
        /// <summary>
        /// 设置配置文件名
        /// </summary>
        /// <param name="val">文件名</param>
        public static void setConfigFileName(string val)
        {
            configFileName = val;
            // 清除已缓存的配置文件内容
            configFileDoc = null;
        }

        public static string getItemTableName()
        {
            string res = null;
            try
            {
                XmlDocument doc = loadConfigFile();
                XmlNode node = doc.SelectSingleNode("/fields");
                if (node != null && node.Attributes["tablename"] != null)
                {
                    res = node.Attributes["tablename"].Value;
                }
            }
            catch (Exception e)
            {
                log.error("读取配置文件出错");
                log.error(e);
            }
            return res;
        }

        public static string getIdFieldName()
        {
            string res = null;
            try
            {
                XmlDocument doc = loadConfigFile();
                XmlNode node = doc.SelectSingleNode("/fields");
                if (node != null && node.Attributes["id"] != null)
                {
                    res = node.Attributes["id"].Value;
                }
            }
            catch (Exception e)
            {
                log.error("读取配置文件出错");
                log.error(e);
            }
            return res;
        }

        public static string getNameFieldName()
        {
            string res = null;
            try
            {
                XmlDocument doc = loadConfigFile();
                XmlNode node = doc.SelectSingleNode("/fields");
                if (node != null && node.Attributes["name"] != null)
                {
                    res = node.Attributes["name"].Value;
                }
            }
            catch (Exception e)
            {
                log.error("读取配置文件出错");
                log.error(e);
            }
            return res;
        }

        public static string getClassFieldName()
        {
            string res = null;
            try
            {
                XmlDocument doc = loadConfigFile();
                XmlNode node = doc.SelectSingleNode("/fields");
                if (node != null && node.Attributes["class"] != null)
                {
                    res = node.Attributes["class"].Value;
                }
            }
            catch (Exception e)
            {
                log.error("读取配置文件出错");
                log.error(e);
            }
            return res;
        }

        public static string getSubClassFieldName()
        {
            string res = null;
            try
            {
                XmlDocument doc = loadConfigFile();
                XmlNode node = doc.SelectSingleNode("/fields");
                if (node != null && node.Attributes["subclass"] != null)
                {
                    res = node.Attributes["subclass"].Value;
                }
            }
            catch (Exception e)
            {
                log.error("读取配置文件出错");
                log.error(e);
            }
            return res;
        }


        public static ItemProperty[] getItemProperties()
        {
            ArrayList list = new ArrayList();
            try
            {
                XmlDocument doc = loadConfigFile();
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
                log.error("读取配置文件出错");
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
        /// <summary>
        /// 获取配置文件名数组
        /// </summary>
        /// <returns>文件名数组</returns>
        public static string[] getConfigFileList() {
            try
            {
                string dirPath = AppDomain.CurrentDomain.BaseDirectory + "\\conf";
                string[] pathArr = Directory.GetFiles(dirPath);
                return pathArr.Select(path => path.Substring(path.LastIndexOf("\\") + 1)).ToArray();
            }
            catch (Exception e)
            {
                log.error("读取配置文件列表出错");
                log.error(e);
                return null;
            }
        }
        /// <summary>
        /// 加载配置文件内容
        /// </summary>
        /// <returns>XML文档实例</returns>
        private static XmlDocument loadConfigFile()
        {
            if (configFileDoc == null)
            {
                try
                {
                    string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\conf\\" + configFileName;
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filePath);
                    // 缓存到静态变量
                    configFileDoc = doc;
                }
                catch (Exception e)
                {
                    log.error("读取配置文件出错");
                    log.error(e);
                }
            }
            return configFileDoc;
        }
    }
}
