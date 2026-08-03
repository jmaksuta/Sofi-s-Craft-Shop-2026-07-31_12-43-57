using JetBrains.Annotations;
using UnityEngine;

namespace SofisCraftShop.Data
{
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
        public string itemId;
        public string itemName;
        [TextArea(2, 5)]
        public string description;
        public Sprite icon;
        public ItemCategory category;

        [Header("Economy & Rules")]
        public int baseSellPrice = 10;
        public int maxStackSize = 99;
        public bool isTradable = true;

    }

}