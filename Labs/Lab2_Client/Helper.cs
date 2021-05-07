using System.Collections.Generic;
using System.Text;

namespace Lab2_Client
{
    class Helper
    {
        public static byte[] ExtendData(byte[] data)
        {
            int diff = 8 - (data.Length % 8);
            byte[] res = new byte[data.Length + diff];
            data.CopyTo(res, 0);
            return res;
        }
        public static byte[] RecoverData(List<byte> data)
        {
            int i = data.Count - 1;
            while (data[i] == 0)
            {
                data.RemoveRange(i, data.Count - i);
                i--;
            }
            return data.ToArray();
        }
        public static byte[] ExtendKey(string data)
        {
            StringBuilder stringBuilder = new StringBuilder(data);
            if (data.Length > 7)
            {
                return ToByteArray(stringBuilder.Remove(7, data.Length - 7).ToString());
            }
            int diff = 7 - data.Length;
            for (int i = 0; i < diff; i++)
            {
                stringBuilder.Append(data[i % data.Length]);
            }
            return ToByteArray(stringBuilder.ToString());
        }
        public static string ToString(byte[] source)
        {
            return Encoding.UTF8.GetString(source);
        }
        public static byte[] ToByteArray(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
