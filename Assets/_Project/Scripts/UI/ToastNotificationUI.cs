using SofisCraftShop.Data;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SofisCraftShop.UI
{

    [RequireComponent(typeof(CanvasGroup))]
    public class ToastNotificationUI : MonoBehaviour
    {

        [Header("UI References")]
        [SerializeField]
        private Image itemIcon;
        [SerializeField]
        private TextMeshProUGUI messageText;

        [Header("Animation Settings")]
        [SerializeField]
        private float fadeDuration = 0.4f;
        [SerializeField]
        private float displayDuration = 2.0f;

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
        }

        public void Display(RecipeDataSO recipe)
        {
            if (recipe == null || recipe.resultItem == null) return;

            itemIcon.sprite = recipe.resultItem.icon;
            messageText.text = $"Crafted {recipe.resultItem.itemName} x{recipe.resultAmount}!";

            StartCoroutine(AnimateToast());
        }

        private IEnumerator AnimateToast()
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;

            yield return new WaitForSeconds(displayDuration);

            timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer/ fadeDuration);
                yield return null;  
            }
            canvasGroup.alpha = 0f;

            Destroy(gameObject);
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