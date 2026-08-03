using SofisCraftShop.Data;
using System;
using UnityEngine;

namespace SofisCraftShop.Core
{

    [Serializable]
    public class CraftingQueueTask
    {
        public RecipeDataSO recipe;
        public float duration;
        public float elapsedTime;
        public bool isCompleted;

        public CraftingQueueTask(RecipeDataSO recipe)
        {
            this.recipe = recipe;
            this.duration = recipe.craftTimeInSeconds;
            this.elapsedTime = 0f;
            this.isCompleted = false;
        }

    }

}
