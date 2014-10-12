using System;
using System.Collections.Generic;
using System.Text;

namespace LibDB2
{
    public class Convert
    {
        public static byte[] getBytes(int i)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)i;
            bytes[1] = (byte)(i >> 8);
            bytes[2] = (byte)(i >> 16);
            bytes[3] = (byte)(i >> 24);
            return bytes;
        }

        public static byte[] getBytes(uint i)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)i;
            bytes[1] = (byte)(i >> 8);
            bytes[2] = (byte)(i >> 16);
            bytes[3] = (byte)(i >> 24);
            return bytes;
        }

        public static byte[] getBytes(long l)
        {
            byte[] bytes = new byte[8];
            bytes[0] = (byte)l;
            bytes[1] = (byte)(l >> 8);
            bytes[2] = (byte)(l >> 16);
            bytes[3] = (byte)(l >> 24);
            bytes[4] = (byte)(l >> 32);
            bytes[5] = (byte)(l >> 40);
            bytes[6] = (byte)(l >> 48); 
            bytes[7] = (byte)(l >> 56);
            return bytes;
        }

        public static byte[] getBytes(ulong l)
        {
            byte[] bytes = new byte[8];
            bytes[0] = (byte)l;
            bytes[1] = (byte)(l >> 8);
            bytes[2] = (byte)(l >> 16);
            bytes[3] = (byte)(l >> 24);
            bytes[4] = (byte)(l >> 32);
            bytes[5] = (byte)(l >> 40);
            bytes[6] = (byte)(l >> 48);
            bytes[7] = (byte)(l >> 56);
            return bytes;
        }

        public static byte[] getBytes(short s)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)s;
            bytes[1] = (byte)(s >> 8);
            return bytes;
        }

        public static byte[] getBytes(ushort s)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)s;
            bytes[1] = (byte)(s >> 8);
            return bytes;
        }

        public static byte[] getBytes(float f)
        {
            return BitConverter.GetBytes(f);
        }

        public static byte[] getBytes(double d)
        {
            return BitConverter.GetBytes(d);
        }

        public static byte[] getBytes(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }

        public static byte[] getBytes(object obj, Type type)
        {
            byte[] bytes = null;
            if (obj == null && type != typeof(string))
            {
                obj = 0;
            }
            else if (obj == null && type == typeof(string))
            {
                obj = string.Empty;
            }

            else if (obj.ToString() == string.Empty && type != typeof(string))
            {
                obj = 0;
            }
            switch (type.Name.ToLower())
            {
                case "long":
                case "int64":
                    bytes = getBytes((long)obj);
                    break;
                case "ulong":
                    bytes = getBytes((ulong)obj);
                    break;
                case "int32":
                    bytes = getBytes((int)obj);
                    break;
                case "uint32":
                    bytes = getBytes((uint)obj);
                    break;
                case "short":
                    bytes = getBytes((short)obj);
                    break;
                case "ushort":
                    bytes = getBytes((ushort)obj);
                    break;
                case "sbyte":
                case "byte":
                    bytes = new byte[]{ (byte)obj };
                    break;
                case "single":
                    bytes = getBytes((float)obj);
                    break;
                case "double":
                    bytes = getBytes((double)obj);
                    break;
                case "string":
                    bytes = getBytes(obj.ToString());
                    break;
                default:
                    throw new NotSupportedException("the type of " + type.Name + " is not supported.");
            }
            return bytes;
        }

    }
}
