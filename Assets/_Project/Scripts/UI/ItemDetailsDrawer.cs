using SofisCraftShop.Data;
using SofisCraftShop.Network;
//using SofisCraftShop.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SofisCraftShop.UI
{
    public class ItemDetailsDrawer : MonoBehaviour
    {
        [Header("Panel Animation")]
        [SerializeField] 
        private RectTransform drawerRect;
        [SerializeField] 
        private float slideDuration = 0.25f;
        [SerializeField] 
        private Vector2 hiddenPosition = new Vector2(450, 0);
        [SerializeField] 
        private Vector2 visiblePosition = Vector2.zero;

        [Header("Header Elements")]
        [SerializeField] 
        private Image itemIcon;
        [SerializeField] 
        private TMP_Text itemNameText;
        [SerializeField] 
        private TMP_Text itemRarityText;
        [SerializeField] 
        private TMP_Text itemDescriptionText;

        [Header("Recipe & Requirements")]
        [SerializeField] 
        private Transform ingredientContainer;
        [SerializeField] 
        private GameObject ingredientPrefab;

        [Header("Craft Controls")]
        [SerializeField] 
        private Button craftButton;
        [SerializeField] 
        private TMP_Text craftButtonText;
        [SerializeField] 
        private Button closeButton;

        [Header("Dependencies")]
        [SerializeField] 
        private ApiClient apiClient;
        [SerializeField] 
        private LoadingOverlay loadingOverlay;

        private ItemDataSO currentItem;
        private Coroutine slideCoroutine;
        private bool canCraftCurrentItem;

        // Fired when backend confirms a successful craft request
        public event Action<StartCraftResponseDto> OnCraftStartedSuccess;

        private void Awake()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);

            if (craftButton != null)
                craftButton.onClick.AddListener(OnCraftButtonClicked);

            // Ensure drawer starts off-screen
            if (drawerRect != null)
                drawerRect.anchoredPosition = hiddenPosition;
        }

        public void Open(ItemDataSO itemData, Dictionary<string, int> playerInventory)
        {
            if (itemData == null) return;
            currentItem = itemData;

            // 1. Populate metadata
            if (itemIcon != null) itemIcon.sprite = itemData.icon;
            if (itemNameText != null) itemNameText.text = itemData.name;
            if (itemRarityText != null)
            {
                itemRarityText.text = itemData.rarity.ToString();
                itemRarityText.color = itemData.GetRarityColor();
            }
            if (itemDescriptionText != null) itemDescriptionText.text = itemData.description;

            // 2. Populate recipe ingredients
            canCraftCurrentItem = PopulateIngredients(itemData, playerInventory);

            // 3. Update craft button state
            if (craftButton != null)
            {
                craftButton.interactable = canCraftCurrentItem;
            }
            if (craftButtonText != null)
            {
                craftButtonText.text = canCraftCurrentItem ? $"Craft ({itemData .craftTimeSeconds}s)" : "Missing Materials";
            }

            // 4. Slide panel into view
            SlidePanel(visiblePosition);
        }

        public void Hide()
        {
            SlidePanel(hiddenPosition);
        }

        private bool PopulateIngredients(ItemDataSO item, Dictionary<string, int> playerInventory)
        {
            // Clear old requirement rows
            foreach (Transform child in ingredientContainer)
            {
                Destroy(child.gameObject);
            }

            if (item.recipeRequirements == null || item.recipeRequirements.Count == 0)
            {
                return true; // No requirements needed
            }

            bool allRequirementsMet = true;

            foreach (var req in item.recipeRequirements)
            {
                GameObject rowObj = Instantiate(ingredientPrefab, ingredientContainer);
                RecipeIngredientUI rowUI = rowObj.GetComponent<RecipeIngredientUI>();

                int owned = playerInventory.TryGetValue(req.item.itemId, out int count) ? count : 0;
                if (owned < req.quantity)
                {
                    allRequirementsMet = false;
                }

                rowUI.Setup(req.item.icon, req.item.name, req.quantity, owned);
            }

            return allRequirementsMet;
        }

        private async void OnCraftButtonClicked()
        {
            if (currentItem == null || !canCraftCurrentItem) return;

            if (loadingOverlay != null)
                loadingOverlay.Show($"Sending request to craft {currentItem.name}...");

            if (craftButton != null) craftButton.interactable = false;

            StartCraftRequestDto requestData = new StartCraftRequestDto
            {
                itemId = currentItem.itemId,
                quantity = 1
            };

            // Send request to ASP.NET Core backend
            var response = await apiClient.PostAsync<StartCraftRequestDto, StartCraftResponseDto>(
                "/api/crafting/start",
                requestData
            );

            if (loadingOverlay != null) loadingOverlay.Hide();

            if (response != null && response.success)
            {
                Debug.Log($"[Crafting] Server started craft! Queue ID: {response.queueId}");
                OnCraftStartedSuccess?.Invoke(response);
                Hide();
            }
            else
            {
                string errorMsg = response != null ? response.message : "Network error failed to queue item.";
                Debug.LogError($"[Crafting] Craft failed: {errorMsg}");
                if (craftButton != null) craftButton.interactable = true;
            }
        }

        private void SlidePanel(Vector2 targetPosition)
        {
            if (slideCoroutine != null) StopCoroutine(slideCoroutine);
            slideCoroutine = StartCoroutine(AnimateSlide(targetPosition));
        }

        private IEnumerator AnimateSlide(Vector2 targetPosition)
        {
            Vector2 startPosition = drawerRect.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < slideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
                drawerRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            drawerRect.anchoredPosition = targetPosition;
        }
    }
}