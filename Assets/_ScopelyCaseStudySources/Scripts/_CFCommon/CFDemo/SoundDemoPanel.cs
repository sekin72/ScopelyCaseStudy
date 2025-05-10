using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core.Managers.Sound;
using CerberusFramework.Core.Scenes;
using TMPro;
using UnityEngine;
using VContainer;
using static TMPro.TMP_Dropdown;

namespace CFGameClient.CFDemoScene
{
    public class SoundDemoPanel : DemoPanel
    {
        [SerializeField] private TMP_Dropdown _soundDropdown;
        private SoundManager _soundManager;
        private List<SoundKeys> _soundTypes = new List<SoundKeys>();

        [Inject]
        public void Inject(SoundManager soundManager)
        {
            _soundManager = soundManager;
        }

        public override void Initialize(SceneController sceneController, CancellationToken cancellationToken)
        {
            base.Initialize(sceneController, cancellationToken);

            _soundDropdown.ClearOptions();
            _soundTypes.Clear();
            foreach (var enumType in Enum.GetValues(typeof(SoundKeys)))
            {
                var key = (SoundKeys)enumType;
                if (key == SoundKeys.None)
                {
                    continue;
                }

                _soundTypes.Add(key);
                _soundDropdown.options.Add(new OptionData(key.ToString()));
            }

            _soundDropdown.SetValueWithoutNotify(0);
            _soundDropdown.RefreshShownValue();
        }

        public override void OnButtonClicked()
        {
            _soundManager.PlayOneShot(_soundTypes[_soundDropdown.value]);
        }
    }
}
