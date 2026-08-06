using SofisCraftShop.Data;
using System.Collections.Generic;

namespace SofisCraftShop.Core
{

    public static class CraftingEvaluator
    {

        public static bool CanCraft(RecipeDataSO recipe, Dictionary<string, int> playerInventory, int playerGold)
        {
            if (recipe == null) return false;
            if (playerGold < recipe.goldCost) return false;

            foreach (var ingredient in recipe.ingredients)
            {
                if (ingredient.item == null) continue;

                string id = ingredient.item.itemId;
                int requiredAmount = ingredient.quantity;

                if (playerInventory.TryGetValue(id, out int availableAmount) || availableAmount < requiredAmount)
                {
                    return false;
                }
            }

            return true;
        }

        public static void GetRawMaterialCost(RecipeDataSO recipe, Dictionary<ItemDataSO, RecipeDataSO> recipeDatabase, ref Dictionary<ItemDataSO, int> cumulativeIngredients)
        {
            foreach (var ingredient in recipe.ingredients)
            {
                if (recipeDatabase.TryGetValue(ingredient.item, out RecipeDataSO subRecipe))
                {
                    for (int i = 0; i < ingredient.quantity; i++)
                    {
                        GetRawMaterialCost(subRecipe, recipeDatabase, ref cumulativeIngredients);
                    }
                }
                else
                {
                    if (!cumulativeIngredients.ContainsKey(ingredient.item))
                    {
                        cumulativeIngredients[ingredient.item] = 0;
                    }
                    cumulativeIngredients[ingredient.item] += ingredient.quantity;
                }
            }
        }

    }

}