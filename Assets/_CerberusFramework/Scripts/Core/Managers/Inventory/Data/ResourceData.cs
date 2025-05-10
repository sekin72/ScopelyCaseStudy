using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Newtonsoft.Json;

namespace CerberusFramework.Core.Managers.Inventory
{
    [Serializable]
    public sealed class ResourceData : ICloneable
    {
        [JsonProperty("t")] public ResourceKeys Type;

        [JsonProperty("v")] public int Value;

        public ResourceData()
        {
        }

        public ResourceData(ResourceKeys type, int value)
        {
            Type = type;
            Value = value;
        }

        public ResourceData(ResourceData dataToCopy)
        {
            Type = dataToCopy.Type;
            Value = dataToCopy.Value;
        }

        public ResourceData(ResourceData dataToCopy, bool negateValue)
        {
            Type = dataToCopy.Type;
            Value = negateValue ? -dataToCopy.Value : dataToCopy.Value;
        }

        public object Clone()
        {
            return new ResourceData(this);
        }
    }

    public static class ResourceDataExtensions
    {
        public static bool HasType(this List<ResourceData> resources, ResourceKeys type)
        {
            foreach (var resource in resources)
            {
                if (resource.Type == type)
                {
                    return true;
                }
            }

            return false;
        }
    }
}