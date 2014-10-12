using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace WowItemMaker2
{
    public class Util
    {
        public static string GetCpuID()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + HardwareName.Win32_Processor.ToString());
            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj.GetPropertyValue("ProcessorId") != null)
                    return obj.GetPropertyValue("ProcessorId").ToString();
            }
            return null;
        }
    }
}
