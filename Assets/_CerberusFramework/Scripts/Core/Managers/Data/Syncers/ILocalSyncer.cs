using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.Managers.Data.Storages;

namespace CerberusFramework.Core.Managers.Data.Syncers
{
    public interface ILocalSyncer<T> where T : class, IStorageContainer, new()
    {
        UniTask<T> Load(CancellationToken cancellationToken);

        void Save(T data);

        void UpdateAccountId(string accountId);
    }
}