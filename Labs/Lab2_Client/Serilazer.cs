using System.Text;
using System.Text.Json;

namespace Lab2_Client
{
    class Serilazer<T>
    {
        public static byte[] Serilize(T source)
        {
            var json = JsonSerializer.Serialize<T>(source);
            return Encoding.UTF8.GetBytes(json);
        }
        public static T Deserilaze(byte[] source)
        {
            var json = Encoding.UTF8.GetString(source);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
