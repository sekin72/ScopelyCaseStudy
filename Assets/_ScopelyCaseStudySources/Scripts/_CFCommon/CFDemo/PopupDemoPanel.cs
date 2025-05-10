using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core.Managers.UI;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Core.UI.Popups;
using CerberusFramework.Core.UI.Popups.CheckConnection;
using CerberusFramework.Core.UI.Popups.Loading;
using CerberusFramework.Core.UI.Popups.RemoteAssetDownload;
using CerberusFramework.Core.UI.Popups.Settings;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.UI.Popups.Fail;
using ScopelyCaseStudy.Core.Gameplay.UI.Popups.Pause;
using ScopelyCaseStudy.Core.Gameplay.UI.Popups.Win;
using TMPro;
using UnityEngine;
using VContainer;
using static TMPro.TMP_Dropdown;

namespace CFGameClient.CFDemoScene
{
    public class PopupDemoPanel : DemoPanel
    {
        [SerializeField] protected TMP_Dropdown PopupDropdown;
        protected List<PopupKeys> PopupTypes = new List<PopupKeys>();
        protected PopupManager PopupManager;

        [Inject]
        public void Inject(PopupManager popupManager)
        {
            PopupManager = popupManager;
        }

        public override void Initialize(SceneController sceneController, CancellationToken cancellationToken)
        {
            base.Initialize(sceneController, cancellationToken);

            PopupDropdown.ClearOptions();
            PopupTypes.Clear();

            foreach (var enumType in Enum.GetValues(typeof(PopupKeys)))
            {
                var key = (PopupKeys)enumType;
                if (key == PopupKeys.None)
                {
                    continue;
                }

                PopupTypes.Add(key);
                PopupDropdown.options.Add(new OptionData(key.ToString()));
            }

            PopupDropdown.SetValueWithoutNotify(0);
            PopupDropdown.RefreshShownValue();
        }

        public override void OnButtonClicked()
        {
            var popupType = PopupTypes[PopupDropdown.value];

            switch (popupType)
            {
                case PopupKeys.CheckYourConnectionPopup:
                    PopupManager.Open<CheckConnectionPopup, CheckConnectionPopupData, CheckConnectionPopupView>(
                        new CheckConnectionPopupData(true, null, null), PopupShowActions.CloseAll, CancellationToken).Forget();
                    break;
                case PopupKeys.RemoteAssetDownloadPopup:
                    PopupManager.Open<RemoteAssetDownloadPopup, RemoteAssetDownloadPopupData, RemoteAssetDownloadPopupView>(new RemoteAssetDownloadPopupData(null), PopupShowActions.CloseAll,
                        CancellationToken).Forget();
                    break;
                case PopupKeys.LoadingPopup:
                    PopupManager.Open<LoadingPopup, LoadingPopupData, LoadingPopupView>(new LoadingPopupData(), PopupShowActions.CloseAll, CancellationToken).Forget();
                    break;
                case PopupKeys.FailPopup:
                    PopupManager.Open<FailPopup, FailPopupData, FailPopupView>(new FailPopupData(null), PopupShowActions.CloseAll, CancellationToken).Forget();
                    break;
                case PopupKeys.WinPopup:
                    PopupManager.Open<WinPopup, WinPopupData, WinPopupView>(new WinPopupData(null), PopupShowActions.CloseAll, CancellationToken).Forget();
                    break;
                case PopupKeys.SettingsPopup:
                    PopupManager.Open<SettingsPopup, SettingsPopupData, SettingsPopupView>(new SettingsPopupData(), PopupShowActions.CloseAll, CancellationToken).Forget();
                    break;
                case PopupKeys.PausePopup:
                    PopupManager.Open<PausePopup, PausePopupData, PausePopupView>(new PausePopupData(null, null, null, null), PopupShowActions.CloseAll, CancellationToken).Forget();
                    break;
                case PopupKeys.None:
                default:
                    return;
            }
        }
    }
}
