using System.Text;
using Newtonsoft.Json;

namespace VoxCake.Networking.Common
{
    public class NetworkSerializer
    {
        public static byte[] Serialize(object instance)
        {
            var json = JsonConvert.SerializeObject(instance);
            return Encoding.UTF8.GetBytes(json);
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            var json = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}