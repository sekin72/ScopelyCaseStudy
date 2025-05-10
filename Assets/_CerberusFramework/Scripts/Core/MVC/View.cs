using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CerberusFramework.Core.MVC
{
    public abstract class View : MonoBehaviour, ILifecycle
    {
        protected Data Data { get; private set; }

        public abstract UniTask Initialize(CancellationToken cancellationToken);

        public abstract void Activate();

        public abstract void Deactivate();

        public abstract void Dispose();

        public void SetData(Data data)
        {
            Data = data;
        }
    }
}