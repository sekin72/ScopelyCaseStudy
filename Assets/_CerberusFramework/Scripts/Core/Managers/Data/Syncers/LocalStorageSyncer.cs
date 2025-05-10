using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.Managers.Data.Storages;
using CerberusFramework.Utilities;
using CerberusFramework.Utilities.Logging;
using UnityEngine;

namespace CerberusFramework.Core.Managers.Data.Syncers
{
    public sealed class LocalStorageSyncer<T> : ILocalSyncer<T> where T : class, IStorageContainer, new()
    {
        private const int MaxSaveFiles = 1;

        private static readonly ICerberusLogger Logger = CerberusLogger.GetLogger(typeof(LocalStorageSyncer<T>));

        private readonly SortedList<ulong, string> _existingFiles = new SortedList<ulong, string>();

        private readonly string _persistentBasePath;
        private readonly JsonSerializer _serializer;

        private readonly string _storageKey;

        private T _cached;

        private string _playerDataFolder;

        public LocalStorageSyncer(string storageKey, string playerId)
        {
            _persistentBasePath = Application.persistentDataPath;

            _storageKey = storageKey;
            _playerDataFolder = playerId + "_Data/";

            _serializer = new JsonSerializer();
        }

        public async UniTask<T> Load(CancellationToken cancellationToken)
        {
            if (_cached != null)
            {
                return _cached;
            }

            await UniTask.SwitchToThreadPool();
            var container = LoadAsType();
            await UniTask.SwitchToMainThread();

            if (container == null)
            {
                _cached = new T();
                return _cached;
            }

            _cached = container;
            return _cached;
        }

        public void Save(T data)
        {
            _cached = data;
            SaveAsType(_cached);
        }

        public void UpdateAccountId(string accountId)
        {
            _playerDataFolder = accountId + "_Data/";
        }

        private T LoadAsType()
        {
            var dataPath = GetFilePath();

            if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            {
                Logger.Debug($"Save directory is not found: {dataPath}");
                return null;
            }

            var jsonFiles = Directory.GetFiles(dataPath, $"{_storageKey}_*.json");
            _existingFiles.Clear();

            foreach (var filePath in jsonFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var fileNameParts = fileName.Split('_');

                if (fileNameParts.Length >= 2 && ulong.TryParse(fileNameParts[1], out var saveIndex))
                {
                    _existingFiles.Add(saveIndex, filePath);
                }
            }

            T data = null;
            ulong readSaveIndex = 0;

            for (var i = _existingFiles.Count - 1; i >= 0; i--)
            {
                var filePath = _existingFiles.Values[i];
                var saveIndex = _existingFiles.Keys[i];

                if (!TryReadData(filePath, out data))
                {
                    continue;
                }

                readSaveIndex = saveIndex;
                break;
            }

            if (data == null)
            {
                return null;
            }

            for (var i = _existingFiles.Count - 1; i >= 0; i--)
            {
                if (_existingFiles.Keys[i] <= readSaveIndex)
                {
                    continue;
                }

                var saveIndex = _existingFiles.Keys[i];
                var filePath = _existingFiles.Values[i];

                Logger.Debug($"Deleting corrupted file: {filePath}");

                File.Delete(filePath);
                _existingFiles.Remove(saveIndex);
            }

            return data;
        }

        private bool TryReadData(string dataPath, out T data)
        {
            try
            {
                var fileText = File.ReadAllText(dataPath);
                var jsonDataAsBytes = Encoding.UTF8.GetBytes(fileText);

                if (!Application.isEditor)
                {
                    jsonDataAsBytes = Convert.FromBase64String(fileText);
                }

                Logger.Debug(
                    $"<color=green>Loaded all data from: </color> {dataPath}\n{Encoding.UTF8.GetString(jsonDataAsBytes)}"
                );

                var returnedData = _serializer.DeserializeObject<T>(jsonDataAsBytes);
                data = (T)Convert.ChangeType(returnedData, typeof(T));
                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to read data from: {dataPath} , Error: {e.Message}");
                data = null;
                return false;
            }
        }

        private void SaveAsType(T data)
        {
            data.DeviceId = SystemInfo.deviceUniqueIdentifier;
            data.Timestamp = DateTime.UtcNow.ToFileTimeUtc();

            var saveIndex = _existingFiles.Count > 0 ? _existingFiles.Keys[_existingFiles.Count - 1] : 0;
            saveIndex++;

            var dataPath = GetFilePath($"{_storageKey}_{saveIndex.ToString()}");

            var byteData = _serializer.SerializeObject(data);

            if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath) ?? string.Empty);
            }

            try
            {
                var fileText = Encoding.UTF8.GetString(byteData);

                if (!Application.isEditor)
                {
                    fileText = Convert.ToBase64String(byteData);
                }

                File.WriteAllText(dataPath, fileText);
                _existingFiles.Add(saveIndex, dataPath);
                Logger.Debug($"Save data to: {dataPath}");

                while (_existingFiles.Count > MaxSaveFiles)
                {
                    var fileToDelete = _existingFiles[_existingFiles.Keys[0]];
                    File.Delete(fileToDelete);
                    _existingFiles.RemoveAt(0);
                    Logger.Debug($"Deleted Old File: {fileToDelete}");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to save data to: {dataPath} , Error: {e.Message}");
            }
        }

        private string GetFilePath(string fileName = "")
        {
            var filePath = Path.Combine(_persistentBasePath, _playerDataFolder);

            if (fileName != "")
            {
                filePath = Path.Combine(filePath, $"{fileName}.json");
            }

            return filePath;
        }
    }
}