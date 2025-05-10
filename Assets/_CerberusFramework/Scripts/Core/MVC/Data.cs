namespace CerberusFramework.Core.MVC
{
    public abstract class Data
    {
        public bool IsInitialized { get; set; }
        public bool IsActivated { get; set; }
        public bool IsDeactivated { get; set; }
        public bool IsDisposed { get; set; }
    }
}