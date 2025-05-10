using System;

namespace CerberusFramework.Utilities.Extensions
{
    public static class EnumExtensions
    {
        public static T ToEnum<T>(this string value) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T ToEnum<T>(this string value, T defaultValue) where T : Enum
        {
            return value == null ? defaultValue : (T)Enum.Parse(typeof(T), value, true);
        }
    }
}