using System.Collections;
using UnityEngine;
using TMPro;

namespace SofisCraftShop.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingOverlay : MonoBehaviour
    {
        [Header("UI Component References")]
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private RectTransform spinnerTransform;
        [SerializeField] private float spinSpeed = 250f;
        [SerializeField] private float fadeDuration = 0.25f;

        private CanvasGroup canvasGroup;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            // Continuously rotate the spinner icon
            if (spinnerTransform != null && gameObject.activeInHierarchy)
            {
                spinnerTransform.Rotate(0f, 0f, -spinSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Shows the loading overlay with a custom message.
        /// </summary>
        public void Show(string message = "Loading...")
        {
            if (statusText != null)
            {
                statusText.text = message;
            }

            gameObject.SetActive(true);

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeRoutine(1f));
        }

        /// <summary>
        /// Hides the loading overlay smoothly.
        /// </summary>
        public void Hide()
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeRoutine(0f, () => gameObject.SetActive(false)));
        }

        private IEnumerator FadeRoutine(float targetAlpha, System.Action onComplete = null)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            canvasGroup.blocksRaycasts = targetAlpha > 0.5f;
            canvasGroup.interactable = targetAlpha > 0.5f;

            onComplete?.Invoke();
        }
    }
}