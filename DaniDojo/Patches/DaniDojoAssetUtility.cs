using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DaniDojo.Patches
{
    internal class DaniDojoAssetUtility
    {
        public static GameObject CreateText(string name, string text, Rect rect, TMP_FontAsset font, Material material, HorizontalAlignmentOptions alignment, Color32 borderColor, Transform parent)
        {
            GameObject newObj = new GameObject(name);
            newObj.transform.SetParent(parent);

            var tmpText = newObj.AddComponent<TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.horizontalAlignment = alignment;
                tmpText.verticalAlignment = VerticalAlignmentOptions.Middle;
                tmpText.text = text;
                tmpText.enableAutoSizing = true;
                tmpText.fontSizeMax = 1000;

                tmpText.font = font;
                tmpText.fontSharedMaterial = material;

                //if (borderColor != Color.clear)
                //{
                //    tmpText.outlineWidth = 0.2f;
                //    tmpText.outlineColor = borderColor;
                //}

                var rectTransform = newObj.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.pivot = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
                rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
                rectTransform.localScale = Vector3.one;

                var position = tmpText.transform.position;
                position.z = 0;
                tmpText.transform.position = position;
            }

            return newObj;
        }


        public static GameObject CreateImage(string name, string filePath, Vector2 location, Transform parent)
        {
            if (!File.Exists(filePath))
            {
                Plugin.Log.LogError("Could not find file: " + filePath);
                return new GameObject(name);
            }
            
            //var imageCanvas = newImageObj.AddComponent<Canvas>();
            //var parentCanvas = parent.gameObject.GetComponent<Canvas>();
            //imageCanvas.sortingOrder = sortOrder;
            //imageCanvas.overrideSorting = true;
            //imageCanvas.sortingLayerName = "EnsoBG";
            //imageCanvas.worldCamera = parentCanvas.worldCamera;
            //imageCanvas.renderMode = parentCanvas.renderMode;

            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
            tex.LoadImage(File.ReadAllBytes(filePath));

            Rect rect = new Rect(0, 0, tex.width, tex.height);
            var sprite = Sprite.Create(tex, rect, new Vector2(0, 0));

            return CreateImage(name, sprite, location, parent);
        }
        public static GameObject CreateImage(string name, Sprite sprite, Vector2 location, Transform parent)
        {
            GameObject newImageObj = new GameObject(name);
            newImageObj.transform.SetParent(parent);
            newImageObj.layer = LayerMask.NameToLayer("UI");

            var image = newImageObj.AddComponent<Image>();
            image.sprite = sprite;

            var rectTransform = newImageObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.anchoredPosition = location;
            rectTransform.anchoredPosition3D = new Vector3(location.x, location.y, 80);
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height);


            var position = newImageObj.transform.localPosition;
            //position.x = location.x;
            //position.y = location.y;
            position.z = 0;
            newImageObj.transform.localPosition = position;

            return newImageObj;
        }

        public static GameObject CreateNewImage(string name, Color32 color, Rect rect, Transform parent)
        {
            GameObject newImageObj = new GameObject(name);
            newImageObj.transform.SetParent(parent);
            var image = newImageObj.AddComponent<Image>();
            image.color = new Color32(color.r, color.g, color.b, color.a);

            var rectTransform = newImageObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            rectTransform.anchoredPosition = new Vector2(rect.x, rect.y);
            rectTransform.localScale = Vector3.one;

            var position = newImageObj.transform.localPosition;
            //position.x = location.x;
            //position.y = location.y;
            position.z = 0;
            newImageObj.transform.localPosition = position;

            return newImageObj;
        }

        //public static GameObject CreateAnimator(string name, Sprite sprite, Rect rect, Transform parent)
        //{

        //}

        public static void ChangeSprite(string gameObjectName, string newImageFilePath)
        {
            var gameObject = GameObject.Find(gameObjectName);
            ChangeSprite(gameObject, newImageFilePath);
        }

        public static void ChangeSprite(GameObject gameObject, string newImageFilePath)
        {
            if (!File.Exists(newImageFilePath))
            {
                Plugin.Log.LogError("Could not find file: " + newImageFilePath);
                return;
            }
            if (gameObject != null)
            {
                var laneImage = gameObject.GetComponentInChildren<Image>();

                if (laneImage != null && laneImage.sprite != null)
                {
                    Texture2D tex = laneImage.sprite.texture;
                    ImageConversion.LoadImage(tex, File.ReadAllBytes(newImageFilePath));
                    laneImage.sprite = Sprite.Create(tex, new Rect(laneImage.sprite.rect.x, laneImage.sprite.rect.y, tex.width, tex.height), laneImage.sprite.pivot);
                }
                else
                {
                    var canvas = gameObject.GetComponentInChildren<Canvas>();
                    if (canvas != null)
                    {
                        var canvasImage = canvas.GetComponentInChildren<Image>();
                        if (canvasImage != null && canvasImage.sprite != null)
                        {
                            Texture2D tex = canvasImage.sprite.texture;
                            ImageConversion.LoadImage(tex, File.ReadAllBytes(newImageFilePath));
                            canvasImage.sprite = Sprite.Create(tex, new Rect(canvasImage.sprite.rect.x, canvasImage.sprite.rect.y, tex.width, tex.height), canvasImage.sprite.pivot);
                        }
                    }
                }
            }
        }

        public static Sprite CreateSprite(string filePath)
        {
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
            if (File.Exists(filePath))
            {
                tex.LoadImage(File.ReadAllBytes(filePath));
            }

            Rect rect = new Rect(0, 0, tex.width, tex.height);
            return Sprite.Create(tex, rect, new Vector2(0, 0));
        }

        public static GameObject GetChildWithName(GameObject obj, string name)
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

        public static void ChangeImageSprite(GameObject obj, string filePath)
        {
            if (!File.Exists(filePath))
            {
                Plugin.Log.LogError("Could not find file: " + filePath);
                return;
            }
            var image = obj.GetComponent<Image>();

            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
            tex.LoadImage(File.ReadAllBytes(filePath));

            Rect rect = new Rect(0, 0, tex.width, tex.height);
            image.sprite = Sprite.Create(tex, rect, new Vector2(0, 0));
        }

        public static void ChangeImageSprite(GameObject obj, Sprite sprite)
        {
            var image = obj.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = sprite;
            }
        }
    }
}
