using UnityEngine;
using System.Collections.Generic;
using SofisCraftShop.Data;
using SofisCraftShop.Core;
using System;

namespace SofisCraftShop.UI
{

    public class RecipeListPanelUI : MonoBehaviour
    {
        [Header("Recipe Catalog")]
        [SerializeField]
        private List<RecipeDataSO> availableRecipes = new List<RecipeDataSO>();

        [Header("UI References")]
        [SerializeField]
        private Transform contentParent;
        [SerializeField]
        private RecipeCardUI recipeCardPrefab;

        private List<RecipeCardUI> spawnedCards = new List<RecipeCardUI>();

        private void OnEnable()
        {
            if (CraftingManager.Instance != null)
            {
                CraftingManager.Instance.OnInventoryUpdated += RefreshAllCardStates;
            }
            PopulateRecipeList();
        }

        private void OnDisable()
        {
            if (CraftingManager.Instance != null)
            {
                CraftingManager.Instance.OnInventoryUpdated -= RefreshAllCardStates;
            }
        }

        private void PopulateRecipeList()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            spawnedCards.Clear();

            foreach (var recipe in availableRecipes)
            {
                if (recipe == null) continue;

                RecipeCardUI cardInstance = Instantiate(recipeCardPrefab, contentParent);
                cardInstance.SetupCard(recipe);
                spawnedCards.Add(cardInstance);
            }
        }

        private void RefreshAllCardStates()
        {
            foreach (var card in spawnedCards)
            {
                if (card != null)
                {
                    card.RefreshButtonState();
                }
            }
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