using SofisCraftShop.Data;
using SofisCraftShop.Network;
using SofisCraftShop.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SofisCraftShop.Core
{

    public class CraftingManager : MonoBehaviour
    {

        public static CraftingManager Instance { get; private set; }
        
        [Header("State Data (Server Synced)")]
        [SerializeField] 
        private PlayerSyncDto currentSyncData = new PlayerSyncDto();

        // UI Events
        public event Action<PlayerSyncDto> OnSyncDataUpdated;
        public event Action<string> OnCraftingFailed;

        [Header("Runtime Player State")]
        [SerializeField]
        private int playerGold = 100;

        private Dictionary<string, int> inventory = new Dictionary<string, int>();

        [Header("Crafting Settings")]
        [SerializeField]
        private int maxQueueSlots = 3;
        private List<CraftingQueueTask> activeTaskQueue = new List<CraftingQueueTask>();

        public event Action OnInventoryUpdated;
        public event Action<CraftingQueueTask> OnCraftStarted;
        public event Action<RecipeDataSO> OnCraftCompleted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private async void Start()
        {
            // Initial sync on startup
            await RefreshServerStateAsync();
        }

        // Update is called once per frame
        void Update()
        {
            //ProcessCraftingQueue(Time.deltaTime);
        }

        //// Example UI Recipe Button Click Handler
        //public void OnCraftButtonClicked(string recipeId)
        //{
        //    CraftingManager.Instance.StartCraft(recipeId);
        //}

        //// Example UI Queue Slot "Claim" Button Click Handler
        //public void OnClaimButtonClicked(string queueItemId)
        //{
        //    CraftingManager.Instance.ClaimCraft(queueItemId);
        //}

        public void AddItem(ItemDataSO item, int amount = 1)
        {
            if (item == null || amount <= 0)
            {
                return;
            }
            if (inventory.ContainsKey(item.itemId))
            {
                inventory[item.itemId] += amount;
            }
            else
            {
                inventory[item.itemId] = amount;
            }
            OnInventoryUpdated.Invoke();
        }

        public bool HasIngredients(RecipeDataSO recipe)
        {
            if (recipe == null) return false;
            if (playerGold < recipe.goldCost) return false;

            foreach (var req in recipe.ingredients)
            {
                if (req.item == null) continue;

                string id = req.item.itemId;
                if (!inventory.TryGetValue(id, out int count) || count < req.quantity)
                {
                    return false;
                }
            }
            return true;
        }

        private void DeductIngredients(RecipeDataSO recipe)
        {
            playerGold -= recipe.goldCost;

            foreach (var req in recipe.ingredients)
            {
                if (req.item == null) continue;

                string id = req.item.itemId;
                if (inventory.ContainsKey(id))
                {
                    inventory[id] -= req.quantity;
                    if (inventory[id] <= 0)
                    {
                        inventory.Remove(id);
                    }
                }
            }
            OnInventoryUpdated.Invoke();
        }

        public bool TryStartCraft(RecipeDataSO recipe)
        {
            if (activeTaskQueue.Count >= maxQueueSlots)
            {
                Debug.LogWarning("Crafting queue is full!");
                return false;
            }

            if (!HasIngredients(recipe))
            {
                Debug.LogWarning("Missing Ingredients or gold!");
                return false;
            }

            DeductIngredients(recipe);

            CraftingQueueTask newTask = new CraftingQueueTask(recipe);
            activeTaskQueue.Add(newTask);

            OnCraftStarted?.Invoke(newTask);
            Debug.Log($"Started crafting: {recipe.name}");
            return true;
        }

        #region API Handlers

        /// <summary>
        /// Fetches full player inventory, gold, and crafting queue from backend.
        /// </summary>
        public async Task RefreshServerStateAsync()
        {
            if (!ApiClient.Instance.IsLoggedIn)
            {
                Debug.LogWarning("[CraftingManager] Cannot sync: Player not logged in.");
                return;
            }

            string json = await ApiClient.Instance.GetSyncDataAsync();
            if (!string.IsNullOrEmpty(json))
            {
                currentSyncData = JsonUtility.FromJson<PlayerSyncDto>(json);
                OnSyncDataUpdated?.Invoke(currentSyncData);
            }
        }

        /// <summary>
        /// Replaces local crafting logic with API call to start crafting.
        /// </summary>
        public async void StartCraft(string recipeId)
        {
            if (string.IsNullOrEmpty(recipeId)) return;

            // 1. Call Backend
            string responseJson = await ApiClient.Instance.RequestStartCraftAsync(recipeId);

            if (!string.IsNullOrEmpty(responseJson))
            {
                // 2. Parse updated queue item or refreshed state returned by server
                Debug.Log($"<color=green>[Crafting] Craft started successfully for {recipeId}</color>");

                // Refresh full state to sync updated gold, materials, and queue
                await RefreshServerStateAsync();
            }
            else
            {
                OnCraftingFailed?.Invoke("Failed to start craft. Check missing materials or queue capacity.");
            }
        }

        /// <summary>
        /// Replaces local array manipulation with API call to claim completed craft.
        /// </summary>
        public async void ClaimCraft(string queueItemId)
        {
            if (string.IsNullOrEmpty(queueItemId)) return;

            // 1. Call Backend
            string responseJson = await ApiClient.Instance.RequestClaimCraftAsync(queueItemId);

            if (!string.IsNullOrEmpty(responseJson))
            {
                // 2. Server processed item delivery and removed item from queue
                Debug.Log($"<color=green>[Crafting] Item claimed successfully! Queue ID: {queueItemId}</color>");

                // Parse updated player sync state returned by claim endpoint
                currentSyncData = JsonUtility.FromJson<PlayerSyncDto>(responseJson);
                OnSyncDataUpdated?.Invoke(currentSyncData);
            }
            else
            {
                OnCraftingFailed?.Invoke("Failed to claim craft. Item might still be in progress.");
            }
        }

        #endregion

        #region Getters for UI Bindings

        public PlayerSyncDto CurrentState => currentSyncData;

        #endregion

        private void ProcessCraftingQueue(float deltaTime)
        {
            if (this.activeTaskQueue.Count == 0)
            {
                return;
            }

            CraftingQueueTask currentTask = activeTaskQueue[0];

            if (!currentTask.isCompleted)
            {
                currentTask.elapsedTime += deltaTime;

                if (currentTask.elapsedTime >= currentTask.duration)
                {
                    currentTask.isCompleted = true;
                    CompleteCraft(currentTask);
                }
            }
        }

        private void CompleteCraft(CraftingQueueTask task)
        {

            AddItem(task.recipe.resultItem, task.recipe.resultAmount);

            activeTaskQueue.Remove(task);
            OnCraftCompleted(task.recipe);

            Debug.Log($"Craft Complete! received: {task.recipe.resultItem.name} x{task.recipe.resultAmount}.");
        }

        public int GetItemCount(string itemId)
        {
            return inventory.TryGetValue(itemId, out int count) ? count : 0;
        }

        public int GetGold() => playerGold;

        public IReadOnlyList<CraftingQueueTask> GetActiveQueue() => activeTaskQueue.AsReadOnly();

    }
}