using SofisCraftShop.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SofisCraftShop.UI
{

    public class CraftingQueueSlotUI : MonoBehaviour
    {
        [Header("UI Bindings")]
        [SerializeField]
        private Image itemIcon;
        [SerializeField]
        private TMP_Text itemNameText;
        [SerializeField]
        private TMP_Text timerText;
        [SerializeField]
        private Slider progressBar;
        [SerializeField]
        private Button claimButton;
        [SerializeField]
        private TMP_Text claimButtonText;

        private string queueItemId;
        private string itemId;
        private DateTime startTime;
        private DateTime endTime;
        private Action<string> onClaimCallback;
        private bool isCompleted;

        //[Header("UI References")]
        //[SerializeField]
        //private Image resultIcon;
        //[SerializeField]
        //private TextMeshProUGUI recipeTitleText;
        //[SerializeField]
        //private TextMeshProUGUI timerText;
        //[SerializeField]
        //private Slider progressBar;

        private CraftingQueueTask boundTask;

        private void Awake()
        {
            if (claimButton != null)
            {
                claimButton.onClick.AddListener(OnClaimClicked);
            }
        }

        public void SetupSlot(string queueId, string recipeItemId, Sprite icon, string name, DateTime start, DateTime end, Action<string> onClaim)
        {
            queueItemId = queueId;
            itemId = recipeItemId;
            startTime = start;
            endTime = end;
            this.onClaimCallback = onClaim;

            if (itemIcon != null)
            {
                itemIcon.sprite = icon;
                itemIcon.enabled = icon != null;
            }

            if (itemNameText != null)
            {
                itemNameText.text = name;
            }

            UpdateState();
        }

        private void Update()
        {
            if (isCompleted) return;
            UpdateState();
        }

        private void UpdateState()
        {
            DateTime now = DateTime.UtcNow;
            double totalDuration = (endTime - startTime).TotalSeconds;
            double remainingSeconds = (endTime - now).TotalSeconds;

            if (remainingSeconds <= 0)
            {
                // Crafting complete state
                isCompleted = true;

                if (timerText != null) timerText.text = "Ready!";
                if (progressBar != null) progressBar.value = 1f;

                if (claimButton != null)
                {
                    claimButton.gameObject.SetActive(true);
                    claimButton.interactable = true;
                }
                if (claimButtonText != null) claimButtonText.text = "Claim";
            }
            else
            {
                // Crafting in-progress state
                isCompleted = false;

                // Format countdown (e.g., "01:45" or "00:08")
                TimeSpan t = TimeSpan.FromSeconds(remainingSeconds);
                if (timerText != null)
                {
                    timerText.text = t.Hours > 0
                        ? $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}"
                        : $"{t.Minutes:D2}:{t.Seconds:D2}";
                }

                // Progress ratio (0.0 to 1.0)
                if (progressBar != null && totalDuration > 0)
                {
                    double elapsed = (now - startTime).TotalSeconds;
                    progressBar.value = Mathf.Clamp01((float)(elapsed / totalDuration));
                }

                if (claimButton != null)
                {
                    claimButton.gameObject.SetActive(false);
                }
            }
        }

        private void OnClaimClicked()
        {
            if (!isCompleted) return;
            claimButton.interactable = false; // Prevent double clicks
            onClaimCallback?.Invoke(queueItemId);
        }
    }
}


//public void BindTask(CraftingQueueTask task)
//        {
//            boundTask = task;

//            if (task != null && task.recipe != null)
//            {
//                recipeTitleText.text = task.recipe.name;
//                if (task.recipe.resultItem != null)
//                {
//                    resultIcon.sprite = task.recipe.resultItem.icon;
//                    resultIcon.enabled = true;
//                }
//                UpdateUI();
//            }
//        }

//        // Start is called once before the first execution of Update after the MonoBehaviour is created
//        void Start()
//        {

//        }

//        // Update is called once per frame
//        void Update()
//        {
//            if (boundTask != null && !boundTask.isCompleted)
//            {
//                UpdateUI();
//            }
//        }
        
//        private void UpdateUI()
//        {
//            float progress = Mathf.Clamp01(boundTask.elapsedTime / boundTask.duration);
//            progressBar.value = progress;

//            float remainingSeconds = Mathf.Max(0, boundTask.duration - boundTask.elapsedTime);
//            timerText.tag = FormatTime(remainingSeconds);
//        }

//        private string FormatTime(float seconds)
//        {
//            int mins = Mathf.FloorToInt(seconds / 60f);
//            int secs = Mathf.FloorToInt(seconds % 60f);

//            return string.Format("{0:00}:{1:00}", mins, secs);
//        }

//    }

//}