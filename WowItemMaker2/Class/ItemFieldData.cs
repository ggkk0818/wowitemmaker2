using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowItemMaker2
{
    public class ItemFieldData
    {
        private string _name;
        private string _value;
        private string _parent;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
    }
}
