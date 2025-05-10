using System;

namespace CerberusFramework.Utilities
{
    public interface ISerializer
    {
        byte[] SerializeObject(object obj);

        T DeserializeObject<T>(byte[] serializedObj);

        object DeserializeObject(byte[] serializedObj, Type type);
    }
}