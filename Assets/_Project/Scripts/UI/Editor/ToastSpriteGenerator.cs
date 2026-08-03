using UnityEngine;
using UnityEditor;
using System.IO;

namespace SofisCraftShop.EditorTools
{
    public class ToastSpriteGenerator : EditorWindow
    {
        [MenuItem("Sofi's Tools/Generate Toast Banner Sprite")]
        public static void GenerateRoundedSprite()
        {
            int width = 128;
            int height = 128;
            int cornerRadius = 32;

            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[width * height];

            Vector2 centerBL = new Vector2(cornerRadius, cornerRadius);
            Vector2 centerTL = new Vector2(cornerRadius, height - cornerRadius);
            Vector2 centerBR = new Vector2(width - cornerRadius, cornerRadius);
            Vector2 centerTR = new Vector2(width - cornerRadius, height - cornerRadius);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool inside = true;

                    // Corner Distance Checks
                    if (x < cornerRadius && y < cornerRadius)
                        inside = Vector2.Distance(new Vector2(x, y), centerBL) <= cornerRadius;
                    else if (x < cornerRadius && y >= height - cornerRadius)
                        inside = Vector2.Distance(new Vector2(x, y), centerTL) <= cornerRadius;
                    else if (x >= width - cornerRadius && y < cornerRadius)
                        inside = Vector2.Distance(new Vector2(x, y), centerBR) <= cornerRadius;
                    else if (x >= width - cornerRadius && y >= height - cornerRadius)
                        inside = Vector2.Distance(new Vector2(x, y), centerTR) <= cornerRadius;

                    pixels[y * width + x] = inside ? Color.white : Color.clear;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();

            // Save to Assets directory
            string dirPath = "Assets/_Project/Sprites/UI/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            string filePath = dirPath + "UI_ToastBackground.png";
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);

            AssetDatabase.Refresh();

            // Automatically configure texture import settings for UI 9-slicing
            TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteBorder = new Vector4(cornerRadius, cornerRadius, cornerRadius, cornerRadius);
                importer.SaveAndReimport();
            }

            Debug.Log($"<color=green>Successfully generated 9-Sliced Toast Sprite at: {filePath}</color>");
        }
    }
}