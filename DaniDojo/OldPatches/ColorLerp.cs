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
    // This entire class sucks, but it works for now
    internal class ColorLerp : MonoBehaviour
    {
        bool isEnabled = false;
        bool isRainbow = false;
        bool isSmallRainbow = false;
        bool isSmallResultRainbow = false;

        bool isResult = false;

        Color32 mainColor;
        Color32 endColor;
        float interval;
        float timeStarted;

        BorderBarData data;


        Image image;
        List<Sprite> rainbowSprites = new List<Sprite>();
        List<Sprite> smallRainbowSprites = new List<Sprite>();
        List<Sprite> smallResultRainbowSprites = new List<Sprite>();

        int currentSprite = 0;

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

            InitializeSprites();
        }

        private void InitializeSprites()
        {
            rainbowSprites = new List<Sprite>();
            DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(Plugin.Instance.ConfigDaniDojoAssetLocation.Value, "Enso", "LargeRainbow"));
            if (dirInfo.Exists)
            {
                var files = dirInfo.GetFiles("*.*");
                for (int i = 0; i < files.Length; i++)
                {
                    rainbowSprites.Add(AssetUtility.LoadSprite(files[i].FullName));
                }
            }

            smallRainbowSprites = new List<Sprite>();
            dirInfo = new DirectoryInfo(Path.Combine(Plugin.Instance.ConfigDaniDojoAssetLocation.Value, "Enso", "SmallRainbow"));
            if (dirInfo.Exists)
            {
                var files = dirInfo.GetFiles("*.*");
                for (int i = 0; i < files.Length; i++)
                {
                    smallRainbowSprites.Add(AssetUtility.LoadSprite(files[i].FullName));
                }
            }

            smallResultRainbowSprites = new List<Sprite>();
            dirInfo = new DirectoryInfo(Path.Combine(Plugin.Instance.ConfigDaniDojoAssetLocation.Value, "Results", "SmallRainbow"));
            if (dirInfo.Exists)
            {
                var files = dirInfo.GetFiles("*.*");
                for (int i = 0; i < files.Length; i++)
                {
                    smallResultRainbowSprites.Add(AssetUtility.LoadSprite(files[i].FullName));
                }
            }
        }

        private void Update()
        {
            if (image != null)
            {
                if (data != null && !isResult)
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
                }
               

                if (isRainbow || isSmallRainbow || isSmallResultRainbow)
                {
                    List<Sprite> spriteList = null;
                    if (isRainbow)
                    {
                        spriteList = rainbowSprites;
                    }
                    else if (isSmallRainbow)
                    {
                        spriteList = smallRainbowSprites;
                    }
                    else if (isSmallResultRainbow)
                    {
                        spriteList = smallResultRainbowSprites;
                    }
                    int newSpriteIndex = ((int)((Time.time - EnsoAssets.GetRainbowStartTime()) / rainbowInterval) % (int)spriteList.Count);
                    if (newSpriteIndex != currentSprite)
                    {
                        currentSprite = newSpriteIndex;
                        image.sprite = spriteList[currentSprite];
                    }
                }
            }
        }

        public void UpdateState(BorderBarData newData, bool isTotal, bool isResult = false)
        {
            data = newData;
            this.isResult = isResult;

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
            if (data.State == BorderBarState.Rainbow)
            {
                BeginRainbow(isTotal, isResult);
            }
        }

        public void Begin(Color32 newStartColor, Color32 newEndColor, float newInterval)
        {
            mainColor = newStartColor;
            endColor = newEndColor;
            interval = newInterval;

            isEnabled = true;
        }

        public void BeginRainbow(bool isLargeRainbow, bool isResult = false)
        {
            image = GetComponent<Image>();
            image.color = new Color32(255, 255, 255, 255);
            isEnabled = false;

            if (rainbowSprites.Count == 0 || smallRainbowSprites.Count == 0)
            {
                InitializeSprites();
            }

            if (isLargeRainbow)
            {
                //image.sprite = rainbowSprites[0];

                isRainbow = true;
                isSmallRainbow = false;
                isSmallResultRainbow = false;
            }
            else if (!isLargeRainbow && !isResult)
            {
                //image.sprite = smallRainbowSprites[0];

                isRainbow = false;
                isSmallRainbow = true;
                isSmallResultRainbow = false;
            }
            else if (!isLargeRainbow && isResult)
            {
                //image.sprite = smallResultRainbowSprites[0];

                isRainbow = false;
                isSmallRainbow = false;
                isSmallResultRainbow = true;
            }
        }

        public void End()
        {
            image.color = mainColor;
            isEnabled = false;
        }
    }
}
