using System.Collections.Generic;
using CerberusFramework.Utilities.Transaction;
using UnityEngine.Pool;

namespace CerberusFramework.Core.Managers.Inventory
{
    public class InventoryTransactionToken : TransactionToken<InventoryTransactionData>
    {
        public InventoryTransactionToken(long uniqueId, InventoryTransactionData value) : base(value, uniqueId)
        {
        }

        public InventoryTransactionToken(TransactionToken<InventoryTransactionData> token) : base(token.Data, token.Id)
        {
        }
    }

    public static class InventoryTransactionTokenExtensions
    {
        public static List<ResourceData> GetCombinedResourceData(this List<InventoryTransactionToken> tokens)
        {
            var resources = new List<ResourceData>();
            var cumulativeRewards = DictionaryPool<ResourceKeys, int>.Get();

            foreach (var token in tokens)
            {
                cumulativeRewards[token.Data.ResourceData.Type] = 0;

                cumulativeRewards[token.Data.ResourceData.Type] += token.Data.ResourceData.Value;
            }

            foreach (var reward in cumulativeRewards)
            {
                resources.Add(new ResourceData(reward.Key, reward.Value));
            }

            DictionaryPool<ResourceKeys, int>.Release(cumulativeRewards);

            return resources;
        }

        public static void AddCombinedResourceData(this List<InventoryTransactionToken> tokens,
            List<ResourceData> resources)
        {
            var cumulativeRewards = DictionaryPool<ResourceKeys, int>.Get();
            foreach (var resource in resources)
            {
                cumulativeRewards[resource.Type] = 0;

                cumulativeRewards[resource.Type] += resource.Value;
            }

            resources.Clear();

            foreach (var token in tokens)
            {
                cumulativeRewards[token.Data.ResourceData.Type] = 0;

                cumulativeRewards[token.Data.ResourceData.Type] += token.Data.ResourceData.Value;
            }

            foreach (var reward in cumulativeRewards)
            {
                resources.Add(new ResourceData(reward.Key, reward.Value));
            }

            DictionaryPool<ResourceKeys, int>.Release(cumulativeRewards);
        }
    }
}