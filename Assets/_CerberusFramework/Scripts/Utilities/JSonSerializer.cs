using System;
using System.Text;
using Newtonsoft.Json;

namespace CerberusFramework.Utilities
{
    public class JsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonSerializer() : this(JsonConvert.DefaultSettings != null ? JsonConvert.DefaultSettings() : new JsonSerializerSettings())
        {
        }

        public JsonSerializer(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public T DeserializeObject<T>(byte[] serializedObj)
        {
            return (T)DeserializeObject(serializedObj, typeof(T));
        }

        public object DeserializeObject(byte[] serializedObj, Type type)
        {
            var json = Encoding.UTF8.GetString(serializedObj);
            return JsonConvert.DeserializeObject(json, type, _settings);
        }

        public byte[] SerializeObject(object obj)
        {
            var json = JsonConvert.SerializeObject(obj, _settings);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}