using SofisCraftShop.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SofisCraftShop.UI
{

    public class InventorySlotUI : MonoBehaviour
    {
        [Header("UI Reference")]
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private TextMeshProUGUI amountText;
        [SerializeField]
        private GameObject emptyState;

        public void SetupSlot(ItemDataSO item, int count)
        {
            if (item == null || count <= 0)
            {
                ClearSlot();
                return;
            }

            iconImage.enabled = true;
            iconImage.sprite = item.icon;

            if (amountText != null)
            {
                amountText.enabled = count > 1;
                amountText.text = count.ToString();
            }

            if (emptyState != null)
            {
                emptyState.SetActive(false);
            }
        }

        public void ClearSlot()
        {
            iconImage.enabled = false;
            iconImage.sprite = null;
            if (amountText != null)
            {
                amountText.enabled = false;
            }
            if (emptyState != null)
            {
                emptyState.SetActive(true);
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}