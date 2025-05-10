using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.Managers.Asset;
using CerberusFramework.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace CerberusFramework.Core.UI.Components
{
    public class ResourceView : MonoBehaviour, IDisposable
    {
        [SerializeField] protected Image Icon;
        [SerializeField] protected CFText Amount;
        [SerializeField] protected string SpriteName = "Infinite";

        private AddressableManager _addressableManager;
        private Sprite _loadedSprite;

        [Inject]
        public void Inject(AddressableManager addressableManager)
        {
            _addressableManager = addressableManager;
        }

        public UniTask Initialize(int value, CancellationToken cancellationToken)
        {
            if (_loadedSprite != null)
            {
                _addressableManager.ReleaseInstance(_loadedSprite);
                _loadedSprite = null;
            }

            SetAmount(value);

            if (Icon == null)
            {
                //if (_loadedSprite == null)
                //{
                //    _loadedSprite = await _addressableManager.LoadAssetAsync(
                //        _addressableManager.TextureHolder.GetResourceTypeAssetReferenceAtlasedSprite(data.Type),
                //        cancellationToken
                //    );
                //}
            }

            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            if (_loadedSprite == null)
            {
                return;
            }

            _addressableManager.ReleaseInstance(_loadedSprite);
            _loadedSprite = null;
        }

        public void SetAmount(int value)
        {
            Amount.Text = value.ToFormattedString();
        }
    }
}