using System.Collections.Generic;
using UnityEngine;

namespace SofisCraftShop.Data
{

    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Sofi's Craft Shop/Data/Item Database")]
    public class ItemDatabaseSO : ScriptableObject
    {
        [SerializeField] private List<ItemDataSO> items = new List<ItemDataSO>();

        private Dictionary<string, ItemDataSO> itemLookup;

        /// <summary>
        /// Builds the internal dictionary mapping server item IDs to ItemDataSO assets.
        /// </summary>
        public void Initialize()
        {
            itemLookup = new Dictionary<string, ItemDataSO>();

            foreach (var item in items)
            {
                if (item == null) continue;

                if (string.IsNullOrEmpty(item.itemId))
                {
                    Debug.LogWarning($"[ItemDatabase] Item '{item.name}' has no itemId assigned!");
                    continue;
                }

                if (!itemLookup.ContainsKey(item.itemId))
                {
                    itemLookup.Add(item.itemId, item);
                }
                else
                {
                    Debug.LogError($"[ItemDatabase] Duplicate itemId found: '{item.itemId}'.");
                }
            }
        }

        public ItemDataSO GetItem(string itemId)
        {
            if (itemLookup == null)
            {
                Initialize();
            }

            if (itemLookup.TryGetValue(itemId, out ItemDataSO item))
            {
                return item;
            }

            Debug.LogWarning($"[ItemDatabase] Item with ID '{itemId}' not found in database.");
            return null;
        }

        public Sprite GetIcon(string itemId)
        {
            var item = GetItem(itemId);
            return item != null ? item.icon : null;
        }

        public Color GetRarityColor(string itemId)
        {
            var item = GetItem(itemId);
            return item != null ? item.GetRarityColor() : Color.white;
        }
    }

}