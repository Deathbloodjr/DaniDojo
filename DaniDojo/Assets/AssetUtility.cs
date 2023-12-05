using PlayFab.DataModels;
using System;
using System.Collections;
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
        static string AssetFilePath = "";
        static Dictionary<string, Sprite> LoadedSprites;

        static public Sprite LoadSprite(string spriteFilePath)
        {
            var filePath = spriteFilePath;
            // If the dictionary wasn't created yet, create it
            if (LoadedSprites == null)
            {
                LoadedSprites = new Dictionary<string, Sprite>();
            }

            if (AssetFilePath == "")
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Plugin.Instance.ConfigDaniDojoAssetLocation.Value);
                var files = dirInfo.GetFiles("danidojo.scene");
                if (files.Length != 0)
                {
                    AssetFilePath = files[0].Directory.FullName;
                }
            }

            // If the file doesn't start with the Asset path, append it on
            if (!File.Exists(filePath) && !filePath.StartsWith(AssetFilePath))
            {
                filePath = Path.Combine(AssetFilePath, filePath);
            }

            // If the file doesn't end with an extension (".png")
            // Add the extension automatically
            // This feels very hardcodey, but it could be a decent start for something decent
            if (!File.Exists(filePath) && !filePath.Contains("."))
            {
                filePath += ".png";
            }

            // If the dictionary has the filepath as a key, return the corresponding sprite
            if (LoadedSprites.ContainsKey(spriteFilePath))
            {
                return LoadedSprites[spriteFilePath];
            }
            // otherwise, if the file exists, load it, and add it to the dictionary
            else if (File.Exists(filePath))
            {
                LoadedSprites.Add(spriteFilePath, LoadSpriteFromFile(filePath));
                return LoadedSprites[spriteFilePath];
            }
            // otherwise, the file doesn't exist, log an error, and return null (or hopefully a small transparent sprite
            else
            {
                Plugin.LogInfo(LogType.Error, "Could not find file: " + spriteFilePath);
                Plugin.LogInfo(LogType.Error, "Searched for : " + filePath);
                // Instead of null, could I have this return just a 1x1 transparent sprite or something?

                // Creates a transparent 2x2 texture, and returns that as the sprite
                Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
                Color fillColor = Color.clear;
                Color[] fillPixels = new Color[tex.width * tex.height];
                for (int i = 0; i < fillPixels.Length; i++)
                {
                    fillPixels[i] = fillColor;
                }
                tex.SetPixels(fillPixels);
                tex.Apply();

                Rect rect = new Rect(0, 0, tex.width, tex.height);
                LoadedSprites.Add(spriteFilePath, Sprite.Create(tex, rect, new Vector2(0, 0)));
                return LoadedSprites[spriteFilePath];
            }
        }

        static public GameObject GetOrCreateEmptyChild(GameObject parent, string name, Vector2 position)
        {
            var child = GetChildByName(parent, name);
            if (child == null)
            {
                child = CreateEmptyObject(parent, name, position);
            }
            return child;
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


        static public Canvas AddCanvasComponent(GameObject gameObject)
        {
            var canvasObject = gameObject.GetOrAddComponent<Canvas>();
            canvasObject.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.worldCamera = null;
            canvasObject.overrideSorting = true;

            var canvasScalerObject = gameObject.GetOrAddComponent<CanvasScaler>();
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
            var textComponent = newObject.GetOrAddComponent<TextMeshProUGUI>();
            ChangeText(newObject, text);
            textComponent.enableAutoSizing = true;
            textComponent.fontSizeMax = 1000;
            textComponent.verticalAlignment = VerticalAlignmentOptions.Middle;

            return newObject;
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
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();

            ChangeText(textComponent, text);

            return;
        }
        static public void SetTextFontAndMaterial(GameObject gameObject, TMP_FontAsset font, Material material)
        {
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();
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
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();
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
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();
            textComponent.color = color;
        }

        static public void SetTextFontSize(GameObject gameObject, float fontSize)
        {
            var textComponent = gameObject.GetOrAddComponent<TextMeshProUGUI>();
            textComponent.enableAutoSizing = false;
            textComponent.fontSize = fontSize;
        }

        #endregion


        #region Image

        static public GameObject GetOrCreateImageChild(GameObject parent, string name, Vector2 position, string spriteFilePath)
        {
            var imageChild = GetChildByName(parent, name);
            if (imageChild == null)
            {
                imageChild = CreateImageChild(parent, name, position, spriteFilePath);
            }
            else
            {
                imageChild.GetOrAddComponent<Image>().sprite = LoadSprite(spriteFilePath);
            }
            return imageChild;
        }

        static public GameObject CreateImageChild(GameObject parent, string name, Rect rect, Color32 color)
        {
            GameObject newObject = CreateEmptyObject(parent, name, rect);
            var image = newObject.GetOrAddComponent<Image>();
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
            var image = newObject.GetOrAddComponent<Image>();
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

        // This feels kinda repetitive, but I think it's fine
        static public RectTransform SetRect(GameObject gameObject, Rect rect)
        {
            var rectTransform = gameObject.GetOrAddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;

            gameObject.transform.localScale = Vector3.one;

            return rectTransform;
        }
        static public RectTransform SetRect(GameObject gameObject, Vector2 pos)
        {
            var rectTransform = gameObject.GetOrAddComponent<RectTransform>();
            rectTransform.anchoredPosition = pos;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;

            gameObject.transform.localScale = Vector3.one;

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

        public static IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds, bool deleteAfter = false)
        {
            float elapsedTime = 0;
            Vector3 startingPos = objectToMove.transform.position;
            while (elapsedTime < seconds)
            {
                objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            objectToMove.transform.position = end;
            if (deleteAfter)
            {
                GameObject.Destroy(objectToMove);
            }
        }

        public static IEnumerator ChangeTransparencyOverSeconds(GameObject obj, float seconds, bool makeVisible)
        {
            float endValue = makeVisible ? 1f : 0f;
            var image = obj.GetComponent<Image>();
            float imageStartValue = 0f;
            if (image != null)
            {
                imageStartValue = image.color.a;
            }
            var text = obj.GetComponent<TextMeshProUGUI>();
            float textStartValue = 0f;
            if (text != null)
            {
                textStartValue = text.color.a;
            }
            float elapsedTime = 0;
            while (elapsedTime < seconds)
            {
                if (image != null)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(imageStartValue, endValue, elapsedTime / seconds));
                }
                if (text != null)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(textStartValue, endValue, elapsedTime / seconds));
                }
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            if (image != null)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, endValue);
            }
            if (text != null)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, endValue);
            }
        }

        public static Vector2 GetPositionFrom1080p(Vector2 inputPos)
        {
            Vector2 result = new Vector2()
            {
                x = (inputPos.x / 1920f) * Screen.width,
                y = (inputPos.y / 1080f) * Screen.height,
            };
            return result;
        }

        public static Vector3 GetPositionFrom1080p(Vector3 inputPos)
        {
            Vector3 result = new Vector3()
            {
                x = (inputPos.x / 1920f) * Screen.width,
                y = (inputPos.y / 1080f) * Screen.height,
                z = inputPos.z,
            };
            return result;
        }
    }

    public static class Extensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out T t))
            {
                return t;
            }
            else
            {
                return gameObject.AddComponent<T>();
            }
        }
    }
}
