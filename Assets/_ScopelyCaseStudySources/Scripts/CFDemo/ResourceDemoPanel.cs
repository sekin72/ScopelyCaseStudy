using System.Threading;
using CerberusFramework.Core.Managers.Inventory;
using CerberusFramework.Core.Managers.UI;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Core.UI.Components;
using CerberusFramework.Core.UI.FlyTweens.Coin;
using CerberusFramework.Core.UI.FlyTweens.Star;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;

namespace CFGameClient.CFDemoScene
{
    public class ResourceDemoPanel : DemoPanel
    {
        [SerializeField] private ResourceKeys _resourceType;
        [SerializeField] private ResourceView _resourceView;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private int _defaultValue = 100;

        private InventoryManager _inventoryManager;
        private FlyTweenManager _flyTweenManager;

        private ResourceData _resourceData;

        [Inject]
        public void Inject(InventoryManager inventoryManager, FlyTweenManager flyTweenManager)
        {
            _inventoryManager = inventoryManager;
            _flyTweenManager = flyTweenManager;
        }

        public override void Initialize(SceneController sceneController, CancellationToken cancellationToken)
        {
            base.Initialize(sceneController, cancellationToken);

            _resourceData = new ResourceData(_resourceType, _defaultValue);
            _resourceView.Initialize(_inventoryManager.GetValue(_resourceType), cancellationToken);

            _inputField.characterValidation = TMP_InputField.CharacterValidation.Integer;
            _inputField.text = _defaultValue.ToString();
        }

        public override void OnButtonClicked()
        {
            int.TryParse(_inputField.text, out _resourceData.Value);
            var token = _inventoryManager.Commit(_resourceData, "Demo");

            switch (_resourceType)
            {
                case ResourceKeys.Coin:
                    _flyTweenManager.FlyTween<CoinFlyTween, CoinFlyTweenData, CoinFlyTweenView>(
                            new CoinFlyTweenData(_resourceData, Button.transform, _resourceView.transform, SceneController.UIContainer, () =>
                            {
                                _inventoryManager.Submit(token);
                                _resourceView.SetAmount(_inventoryManager.GetValue(_resourceType));
                            }),
                            CancellationToken)
                        .Forget();
                    break;
                case ResourceKeys.Star:
                    _flyTweenManager.FlyTween<StarFlyTween, StarFlyTweenData, StarFlyTweenView>(
                            new StarFlyTweenData(_resourceData, Button.transform, _resourceView.transform, SceneController.UIContainer, () =>
                            {
                                _inventoryManager.Submit(token);
                                _resourceView.SetAmount(_inventoryManager.GetValue(_resourceType));
                            }),
                            CancellationToken)
                        .Forget();
                    break;
            }
        }
    }
}
