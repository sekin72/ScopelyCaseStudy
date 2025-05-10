using System.Runtime.CompilerServices;

namespace CerberusFramework.Utilities.Logging
{
    public interface ICerberusLogger
    {
        void Debug(string message, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0);

        void Warn(string message, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0);

        void Error(string message, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0);
    }
}