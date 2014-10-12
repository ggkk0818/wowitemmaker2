using System;
using System.Collections.Generic;
using System.Text;

namespace LibDB2
{
    public class FieldsManager
    {
        public static Dictionary<string, Type> getFieldsDic(string name, uint build)
        {
            Dictionary<string, Type> dic = new Dictionary<string, Type>();
            if (name.ToLower() == "Item.db2")
            {
                dic.Add("id", typeof(uint));
                dic.Add("class", typeof(uint));
                dic.Add("subclass", typeof(uint));
                dic.Add("unk0", typeof(int));
                dic.Add("material", typeof(int));
                dic.Add("displayid", typeof(uint));
                dic.Add("inventorytype", typeof(uint));
                dic.Add("sheath", typeof(uint));
            }
            else if (name.ToLower() == "item-sparse.db2")
            {
                if (build == 12984)
                {
                    for (int i = 0; i < 131; i++)
                    {
                        dic.Add("field" + i, typeof(int));
                    }
                    dic["field2"] = typeof(uint);
                    dic["field3"] = typeof(uint);
                    dic["field64"] = typeof(float);
                    dic["field96"] = typeof(string);
                    dic["field97"] = typeof(string);
                    dic["field98"] = typeof(string);
                    dic["field99"] = typeof(string);
                    dic["field124"] = typeof(float);
                    dic["field128"] = typeof(float);
                }
            }
            return dic;
        }
    }
}
