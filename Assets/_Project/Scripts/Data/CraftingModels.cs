using System;
using System.Collections.Generic;
using UnityEngine;

namespace SofisCraftShop.Data
{

    [Serializable]
    public class PlayerSyncDto
    {
        public int gold;
        public List<InventoryItemDto> inventory = new();
        public List<QueueItemDto> activeQueue = new();
    }

    [Serializable]
    public class InventoryItemDto
    {
        public string itemId;
        public int quantity;
    }

    [Serializable]
    public class QueueItemDto
    {
        public string queueItemId;
        public string recipeId;
        public long startedAtUnix;
        public long completesAtUnix;
        public bool isCompleted;

        public DateTime StartedAtDateTime
        {
            get
            {
                return ToDateTime(this.startedAtUnix);
            }
        }

        public DateTime CompletesAtDateTime
        {
            get
            {
                return ToDateTime(this.completesAtUnix);
            }
        }


        private DateTime ToDateTime(long unixTimestamp)
        {
            // Convert to Local DateTime
            DateTime localDateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).LocalDateTime;
            return localDateTime;
        }

        private DateTime ToUTCDateTime(long unixTimestamp)
        {
            // Convert to UTC DateTime
            DateTime utcDateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
            return utcDateTime;
        }

    }

    [Serializable]
    public class StartCraftRequestDto
    {
        public string itemId;
        public int quantity = 1;
    }

    [Serializable]
    public class StartCraftResponseDto
    {
        public bool success;
        public string message;
        public string queueId;
        public DateTime startTimeUtc;
        public DateTime endTimeUtc;
    }

}