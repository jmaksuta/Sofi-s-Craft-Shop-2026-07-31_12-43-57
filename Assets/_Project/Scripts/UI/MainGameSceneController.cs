using SofisCraftShop.Core;
//using SofisCraftShop.Managers;
using SofisCraftShop.Network;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SofisCraftShop.UI
{
    public class MainGameSceneController : MonoBehaviour
    {
        [Header("HUD Header Bindings")]
        [SerializeField] 
        private TMP_Text goldCountText;
        [SerializeField] 
        private TMP_Text playerTitleText;

        [Header("Panels & Canvas Views")]
        [SerializeField] 
        private GameObject shopViewPanel;
        [SerializeField] 
        private GameObject inventoryPanel;
        [SerializeField] 
        private GameObject craftingQueuePanel;

        private async void Start()
        {
            // Fetch initial server state on scene boot
            if (CraftingManager.Instance != null)
            {
                // TODO: fix this.
                //CraftingManager.Instance.OnSyncDataUpdated += HandleSyncDataUpdated;
                //await CraftingManager.Instance.RefreshServerStateAsync();
            }
        }

        private void OnDestroy()
        {
            if (CraftingManager.Instance != null)
            {
                // TODO: fix this.
                //CraftingManager.Instance.OnSyncDataUpdated -= HandleSyncDataUpdated;
            }
        }

        // TODO: fix this.
        //private void HandleSyncDataUpdated(Data.PlayerSyncDto syncData)
        //{
        //    if (syncData == null) return;

        //    // Update Header HUD
        //    if (goldCountText != null)
        //    {
        //        goldCountText.text = syncData.gold.ToString("N0");
        //    }

        //    Debug.Log($"[MainGame] UI updated with {syncData.inventory.Count} items and {syncData.activeQueue.Count} crafting tasks.");
        //}
    }
}