using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SofisCraftShop.Data
{
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum ItemCategory
    {
        RawMaterial,
        Component,
        Consumable,
        Equipment,
        QuestItem
    }

    [CreateAssetMenu(fileName = "NewItemData", menuName = "Sofi's Craft Shop/Item Data")]
    public class ItemDataSO : ScriptableObject
    {

        [Header("Basic Information")]
        [Tooltip("Must match the exact itemId string returned by your server/database.")]
        public string itemId;

        [Header("Display Metadata")]
        public string itemName;
        [TextArea(2, 5)]
        public string description;
        public Sprite icon;
        public ItemRarity rarity = ItemRarity.Common;
        public ItemCategory category;

        [Header("Economy & Rules")]
        public int baseSellPrice = 10;
        public int maxStackSize = 99;
        public bool isTradable = true;
        public float craftTimeSeconds;

        [Header("Recipe Requirements")]
        public List<IngredientRequirement> recipeRequirements = new List<IngredientRequirement>();

        public Color GetRarityColor()
        {
            return rarity switch
            {
                ItemRarity.Common => new Color(0.85f, 0.85f, 0.85f),    // Soft Grey
                ItemRarity.Uncommon => new Color(0.45f, 0.75f, 0.45f),  // Soft Green
                ItemRarity.Rare => new Color(0.40f, 0.60f, 0.90f),      // Soft Blue
                ItemRarity.Epic => new Color(0.65f, 0.40f, 0.85f),      // Soft Purple
                ItemRarity.Legendary => new Color(0.95f, 0.70f, 0.30f), // Soft Gold
                _ => Color.white
            };
        }

    }

}