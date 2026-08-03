using SofisCraftShop.Core;
using SofisCraftShop.Data;
using System;
using UnityEngine;

namespace SofisCraftShop.UI
{

    public class ToastManagerUI : MonoBehaviour
    {
        [Header("Container & Prefab")]
        [SerializeField]
        private Transform toastContainerParent;
        [SerializeField]
        private ToastNotificationUI toastPrefab;

        private void OnEnable()
        {
            if (CraftingManager.Instance != null)
            {
                CraftingManager.Instance.OnCraftCompleted += ShowCraftCompletedToast;
            }
        }

        private void OnDisable()
        {
            if (CraftingManager.Instance != null)
            {
                CraftingManager.Instance.OnCraftCompleted -= ShowCraftCompletedToast;
            }
        }

        private void ShowCraftCompletedToast(RecipeDataSO recipe)
        {
            if (toastPrefab == null || toastContainerParent == null) return;

            ToastNotificationUI toastInstance = Instantiate(toastPrefab, toastContainerParent);
            toastInstance.Display(recipe);
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