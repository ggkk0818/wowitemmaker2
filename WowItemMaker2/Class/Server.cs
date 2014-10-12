using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WowItemMaker2.Class
{
    public class Server
    {
        private string _host;
        private string _userName;
        private string _password;
        private string _charSet;
        private uint _port;

        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string CharSet
        {
            get { return _charSet; }
            set { _charSet = value; }
        }

        public uint Port
        {
            get { return _port; }
            set { _port = value; }
        }


    }
}
