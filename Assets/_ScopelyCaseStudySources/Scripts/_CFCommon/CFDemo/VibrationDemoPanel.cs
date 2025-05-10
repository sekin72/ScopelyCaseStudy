using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core.Managers.Vibration;
using CerberusFramework.Core.Scenes;
using TMPro;
using UnityEngine;
using VContainer;
using static TMPro.TMP_Dropdown;

namespace CFGameClient.CFDemoScene
{
    public class VibrationDemoPanel : DemoPanel
    {
        [SerializeField] private TMP_Dropdown _vibrationDropdown;
        private VibrationManager _vibrationManager;
        private List<VibrationType> _vibrationTypes = new List<VibrationType>();

        [Inject]
        public void Inject(VibrationManager vibrationManager)
        {
            _vibrationManager = vibrationManager;
        }

        public override void Initialize(SceneController sceneController, CancellationToken cancellationToken)
        {
            base.Initialize(sceneController, cancellationToken);

            _vibrationDropdown.ClearOptions();
            _vibrationTypes.Clear();

            foreach (var enumType in Enum.GetValues(typeof(VibrationType)))
            {
                var key = (VibrationType)enumType;
                if (key == VibrationType.None)
                {
                    continue;
                }

                _vibrationTypes.Add(key);
                _vibrationDropdown.options.Add(new OptionData(key.ToString()));
            }

            _vibrationDropdown.SetValueWithoutNotify(0);
            _vibrationDropdown.RefreshShownValue();
        }

        public override void OnButtonClicked()
        {
            _vibrationManager.Vibrate(_vibrationTypes[_vibrationDropdown.value]);
        }
    }
}
