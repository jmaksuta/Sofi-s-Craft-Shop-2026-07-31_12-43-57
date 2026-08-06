using UnityEngine;
using System.Collections.Generic;
using SofisCraftShop.Data;

namespace SofisCraftShop.UI
{

    public class InventoryGridUI : MonoBehaviour
    {
        [Header("Grid Configuration")]
        [SerializeField]
        private Transform contentParent;
        [SerializeField]
        private InventorySlotUI slotPrefab;
        [SerializeField]
        private int totalSlotCapacity = 24;

        private List<InventorySlotUI> spawnedSlots = new List<InventorySlotUI>();

        [SerializeField]
        private ItemDatabaseSO itemDatabase;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            spawnedSlots.Clear();

            for (int i = 0; i < totalSlotCapacity; i++)
            {
                InventorySlotUI slotInstance = Instantiate(slotPrefab, contentParent);
                slotInstance.ClearSlot();
                spawnedSlots.Add(slotInstance);
            }
        }

        public void PopulateInventory(List<ItemDataSO> items)
        {
            for (int i = 0; i < spawnedSlots.Count; i++)
            {
                if (i < items.Count && items[i] != null)
                {
                    spawnedSlots[i].SetupSlot(items[i], 1);
                }
                else
                {
                    spawnedSlots[i].ClearSlot();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}