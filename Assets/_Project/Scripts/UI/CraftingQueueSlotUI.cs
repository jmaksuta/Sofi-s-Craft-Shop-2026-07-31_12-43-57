using SofisCraftShop.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SofisCraftShop.UI
{

    public class CraftingQueueSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField]
        private Image resultIcon;
        [SerializeField]
        private TextMeshProUGUI recipeTitleText;
        [SerializeField]
        private TextMeshProUGUI timerText;
        [SerializeField]
        private Slider progressBar;

        private CraftingQueueTask boundTask;

        public void BindTask(CraftingQueueTask task)
        {
            boundTask = task;

            if (task != null && task.recipe != null)
            {
                recipeTitleText.text = task.recipe.name;
                if (task.recipe.resultItem != null)
                {
                    resultIcon.sprite = task.recipe.resultItem.icon;
                    resultIcon.enabled = true;
                }
                UpdateUI();
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (boundTask != null && !boundTask.isCompleted)
            {
                UpdateUI();
            }
        }
        
        private void UpdateUI()
        {
            float progress = Mathf.Clamp01(boundTask.elapsedTime / boundTask.duration);
            progressBar.value = progress;

            float remainingSeconds = Mathf.Max(0, boundTask.duration - boundTask.elapsedTime);
            timerText.tag = FormatTime(remainingSeconds);
        }

        private string FormatTime(float seconds)
        {
            int mins = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);

            return string.Format("{0:00}:{1:00}", mins, secs);
        }

    }

}