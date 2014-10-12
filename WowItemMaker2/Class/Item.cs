using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WowItemMaker2
{
    public class Item 
    {
        private ArrayList list_properties;

        public Item()
        {
            list_properties = new ArrayList();
        }

        public void addProperty(ItemProperty p)
        {
            list_properties.Add(p);
        }

        public ItemProperty getProperty(string name)
        {
            ItemProperty res = null;
            if (name != null && this.list_properties != null)
            {
                foreach (ItemProperty p in this.list_properties)
                {
                    if (p.Name.Trim().ToUpper() == name.Trim().ToUpper())
                        res = p;
                }
            }
            return res;
        }

        public void removeProperty(string name)
        {
            ItemProperty p = getProperty(name);
            if (p != null)
                this.list_properties.Remove(p);
        }

        public bool hasProperty(string name)
        {
            bool res = false;
            ItemProperty p = getProperty(name);
            if (p != null)
                res = true;
            return res;
        }

        public ItemProperty[] Properties
        {
            get { return (ItemProperty[])this.list_properties.ToArray(typeof(ItemProperty)); }
        }
    }
}
