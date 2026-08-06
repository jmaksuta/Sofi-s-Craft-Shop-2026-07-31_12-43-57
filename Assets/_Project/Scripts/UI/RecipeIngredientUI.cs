using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SofisCraftShop.UI
{
    public class RecipeIngredientUI : MonoBehaviour
    {
        [SerializeField] 
        private Image ingredientIcon;
        [SerializeField] 
        private TMP_Text ingredientNameText;
        [SerializeField] 
        private TMP_Text quantityText;

        public void Setup(Sprite icon, string name, int requiredCount, int ownedCount)
        {
            if (ingredientIcon != null)
            {
                ingredientIcon.sprite = icon;
                ingredientIcon.enabled = icon != null;
            }

            if (ingredientNameText != null)
            {
                ingredientNameText.text = name;
            }

            if (quantityText != null)
            {
                bool hasEnough = ownedCount >= requiredCount;
                quantityText.text = $"{ownedCount} / {requiredCount}";
                quantityText.color = hasEnough
                    ? new Color(0.4f, 0.8f, 0.4f)  // Soft Green
                    : new Color(0.9f, 0.4f, 0.4f); // Soft Red
            }
        }
    }
}