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

        Image image;
        List<Sprite> rainbowSprites = new List<Sprite>();
        List<Sprite> smallRainbowSprites = new List<Sprite>();
        int currentSprite = 0;
        string rainbowSpriteLocation = Path.Combine(Plugin.Instance.ConfigDaniDojoAssetLocation.Value, "Enso");


        //float rainbowInterval = 1f;
        float rainbowInterval = 0.1f;

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

            timeStarted = Time.time;
        }

        private void Update()
        {
            if (isEnabled)
            {
                image.color = Color.Lerp(mainColor, endColor, Mathf.PingPong(Time.time - timeStarted, interval));
            }
            else if (isRainbow)
            {
                int newSpriteIndex = ((int)((Time.time - timeStarted) / rainbowInterval) % (int)rainbowSprites.Count);
                if (newSpriteIndex != currentSprite)
                {
                    currentSprite = newSpriteIndex;
                    image.sprite = rainbowSprites[currentSprite];
                }
            }
            else if (isSmallRainbow)
            {
                int newSpriteIndex = ((int)((Time.time - timeStarted) / rainbowInterval) % (int)smallRainbowSprites.Count);
                if (newSpriteIndex != currentSprite)
                {
                    currentSprite = newSpriteIndex;
                    image.sprite = smallRainbowSprites[currentSprite];
                }
            }
        }

        public void Begin(Color32 newStartColor, Color32 newEndColor, float newInterval)
        {
            timeStarted = Time.time;

            mainColor = newStartColor;
            endColor = newEndColor;
            interval = newInterval;

            isEnabled = true;
        }

        public void BeginRainbow(bool isLargeRainbow)
        {
            if (isLargeRainbow && !isRainbow && rainbowSprites.Count != 0)
            {
                timeStarted = Time.time;

                image.sprite = rainbowSprites[0];
                image.color = new Color32(255, 255, 255, 255);

                isEnabled = false;
                isRainbow = true;
                isSmallRainbow = false;
            }
            else if (!isLargeRainbow && !isSmallRainbow && smallRainbowSprites.Count != 0)
            {
                timeStarted = Time.time;

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
