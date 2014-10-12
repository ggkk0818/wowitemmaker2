using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace WowItemMaker2
{
    public class ObjectBuilder
    {
        public static DataTable Items2DataTable(Item[] items)
        {
            DataTable dt = new DataTable();
            if (items != null && items.Length > 0)
            {
                Dictionary<string, ItemFieldData[]> dic_itemfields = new Dictionary<string, ItemFieldData[]>();
                ItemProperty[] properties = items[0].Properties;
                foreach (ItemProperty p in properties)
                {
                    DataColumn c = new DataColumn();
                    c.ColumnName = p.Name;
                    c.Caption = p.DisplayName;
                    c.DataType = p.ValueType == null ? typeof(String) : p.ValueType;
                    dt.Columns.Add(c);
                    string fieldName = p.Name;
                    if (p.Data != null && p.Data.Trim() != string.Empty)
                        fieldName = p.Data;
                    if (dic_itemfields.ContainsKey(fieldName))
                    {
                        c.DataType = typeof(String);
                        continue;
                    }
                    ItemFieldData[] fieldData = Configuration.getItemFieldData(fieldName);
                    if (fieldData.Length > 0)
                    {
                        dic_itemfields.Add(fieldName, fieldData);
                        c.DataType = typeof(String);
                    }
                }
                DataColumn clm_edit = new DataColumn("editColumn");
                dt.Columns.Add(clm_edit);
                DataColumn clm_del = new DataColumn("delColumn");
                dt.Columns.Add(clm_del);
                foreach (Item item in items)
                {
                    ItemProperty[] arr_p = item.Properties;
                    object[] objs = new object[arr_p.Length + 2];
                    for (int i = 0; i < arr_p.Length; i++)
                    {
                        ItemProperty p = arr_p[i];
                        string fieldName = p.Name;
                        string value = p.Value;
                        if (p.Data != null && p.Data.Trim() != string.Empty)
                            fieldName = p.Data;
                        if (p.Parent != null && p.Parent.Trim() != string.Empty)
                        {
                            ItemProperty pp = item.getProperty(p.Parent);
                            if (pp != null)
                            {
                                fieldName += pp.Value;
                                if (!dic_itemfields.ContainsKey(fieldName))
                                {
                                    ItemFieldData[] fieldData = Configuration.getItemFieldData(fieldName);
                                    if (fieldData.Length > 0)
                                    {
                                        dic_itemfields.Add(fieldName, fieldData);
                                        dt.Columns[i].DataType = typeof(String);
                                    }
                                }
                                else
                                {
                                    dt.Columns[i].DataType = typeof(String);
                                }
                            }
                        }

                        if (dic_itemfields.ContainsKey(fieldName))
                        {
                            ItemFieldData data = getFieldDataByValue(value, dic_itemfields[fieldName]);
                            if(data != null && data.Value != null && data.Value.Trim() != string.Empty)
                                value = data.Name;
                        }
                        objs[i] = value;
                    }
                    objs[objs.Length - 2] = "修改";
                    objs[objs.Length - 1] = "删除";
                    dt.Rows.Add(objs);
                }
            }
            return dt;
        }

        private static ItemFieldData getFieldDataByValue(string value, ItemFieldData[] fieldData)
        {
            foreach (ItemFieldData data in fieldData)
            {
                if (data.Value == value)
                    return data;
            }
            return null;
        }
    }
}
