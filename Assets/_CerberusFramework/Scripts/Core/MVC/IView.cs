using System;
using Cysharp.Threading.Tasks;

namespace CerberusFramework.Core.MVC
{
    public interface IView
    {
        UniTask Initialize();

        void Activate();

        void Deactivate();

        void Dispose();
    }
}