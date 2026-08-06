using SofisCraftShop.Core;
//using SofisCraftShop.Managers;
using SofisCraftShop.Network;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SofisCraftShop.UI
{
    public class MainGameSceneController : BaseSceneController
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

        [SerializeField]
        private CraftingQueueManager craftingQueueManager;


        [Header("Scene Config")]
        [SerializeField]
        private string loginSceneName = "Scene_Login";


        private async void Start()
        {
            base.Start();
            // Fetch initial server state on scene boot
            if (CraftingManager.Instance != null)
            {
                // TODO: fix this.
                CraftingManager.Instance.OnSyncDataUpdated += HandleSyncDataUpdated;
                await CraftingManager.Instance.RefreshServerStateAsync();
            }
        }

        private void OnDestroy()
        {
            if (CraftingManager.Instance != null)
            {
                // TODO: fix this.
                CraftingManager.Instance.OnSyncDataUpdated -= HandleSyncDataUpdated;
            }
        }

        public override void LoginSuccess(Data.PlayerSyncDto syncData)
        {
            HandleSyncDataUpdated(syncData);
            HideLoading();
        }

        public override void LoginFailure()
        {
            TransitionToLogin();
        }

        public override void UserNotLoggedIn()
        {
            TransitionToLogin();
        }

        private void TransitionToLogin()
        {
            ShowLoading("Loading shop...");
            SceneManager.LoadScene(loginSceneName);
        }

        // TODO: fix this.
        private void HandleSyncDataUpdated(Data.PlayerSyncDto syncData)
        {
            if (syncData == null) return;


            if (craftingQueueManager != null)
            {
                craftingQueueManager.RefreshQueue(syncData.activeQueue);
            }


            // Update Header HUD
            if (goldCountText != null)
            {
                goldCountText.text = syncData.gold.ToString("N0");
            }

            Debug.Log($"[MainGame] UI updated with {syncData.inventory.Count} items and {syncData.activeQueue.Count} crafting tasks.");
        }
    }
}