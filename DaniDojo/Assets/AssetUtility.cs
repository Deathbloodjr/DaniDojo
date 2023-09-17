using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DaniDojo.Assets
{
    internal class AssetUtility
    {
        static Dictionary<string, Sprite> LoadedSprites;

        static public Sprite LoadSprite(string spriteFilePath)
        {
            if (LoadedSprites == null)
            {
                LoadedSprites = new Dictionary<string, Sprite>();
            }
            if (LoadedSprites.ContainsKey(spriteFilePath))
            {
                return LoadedSprites[spriteFilePath];
            }
            else
            {
                LoadedSprites.Add(spriteFilePath, LoadSpriteFromFile(spriteFilePath));
                return LoadedSprites[spriteFilePath];
            }
        }

        static public GameObject CreateEmptyObject(GameObject parent, string name, Vector2 position)
        {
            Rect rect = new Rect(position, Vector2.zero);
            return CreateEmptyObject(parent, name, rect);
        }

        public static GameObject GetChildByName(GameObject obj, string name)
        {
            Transform trans = obj.transform;
            Transform childTrans = trans.Find(name);
            if (childTrans != null)
            {
                return childTrans.gameObject;
            }
            else
            {
                return null;
            }
        }

        static public GameObject CreateEmptyObject(GameObject parent, string name, Rect rect)
        {
            GameObject newObject = new GameObject(name);
            if (parent != null)
            {
                newObject.transform.SetParent(parent.transform);
            }
            SetRect(newObject, rect);
            return newObject;
        }

        static public Canvas GetOrAddCanvasComponent(GameObject gameObject)
        {
            var canvasObject = gameObject.GetComponent<Canvas>();
            if (canvasObject == null)
            {
                canvasObject = gameObject.AddComponent<Canvas>();
            }
            return canvasObject;
        }

        static public CanvasScaler GetOrAddCanvasScalerComponent(GameObject gameObject)
        {
            var canvasScalerObject = gameObject.GetComponent<CanvasScaler>();
            if (canvasScalerObject == null)
            {
                canvasScalerObject = gameObject.AddComponent<CanvasScaler>();
            }
            return canvasScalerObject;
        }

        static public Canvas AddCanvasComponent(GameObject gameObject)
        {
            var canvasObject = GetOrAddCanvasComponent(gameObject);
            canvasObject.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.worldCamera = null;
            canvasObject.overrideSorting = true;

            var canvasScalerObject = GetOrAddCanvasScalerComponent(gameObject);
            canvasScalerObject.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScalerObject.referenceResolution = new Vector2(1920, 1080);
            canvasScalerObject.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            canvasScalerObject.matchWidthOrHeight = 0;

            return canvasObject;
        }



        #region Text

        static public GameObject CreateTextChild(GameObject parent, string name, Rect rect, string text)
        {
            GameObject newObject = CreateEmptyObject(parent, name, rect);
            var textComponent = GetOrAddTextComponent(newObject);
            ChangeText(newObject, text);
            textComponent.enableAutoSizing = true;
            textComponent.fontSizeMax = 1000;
            textComponent.verticalAlignment = VerticalAlignmentOptions.Middle;

            return newObject;
        }

        static public TextMeshProUGUI GetOrAddTextComponent(GameObject gameObject)
        {
            var textObject = gameObject.GetComponent<TextMeshProUGUI>();
            if (textObject == null)
            {
                textObject = gameObject.AddComponent<TextMeshProUGUI>();
            }
            return textObject;
        }
        static public void ChangeText(TextMeshProUGUI textComponent, string text)
        {
            if (textComponent == null)
            {
                return;
            }
            textComponent.text = text;
        }

        static public void ChangeText(GameObject gameObject, string text)
        {
            var textComponent = GetOrAddTextComponent(gameObject);

            ChangeText(textComponent, text);

            return;
        }
        static public void SetTextFontAndMaterial(GameObject gameObject, TMP_FontAsset font, Material material)
        {
            var textComponent = GetOrAddTextComponent(gameObject);
            SetTextFontAndMaterial(textComponent, font, material);
        }

        static public void SetTextFontAndMaterial(TextMeshProUGUI text, TMP_FontAsset font, Material material)
        {
            if (text != null)
            {
                text.font = font;
                text.fontSharedMaterial = material;
            }
        }

        static public void SetTextAlignment(GameObject gameObject, HorizontalAlignmentOptions horizAlignment = HorizontalAlignmentOptions.Left,
                                                                     VerticalAlignmentOptions vertAlignment = VerticalAlignmentOptions.Top)
        {
            var textComponent = GetOrAddTextComponent(gameObject);
            SetTextAlignment(textComponent, horizAlignment, vertAlignment);
        }

        static public void SetTextAlignment(TextMeshProUGUI text, HorizontalAlignmentOptions horizAlignment = HorizontalAlignmentOptions.Left,
                                                                             VerticalAlignmentOptions vertAlignment = VerticalAlignmentOptions.Top)
        {
            if (text != null)
            {
                text.horizontalAlignment = horizAlignment;
                text.verticalAlignment = vertAlignment;
            }
        }

        static public void SetTextColor(GameObject gameObject, Color color)
        {
            var textComponent = GetOrAddTextComponent(gameObject);
            textComponent.color = color;
        }

        #endregion


        #region Image

        static public GameObject CreateImageChild(GameObject parent, string name, Rect rect, Color32 color)
        {
            GameObject newObject = CreateEmptyObject(parent, name, rect);
            var image = GetOrAddImageComponent(newObject);
            image.color = color;

            return newObject;
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Vector2 position, string spriteFilePath)
        {
            var sprite = LoadSprite(spriteFilePath);
            return CreateImageChild(parent, name, position, sprite);
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Rect rect, string spriteFilePath)
        {
            var sprite = LoadSprite(spriteFilePath);
            return CreateImageChild(parent, name, rect, sprite);
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Vector2 position, Sprite sprite)
        {
            Rect rect = new Rect(position, new Vector2(sprite.rect.width, sprite.rect.height));
            return CreateImageChild(parent, name, rect, sprite);
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Rect rect, Sprite sprite)
        {
            GameObject newObject = CreateEmptyObject(parent, name, rect);
            var image = GetOrAddImageComponent(newObject);
            image.sprite = sprite;

            return newObject;
        }

        static public void ChangeImageColor(GameObject gameObject, Color32 color)
        {
            var image = GetOrAddImageComponent(gameObject);
            image.color = color;
        }

        static public Image GetOrAddImageComponent(GameObject gameObject)
        {
            var imageObject = gameObject.GetComponent<Image>();
            if (imageObject == null)
            {
                imageObject = gameObject.AddComponent<Image>();
            }

            return imageObject;
        }

        static private Sprite LoadSpriteFromFile(string spriteFilePath)
        {
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
            if (!File.Exists(spriteFilePath))
            {
                Plugin.Log.LogError("Could not find file: " + spriteFilePath);
                //return null;
            }
            else
            {
                tex.LoadImage(File.ReadAllBytes(spriteFilePath));
            }


            Rect rect = new Rect(0, 0, tex.width, tex.height);
            return Sprite.Create(tex, rect, new Vector2(0, 0));
        }

        static public Image ChangeImageSprite(GameObject gameObject, string spriteFilePath)
        {
            var image = GetOrAddImageComponent(gameObject);
            return ChangeImageSprite(image, spriteFilePath);
        }

        static public Image ChangeImageSprite(GameObject gameObject, Sprite sprite)
        {
            var image = GetOrAddImageComponent(gameObject);
            return ChangeImageSprite(image, sprite);
        }

        static public Image ChangeImageSprite(Image image, string spriteFilePath)
        {
            var sprite = LoadSprite(spriteFilePath);
            if (sprite == null)
            {
                return image;
            }
            return ChangeImageSprite(image, sprite);
        }

        static public Image ChangeImageSprite(Image image, Sprite sprite)
        {
            image.sprite = sprite;
            return image;
        }



        #endregion


        #region RectTransform
        static public RectTransform GetRectTransform(GameObject gameObject)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return rectTransform;
            }
            return gameObject.AddComponent<RectTransform>();
        }

        // This feels kinda repetitive, but I think it's fine
        static public RectTransform SetRect(GameObject gameObject, Rect rect)
        {
            var rectTransform = GetRectTransform(gameObject);
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            return rectTransform;
        }
        static public RectTransform SetRect(GameObject gameObject, Rect rect, Vector2 anchorMin, Vector2 anchorMax)
        {
            var rectTransform = SetRect(gameObject, rect);
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            return rectTransform;
        }
        static public void SetRect(GameObject gameObject, Rect rect, Vector2 pivot)
        {
            var rectTransform = SetRect(gameObject, rect);
            rectTransform.pivot = pivot;
        }
        static public void SetRect(GameObject gameObject, Rect rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            var rectTransform = SetRect(gameObject, rect, anchorMin, anchorMax);
            rectTransform.pivot = pivot;
        }

        #endregion
    }
}
