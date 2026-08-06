using SofisCraftShop.Core;
using SofisCraftShop.Data;
using System;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace SofisCraftShop.UI
{

    public class RecipeCardUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField]
        private UnityEngine.UI.Image resultIcon;
        [SerializeField]
        private TextMeshProUGUI titleText;
        [SerializeField]
        private TextMeshProUGUI ingredientsText;
        [SerializeField]
        private UnityEngine.UI.Button craftButton;

        private RecipeDataSO boundRecipe;

        public void SetupCard(RecipeDataSO recipe)
        {
            boundRecipe = recipe;

            titleText.text = recipe.name;

            if (recipe.resultItem != null)
            {
                resultIcon.sprite = recipe.resultItem.icon;
            }

            string reqs = "Requires: ";
            for (int n = 0; n < recipe.ingredients.Count; n++)
            {
                var ing = recipe.ingredients[n];
                if (ing.item == null) continue;

                reqs += $"{ing.item.name} x{ing.quantity}";
                if (n < recipe.ingredients.Count - 1) reqs += ", ";
            }

            if (recipe.goldCost > 0)
            {
                reqs += $" | {recipe.goldCost} Gold";
            }

            ingredientsText.text = reqs;

            craftButton.onClick.RemoveAllListeners();
            craftButton.onClick.AddListener(OnCraftButtonClicked);

            RefreshButtonState();
        }

        public void RefreshButtonState()
        {
            if (boundRecipe == null || CraftingManager.Instance == null) return;

            bool canCraft = CraftingManager.Instance.HasIngredients(boundRecipe);
            craftButton.interactable = canCraft;
        }

        private void OnCraftButtonClicked()
        {

            if (boundRecipe == null || CraftingManager.Instance == null) return;

            CraftingManager.Instance.StartCraft(boundRecipe.resultItem.itemId);

            //bool success = CraftingManager.Instance.TryStartCraft(boundRecipe);
            //if (success)
            //{
            //    RefreshButtonState();
            //}
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