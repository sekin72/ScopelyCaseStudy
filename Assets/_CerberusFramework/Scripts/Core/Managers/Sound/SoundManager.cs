using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using CerberusFramework.Config;
using CerberusFramework.Core.Managers.Data;
using CerberusFramework.Core.Managers.Data.Storages;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.Managers.Vibration;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CerberusFramework.Core.Managers.Sound
{
    public sealed class SoundManager : Manager
    {
        private const int MaxSoundPlaySimultaneously = 3;

        private readonly Dictionary<SoundKeys, AudioClip> _audioClips = new Dictionary<SoundKeys, AudioClip>();
        private readonly List<AudioSource> _sourceList = new List<AudioSource>();
        private AssetManager _assetManager;
        private DataManager _dataManager;
        private GameObject _loadedAsset;

        private PoolManager _poolManager;

        private bool _sequenceStarts;

        private SettingsStorage _settingsStorage;
        private VibrationManager _vibrationManager;
        private AudioSource _currentSource;
        private UnityEngine.AudioSource[] _sources;

        public override bool IsCore => true;

        [Inject]
        private void Inject(DataManager dataManager, VibrationManager vibrationManager, AssetManager assetManager, PoolManager poolManager)
        {
            _dataManager = dataManager;
            _vibrationManager = vibrationManager;
            _assetManager = assetManager;
            _poolManager = poolManager;
        }

        protected override List<IManager> GetDependencies()
        {
            return new List<IManager>
            {
                _dataManager,
                _assetManager,
                _poolManager
            };
        }

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            LoadData();

            var task1 = _assetManager.GetScriptableAsset<SoundsList>(SOKeys.SoundsList, disposeToken).ContinueWith(
                list =>
                {
                    foreach (var enumType in Enum.GetValues(typeof(SoundKeys)))
                    {
                        var key = (SoundKeys)enumType;
                        if (key == SoundKeys.None)
                        {
                            continue;
                        }

                        _audioClips.Add(key, list.AudioClips.FirstOrDefault(x=> x.name == key.ToString()));
                    }
                });

            _loadedAsset = _poolManager.GetGameObject(PoolKeys.SoundController);

            Object.DontDestroyOnLoad(_loadedAsset);

            var toBeDeleted = _loadedAsset.GetComponents<UnityEngine.AudioSource>();
            foreach (var component in toBeDeleted)
            {
                Object.DestroyImmediate(component, true);
            }

            for (var i = 0; i < MaxSoundPlaySimultaneously; i++)
            {
                _loadedAsset.AddComponent<UnityEngine.AudioSource>().playOnAwake = true;
            }

            _sources = _loadedAsset.GetComponents<UnityEngine.AudioSource>();

            for (var i = 0; i < MaxSoundPlaySimultaneously; i++)
            {
                _sourceList.Add(new AudioSource(_sources[i], false));
                InitAudioSource(_sources[i], i);
            }

            _loadedAsset.SetActive(false);
            _loadedAsset.SetActive(true);

            _currentSource = _sourceList[0];

            SetSoundVolume(_settingsStorage.MasterVolumeValue);
            SetSoundActive(_settingsStorage.IsSoundActive);

            await task1;

            SetReady();
        }

        public override void Dispose()
        {
            if (_loadedAsset == null)
            {
                return;
            }

            _poolManager.SafeReleaseObject(PoolKeys.SoundController, _loadedAsset);
            _loadedAsset = null;
        }

        public void SetSoundActive(bool isActive)
        {
            _vibrationManager.Vibrate(VibrationType.Selection);

            foreach (var source in _sources)
            {
                source.volume = isActive ? _settingsStorage.MasterVolumeValue : 0f;
            }

            _settingsStorage.IsSoundActive = isActive;
            Save();
        }

        public void SetSoundVolume(float value)
        {
            _settingsStorage.MasterVolumeValue = value;

            for (var i = 0; IsSoundActive() && i < _sources.Length; i++)
            {
                _sources[i].volume = _settingsStorage.MasterVolumeValue;
            }

            Save();
        }

        public bool IsSoundActive()
        {
            return _settingsStorage.IsSoundActive;
        }

        public float GetVolume()
        {
            return _settingsStorage.MasterVolumeValue;
        }

        public async UniTask PlayForAWhile(SoundKeys soundType, float duration, float frequency,
            bool playSimultaneously = true, bool randomPitch = false, Func<bool> interruptCondition = null)
        {
            await PlayForAWhileRoutine(soundType, duration, frequency, playSimultaneously, randomPitch,
                interruptCondition);
        }

        public void ActivateSound(bool value)
        {
            if (!value)
            {
                foreach (var source in _sourceList)
                {
                    source.Source.Stop();
                    source.IsPlayingSequence = false;
                }
            }
        }

        private static void InitAudioSource(UnityEngine.AudioSource source, int priority)
        {
            source.playOnAwake = false;
            source.priority = priority;
        }

        private AudioSource PickMostAvailableSource(SoundKeys type, float pitch = 1f, bool stopAll = false,
            bool playInLoop = false, float volumeMultiplier = 1f)
        {
            AudioSource tempSource = null;

            foreach (var source in _sourceList)
            {
                if (stopAll)
                {
                    source.Source.Stop();
                    source.IsPlayingSequence = false;
                }

                if (tempSource == null && !source.IsPlayingSequence && !source.Source.isPlaying)
                {
                    tempSource = source;
                }
            }

            if (tempSource == null)
            {
                foreach (var source in _sourceList)
                {
                    if (source.CurrentSound == type)
                    {
                        tempSource = source;
                        break;
                    }
                }
            }

            if (tempSource == null)
            {
                foreach (var source in _sourceList)
                {
                    if (!source.IsPlayingSequence)
                    {
                        tempSource = source;
                        break;
                    }
                }
            }

            tempSource ??= _sourceList[0];

            tempSource.Reset();
            tempSource.Source.volume = _settingsStorage.MasterVolumeValue * volumeMultiplier;
            tempSource.Source.clip = _audioClips[type];
            tempSource.Source.pitch = pitch;
            tempSource.Source.loop = playInLoop;
            tempSource.CurrentSound = type;

            return tempSource;
        }

        public void PlayOneShot(SoundKeys soundType, float pitchValue = 1f, bool playSimultaneously = true,
            bool playInLoop = false, float volumeMultiplier = 1f)
        {
            PickMostAvailableSource(soundType, pitchValue, !playSimultaneously, playInLoop, volumeMultiplier)
                .Play();
        }

        public void StopAll(bool fadeOut = false, float fadeDuration = 0.25f)
        {
            foreach (var source in _sourceList)
            {
                source.Stop(fadeOut, fadeDuration);
            }
        }

        public void UpdateContinuousSoundPitchAndVolume(SoundKeys soundTypes, float pitch,
            float volumeMultiplier = 1f)
        {
            foreach (var source in _sourceList)
            {
                if (source.CurrentSound == soundTypes)
                {
                    source.Source.pitch = pitch;
                    source.Source.volume *= volumeMultiplier;
                }
            }
        }

        public void PlayWithFadeIn(SoundKeys soundType, float pitchValue, bool playSimultaneously = true,
            bool playInLoop = false, float fadeInDuration = 0.25f, float fadeToValue = 1f)
        {
            PickMostAvailableSource(soundType, pitchValue, !playSimultaneously, playInLoop, 0f)
                .Play(true, fadeInDuration, fadeToValue);
        }

        public void PlayOneShotWithRandomPitch(SoundKeys soundType, float minPitch, float maxPitch,
            bool playSimultaneously = true, bool playInLoop = false, float volumeMultiplier = 1f)
        {
            PickMostAvailableSource(soundType, Random.Range(minPitch, maxPitch), !playSimultaneously, playInLoop,
                    volumeMultiplier)
                .Play();
        }

        public void StopSound(SoundKeys soundType, bool fadeOut, float fadeDuration)
        {
            foreach (var source in _sourceList)
            {
                if (source.CurrentSound == soundType)
                {
                    source.Stop(fadeOut, fadeDuration);
                }
            }
        }

        public async UniTask PlayForAWhileRoutine(SoundKeys soundType, float duration, float frequency,
            bool playSimultaneously, bool randomPitch = false, Func<bool> interruptCondition = null)
        {
            if (_sequenceStarts)
            {
                return;
            }

            var source = PickMostAvailableSource(soundType, 1f, !playSimultaneously);

            source.IsPlayingSequence = true;
            _sequenceStarts = true;
            var time = 0f;

            do
            {
                if (!source.IsPlayingSequence || (interruptCondition != null && interruptCondition()))
                {
                    source.Stop();
                    break;
                }

                source.Play(randomPitch: randomPitch);
                time += frequency;
                await UniTask.Delay(TimeSpan.FromSeconds(frequency));
            } while (time <= duration);

            _currentSource.IsPlayingSequence = false;
            _sequenceStarts = false;
        }

        #region Data

        protected override void LoadData()
        {
            _settingsStorage = _dataManager.Load<SettingsStorage>();
            if (_settingsStorage == null)
            {
                _settingsStorage = new SettingsStorage();
                Save();
            }
        }

        protected override void SaveData()
        {
            _dataManager.Save(_settingsStorage);
        }

        #endregion Data
    }

    public class AudioSource
    {
        private object _tweenId;
        public SoundKeys CurrentSound;
        public bool IsPlayingSequence;

        public UnityEngine.AudioSource Source;

        public AudioSource(UnityEngine.AudioSource source, bool isPlayingSequence)
        {
            Source = source;
            IsPlayingSequence = isPlayingSequence;
        }

        public void Play(bool fadeIn = false, float fadeDuration = 0.25f, float toFadeValue = 0.75f,
            bool randomPitch = false)
        {
            DOTween.Kill(_tweenId);

            if (fadeIn)
            {
                Source.volume = 0f;

                if (randomPitch)
                {
                    Source.pitch = Random.Range(0.8f, 1.2f);
                }

                Source.Play();
                _tweenId = DOVirtual.Float(0, toFadeValue, fadeDuration, value => Source.volume = value).id;
            }
            else
            {
                if (randomPitch)
                {
                    Source.pitch = Random.Range(0.8f, 1.2f);
                }

                Source.Play();
            }
        }

        public void PlayOneShot(AudioClip clip, bool fadeIn = false, float fadeDuration = 0.25f,
            float toFadeValue = 0.75f)
        {
            DOTween.Kill(_tweenId);

            if (fadeIn)
            {
                Source.volume = 0f;
                Source.PlayOneShot(clip);
                _tweenId = DOVirtual.Float(0, toFadeValue, fadeDuration, value => Source.volume = value).id;
            }
            else
            {
                Source.PlayOneShot(clip);
            }
        }

        public void Reset()
        {
            Source.loop = false;
            Source.clip = null;
            Source.pitch = 1f;
            Source.Stop();
        }

        public void Stop(bool fadeout = false, float fadeDuration = 0.25f)
        {
            if (fadeout)
            {
                _tweenId = DOVirtual.Float(0.6f, 0f, fadeDuration, value => Source.volume = value)
                    .OnComplete(Reset).id;
            }
            else
            {
                Reset();
            }
        }
    }
}