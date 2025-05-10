using System;
using System.Runtime.CompilerServices;

namespace CerberusFramework.Utilities.Extensions
{
    public static class ExceptionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetLogMessage(this Exception e)
        {
            return $"{e.Message} {e.StackTrace}";
        }
    }
}