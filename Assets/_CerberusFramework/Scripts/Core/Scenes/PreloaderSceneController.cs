using System.Threading;
using Cysharp.Threading.Tasks;

namespace CerberusFramework.Core.Scenes
{
    public class PreloaderSceneController : LoadingSceneController
    {
        public override UniTask ShowEffect(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public override bool KeepSceneLoaded()
        {
            return false;
        }
    }
}