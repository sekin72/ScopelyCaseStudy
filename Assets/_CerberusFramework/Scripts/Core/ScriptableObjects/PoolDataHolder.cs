using System.Collections.Generic;
using CerberusFramework.Core.Managers.Data;
using UnityEngine;
using VInspector;

namespace CerberusFramework.Core.Managers.Pool
{
    [CreateAssetMenu(fileName = "PoolDataHolder", menuName = "CerberusFramework/PoolDataHolder", order = 1)]
    public class PoolDataHolder : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<PoolKeys, PoolDefinition> _poolDataDict = new SerializedDictionary<PoolKeys, PoolDefinition>();

        public PoolDefinition GetPoolDefinition(PoolKeys key)
        {
            if(_poolDataDict.TryGetValue(key, out var poolDefinition))
            {
                return poolDefinition;
            }
            else
            {
                Debug.LogError($"Pool definition for key {key} not found.");
                return null;
            }
        }
    }
}