using DaniDojo.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DaniDojo.Assets
{
    internal class DaniDojoCourseSelectIcon : MonoBehaviour
    {
        int yTop = -42;
        int yBot = -47;
        float verticalMovementInterval = 0.18f;

        float alphaMax = 1.5f;
        float alphaMin = 0.0f;
        float alphaChangeInterval = 0.36f;

        Image background;
        Image text;

        private void Start()
        {

        }

        public void Initialize(DaniCourseLevel level)
        {
            if (level == DaniCourseLevel.None)
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
                gameObject.SetActive(true);
            }

            var bgImageName = SongSelectAssets.GetBackgroundFromCourseLevel(level);
            var textImageName = SongSelectAssets.GetTextImageNameFromCourseLevel(level);

            var backgroundObj = AssetUtility.GetOrCreateImageChild(gameObject, "Background", new Vector2(0, 0), Path.Combine("SongSelect", "CourseSelect", "Backgrounds", bgImageName));
            var textObj = AssetUtility.GetOrCreateImageChild(gameObject, "Text", new Vector2(18, 27), Path.Combine("SongSelect", "CourseSelect", "Text", textImageName));

            background = backgroundObj.GetComponent<Image>();
            text = textObj.GetComponent<Image>();
        }

        private void Update()
        {
            var verticalPingPong = Mathf.PingPong(Time.time, verticalMovementInterval) / verticalMovementInterval;
            transform.localPosition = new Vector2(transform.localPosition.x, ((yTop - yBot) * verticalPingPong) + yBot);

            var transparentPingPong = Mathf.PingPong(Time.time, alphaChangeInterval) / alphaChangeInterval;
            if (background != null)
            { 
                background.color = new Color(background.color.r, background.color.g, background.color.b, ((alphaMax - alphaMin) * transparentPingPong) + alphaMin);
            }
            if (text != null)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, ((alphaMax - alphaMin) * transparentPingPong) + alphaMin);
            }
        }
    }
}
