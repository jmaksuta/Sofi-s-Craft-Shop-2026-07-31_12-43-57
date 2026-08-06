using System;
using System.Collections.Generic;
using UnityEngine;
using SofisCraftShop.Data;

namespace SofisCraftShop.UI
{
    public class CraftingQueueManager : MonoBehaviour
    {
        [Header("Container & Prefab")]
        [SerializeField] private Transform queueContainer;
        [SerializeField] private GameObject queueSlotPrefab;

        [Header("Data References")]
        [SerializeField] private ItemDatabaseSO itemDatabase;

        // Fired when player clicks "Claim" on a completed item
        public event Action<string> OnClaimRequested;

        public void RefreshQueue(List<QueueItemDto> activeQueue)
        {
            // Clear current list
            foreach (Transform child in queueContainer)
            {
                Destroy(child.gameObject);
            }

            if (activeQueue == null || activeQueue.Count == 0) return;

            foreach (var craftItem in activeQueue)
            {
                GameObject slotObj = Instantiate(queueSlotPrefab, queueContainer);
                CraftingQueueSlotUI slotUI = slotObj.GetComponent<CraftingQueueSlotUI>();

                ItemDataSO itemData = itemDatabase != null ? itemDatabase.GetItem(craftItem.recipeId) : null;
                Sprite icon = itemData != null ? itemData.icon : null;
                string displayName = itemData != null ? itemData.name : craftItem.recipeId;

                slotUI.SetupSlot(
                    craftItem.queueItemId,
                    craftItem.recipeId,
                    icon,
                    displayName,
                    craftItem.StartedAtDateTime,
                    craftItem.CompletesAtDateTime,
                    HandleClaimItem
                );
            }
        }

        private void HandleClaimItem(string queueId)
        {
            Debug.Log($"[CraftingQueue] Claiming completed queue item: {queueId}");
            OnClaimRequested?.Invoke(queueId);
        }
    }
}