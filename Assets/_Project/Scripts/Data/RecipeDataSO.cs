using System;
using System.Collections.Generic;
using UnityEngine;

namespace SofisCraftShop.Data
{
    [Serializable]
    public struct IngredientRequirement
    {
        public ItemDataSO item;
        [Min(1)]
        public int quantity;
    }

    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Sofi's Craft Shop/Recipe Data")]
    public class RecipeDataSO : ScriptableObject
    {
        [Header("Recipe Output")]
        public ItemDataSO resultItem;
        [Min(1)]
        public int resultAmount = 1;

        [Header("Requirements")]
        public List<IngredientRequirement> ingredients = new List<IngredientRequirement>();

        [Header("Crafting Parameters")]
        public float craftTimeInSeconds = 5.0f;
        public int goldCost = 0;
        public int requiredCraftingLevel = 1;
    }

}