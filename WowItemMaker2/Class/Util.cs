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
        /// <summary>
        /// AES加密字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encrypt(string data)
        {
            byte[] bytes = AESHelper.AESEncrypt(Encoding.UTF8.GetBytes(data), GetCpuID(), string.Empty);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// AES解密字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Decrypt(string data)
        {
            byte[] bytes = Convert.FromBase64String(data);
            byte[] resultArr = AESHelper.AESDecrypt(bytes, GetCpuID(), string.Empty);
            return Encoding.UTF8.GetString(resultArr);
        }
    }
}
