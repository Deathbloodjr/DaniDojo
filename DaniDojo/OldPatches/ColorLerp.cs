using DaniDojo.Assets;
using DaniDojo.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DaniDojo.Patches
{
    internal class ColorLerp : MonoBehaviour
    {
        bool isEnabled = false;
        bool isRainbow = false;
        bool isSmallRainbow = false;
        Color32 mainColor;
        Color32 endColor;
        float interval;
        float timeStarted;

        BorderBarData data;


        Image image;
        List<Sprite> rainbowSprites = new List<Sprite>();
        List<Sprite> smallRainbowSprites = new List<Sprite>();
        int currentSprite = 0;
        string rainbowSpriteLocation = Path.Combine(Plugin.Instance.ConfigDaniDojoAssetLocation.Value, "Enso");

        float intervalOneTime = 0.3f;
        float intervalSlow = 0.6f;
        float intervalFast = 0.3f;

        float oneTimeFlashStart = 0.0f;
        float flashStart = 0.0f;

        //float rainbowInterval = 1f;
        float rainbowInterval = 0.09f;

        private void Start()
        {
            image = GetComponent<Image>();
            mainColor = image.color;

            DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(rainbowSpriteLocation, "LargeRainbow"));
            if (dirInfo.Exists)
            {
                var files = dirInfo.GetFiles("*.*");
                for (int i = 0; i < files.Length; i++)
                {
                    rainbowSprites.Add(DaniDojoAssetUtility.CreateSprite(files[i].FullName));
                }
            }

            dirInfo = new DirectoryInfo(Path.Combine(rainbowSpriteLocation, "SmallRainbow"));
            if (dirInfo.Exists)
            {
                var files = dirInfo.GetFiles("*.*");
                for (int i = 0; i < files.Length; i++)
                {
                    smallRainbowSprites.Add(DaniDojoAssetUtility.CreateSprite(files[i].FullName));
                }
            }

            //timeStarted = Time.time;
        }

        private void Update()
        {
            if (image != null)
            {
                if (data.StateChanged)
                {
                    image.color = Color32.Lerp(data.Color, new Color32(255, 255, 255, 255), Mathf.PingPong(Time.time - oneTimeFlashStart / intervalOneTime, intervalOneTime));
                    if (Time.time - oneTimeFlashStart > intervalFast)
                    {
                        data.StateChanged = false;
                        image.color = data.Color;
                    }
                }
                if (data.FlashState == BorderBarFlashState.WhiteSlow)
                {
                    image.color = Color32.Lerp(data.Color, new Color32(255, 255, 255, 255), Mathf.PingPong(Time.time / intervalSlow, intervalSlow));
                }
                else if (data.FlashState == BorderBarFlashState.BlackSlow)
                {
                    image.color = Color32.Lerp(data.Color, new Color32(0, 0, 0, 255), Mathf.PingPong(Time.time / intervalSlow, intervalSlow));
                }
                else if (data.FlashState == BorderBarFlashState.WhiteFast)
                {
                    image.color = Color32.Lerp(data.Color, new Color32(255, 255, 255, 255), Mathf.PingPong(Time.time / intervalFast, intervalFast));
                }

                if (isRainbow || isSmallRainbow)
                {
                    int spriteCount = isRainbow ? rainbowSprites.Count : smallRainbowSprites.Count;
                    int newSpriteIndex = ((int)((Time.time - EnsoAssets.GetRainbowStartTime()) / rainbowInterval) % (int)spriteCount);
                    if (newSpriteIndex != currentSprite)
                    {
                        currentSprite = newSpriteIndex;
                        image.sprite = isRainbow ? rainbowSprites[currentSprite] : smallRainbowSprites[currentSprite];
                    }
                }
            }
        }

        public void UpdateState(BorderBarData newData)
        {
            data = newData;

            if (data.StateChanged)
            {
                oneTimeFlashStart = Time.time;
            }
            if (data.FlashState == BorderBarFlashState.WhiteSlow ||
                data.FlashState == BorderBarFlashState.WhiteFast ||
                data.FlashState == BorderBarFlashState.BlackSlow)
            {
                flashStart = Time.time;
            }

        }

        public void Begin(Color32 newStartColor, Color32 newEndColor, float newInterval)
        {
            //timeStarted = Time.time;

            mainColor = newStartColor;
            endColor = newEndColor;
            interval = newInterval;

            isEnabled = true;
        }

        public void BeginRainbow(bool isLargeRainbow)
        {
            if (isLargeRainbow && !isRainbow && rainbowSprites.Count != 0)
            {
                //timeStarted = Time.time;

                image.sprite = rainbowSprites[0];
                image.color = new Color32(255, 255, 255, 255);

                isEnabled = false;
                isRainbow = true;
                isSmallRainbow = false;
            }
            else if (!isLargeRainbow && !isSmallRainbow && smallRainbowSprites.Count != 0)
            {
                //timeStarted = Time.time;

                image.sprite = smallRainbowSprites[0];
                image.color = new Color32(255, 255, 255, 255);

                isEnabled = false;
                isRainbow = false;
                isSmallRainbow = true;
            }
        }

        public void End()
        {
            image.color = mainColor;
            isEnabled = false;
        }
    }
}
