using UnityEngine;
using System.Collections.Generic;
using SofisCraftShop.Core;
using System;
using SofisCraftShop.Data;

namespace SofisCraftShop.UI
{

    public class CraftingQueuePanelUI : MonoBehaviour
    {

        [Header("Container & Prefab")]
        [SerializeField]
        private Transform queueContainerParent;
        [SerializeField]
        private CraftingQueueSlotUI queueSlotPrefab;

        private List<CraftingQueueSlotUI> activeSlotUIs = new List<CraftingQueueSlotUI>();

        private void OnEnable()
        {
            if (CraftingManager.Instance != null)
            {
                CraftingManager.Instance.OnCraftStarted += HandleCraftStarted;
                CraftingManager.Instance.OnCraftCompleted += HandleCraftCompleted;
            }
            RefreshQueueUI();
        }

        private void OnDisable()
        {
            if (CraftingManager.Instance != null)
            {
                CraftingManager.Instance.OnCraftStarted -= HandleCraftStarted;
                CraftingManager.Instance.OnCraftCompleted -= HandleCraftCompleted;
            }
        }

        private void RefreshQueueUI()
        {
            foreach (Transform child in queueContainerParent)
            {
                Destroy(child.gameObject);
            }
            activeSlotUIs.Clear();

            if (CraftingManager.Instance == null) return;

            var currentQueue = CraftingManager.Instance.GetActiveQueue();
            foreach (var task in currentQueue)
            {
                CraftingQueueSlotUI slotInstance = Instantiate(queueSlotPrefab, queueContainerParent);
                // TODO:  slotInstance.BindTask(task);
                activeSlotUIs.Add(slotInstance);
            }
        }

        private void HandleCraftCompleted(Data.RecipeDataSO recipe)
        {
            RefreshQueueUI();
        }

        private void HandleCraftStarted(CraftingQueueTask task)
        {
            RefreshQueueUI();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}