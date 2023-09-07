using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WowItemMaker2
{
    public class Logger
    {
        private Type type;
        private static StreamWriter sw;
        public Logger(Type type)
        {
            this.type = type;
            string basePath = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\";
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            if (Logger.sw == null)
                Logger.sw = new StreamWriter(basePath + "app.log", true);
        }

        public void debug(object o)
        {
            StringBuilder sb = new StringBuilder("DEBUG ");
            DateTime now = DateTime.Now;
            sb.Append(now.ToString("yyyy-MM-dd HH:mm:ss.SSS "));
            if(this.type != null)
                sb.Append(this.type.FullName + " ");
            if (o != null)
                sb.Append(o);
            sw.WriteLine(sb.ToString());
            sw.Flush();
        }

        public void info(object o)
        {
            StringBuilder sb = new StringBuilder("INFO ");
            DateTime now = DateTime.Now;
            sb.Append(now.ToString("yyyy-MM-dd HH:mm:ss.SSS "));
            if (this.type != null)
                sb.Append(this.type.FullName + " ");
            if (o != null)
                sb.Append(o);
            sw.WriteLine(sb.ToString());
            sw.Flush();
        }

        public void warn(object o)
        {
            StringBuilder sb = new StringBuilder("WARN ");
            DateTime now = DateTime.Now;
            sb.Append(now.ToString("yyyy-MM-dd HH:mm:ss.SSS "));
            if (this.type != null)
                sb.Append(this.type.FullName + " ");
            if (o != null)
                sb.Append(o);
            sw.WriteLine(sb.ToString());
            sw.Flush();
        }

        public void error(object o)
        {
            StringBuilder sb = new StringBuilder("ERROR ");
            DateTime now = DateTime.Now;
            sb.Append(now.ToString("yyyy-MM-dd HH:mm:ss.SSS "));
            if (this.type != null)
                sb.Append(this.type.FullName + " ");
            if (o != null)
                sb.Append(o);
            sw.WriteLine(sb.ToString());
            sw.Flush();
        }
    }
}
