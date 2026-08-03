using SofisCraftShop.Data;
using SofisCraftShop.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SofisCraftShop.Core
{

    public class CraftingManager : MonoBehaviour
    {

        public static CraftingManager Instance { get; private set; }

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
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            ProcessCraftingQueue(Time.deltaTime);
        }

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
                if (!inventory.TryGetValue(id, out int count) || count < req.amount)
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
                    inventory[id] -= req.amount;
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

        // TODO: hook into api
        //public async Task RefreshServerStateAsync()
        //{
        //    if (!ApiClient.Instance.IsLoggedIn)
        //    {
        //        Debug.LogWarning("[CraftingManager] Cannot sync: Player not logged in.");
        //        return;
        //    }

        //    string json = await ApiClient.Instance.GetSyncDataAsync();
        //    if (!string.IsNullOrEmpty(json))
        //    {
        //        currentSyncData = JsonUtility.FromJson<PlayerSyncDto>(json);
        //        OnSyncDataUpdated?.Invoke(currentSyncData);
        //    }
        //}

        // TODO: hook into api
        //private void StartCraft(string recipeId)
        //{
        //    if (string.IsNullOrEmpty(recipeId)) return;

        //    // 1. Call Backend
        //    string responseJson = await ApiClient.Instance.RequestStartCraftAsync(recipeId);

        //    if (!string.IsNullOrEmpty(responseJson))
        //    {
        //        // 2. Parse updated queue item or refreshed state returned by server
        //        Debug.Log($"<color=green>[Crafting] Craft started successfully for {recipeId}</color>");

        //        // Refresh full state to sync updated gold, materials, and queue
        //        await RefreshServerStateAsync();
        //    }
        //    else
        //    {
        //        OnCraftingFailed?.Invoke("Failed to start craft. Check missing materials or queue capacity.");
        //    }
        //}

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