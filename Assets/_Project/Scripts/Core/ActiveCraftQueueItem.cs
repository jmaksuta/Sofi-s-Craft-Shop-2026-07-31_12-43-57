using System;
using UnityEngine;

namespace SofisCraftShop.Core
{

    [Serializable]
    public class ActiveCraftQueueItem
    {
        public string recipeId;
        public long startUnixTimestamp;
        public long finishUnixTimestamp;
        public bool isCompleted;

    }

}