using System;
using UnityEngine;

namespace SofisCraftShop.Core
{

    [Serializable]
    public class InventorySlot
    {
        public string itemId;
        public int count;

        public InventorySlot() : base()
        {
            this.itemId = string.Empty;
            this.count = 0;
        }

        public InventorySlot(string itemId, int count) : this()
        {
            this.itemId = itemId;
            this.count = count;
        }

    }

}