using DaniDojo.Data;
using DaniDojo.Hooks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DaniDojo.Assets
{
    internal class EnsoAssets
    {
        static string AssetFilePath => Plugin.Instance.ConfigDaniDojoAssetLocation.Value;
        static public void ChangeCourseIcon(GameObject gameObject)
        {
        }

        #region TopObjectCreation

        static public GameObject CreateTopBg(GameObject parent)
        {
            return AssetUtility.CreateImageChild(parent, "DaniTopBg", new Vector2(0, 800), Path.Combine(AssetFilePath, "Enso", "TopBg.png"));
        }

        static public void CreateTopAnimatedParts(GameObject parent)
        {
            CreateTopAnimatedBlueBack(parent);

            CreateTopAnimatedBlueMid(parent);
            CreateTopAnimatedBlueFront(parent);
            
            CreateTopAnimatedPetals(parent);
            CreateTopAnimatedFlowers(parent);

        }

        static private void CreateTopAnimatedFlowers(GameObject parent)
        {
            // There are 3 instances of these flowers, and they all move at the same rates at all times
            // When one instance is just moving off the left side of the screen, one instance is coming into view from the right
            for (int i = 0; i < 3; i++)
            {
                int x = (1920 / 2) * i;
                var flowerParent = AssetUtility.CreateEmptyObject(parent, "Flowers" + i, new Vector2(x, 0));
                var flowersTop = AssetUtility.CreateImageChild(flowerParent, "FlowersTop", new Vector2(300, 195), Path.Combine(AssetFilePath, "Enso", "TopBg", "DonBgFlowersTop.png"));
                var flowersMid = AssetUtility.CreateImageChild(flowerParent, "FlowersMid", new Vector2(279, 119), Path.Combine(AssetFilePath, "Enso", "TopBg", "DonBgFlowersMid.png"));
                var flowersBot = AssetUtility.CreateImageChild(flowerParent, "FlowersBot", new Vector2(0, -155), Path.Combine(AssetFilePath, "Enso", "TopBg", "DonBgFlowersBot.png"));

                IEnumerator flowersCoroutine = TopBgFlowersAnimation(flowerParent, 45f);
                Plugin.Instance.StartCustomCoroutine(flowersCoroutine);
            }
        }

        static private void CreateTopAnimatedPetals(GameObject parent)
        {
            // TODO: Make this more accurate to the official game
            for (int i = 0; i < 3; i++)
            {
                int x = (1920 / 2) * i;
                var petalParent = AssetUtility.CreateEmptyObject(parent, "Petals" + i, new Vector2(x, 0));
                var petals = AssetUtility.CreateImageChild(petalParent, "Petals", new Vector2(0, 0), Path.Combine(AssetFilePath, "Enso", "TopBg", "DonBgPetals.png"));

                IEnumerator flowersCoroutine = TopBgPetalAnimation(petalParent, 250f);
                Plugin.Instance.StartCustomCoroutine(flowersCoroutine);
            }
        }

        static private void CreateTopAnimatedBlueBack(GameObject parent)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int x;
                    int y;
                    if (j == 0)
                    {
                        x = 1920 * i;
                        y = 155;
                    }
                    else
                    {
                        x = (1920 * i) + 248;
                        y = -23;
                    }
                    var blueBack = AssetUtility.CreateImageChild(parent, "BlueBack" + (i * 2 + j * 1), new Vector2(x, y), Path.Combine(AssetFilePath, "Enso", "TopBg", "DonBgBlueBack.png"));
                    if (j == 1)
                    {
                        blueBack.transform.localScale = new Vector3(-1, 1, 1);
                        var rect = blueBack.GetComponent<RectTransform>();
                        //rect.pivot = new Vector2(-1, 0);
                    }
                    IEnumerator bgBlueBackCoroutine = TopBgBlueBackAnimation(blueBack, 96f);
                    Plugin.Instance.StartCustomCoroutine(bgBlueBackCoroutine);
                }
            }
        }

        static private void CreateTopAnimatedBlueMid(GameObject parent)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int x = 810 * (i + 1);
                    int y = 49;
                    switch (j)
                    {
                        case 0:
                            x = 810 + (1920 * i);
                            y = 49;
                            break;
                        case 1:
                            x = 1433 + (1920 * i);
                            y = 72;
                            break;
                        case 2:
                            x = 211 + (1920 * i);
                            y = 159;
                            break;
                    }
                    var blueMid = AssetUtility.CreateImageChild(parent, "BlueMid" + (i * 3 + j * 1), new Vector2(x, y), Path.Combine(AssetFilePath, "Enso", "TopBg", "DonBgBlueMid.png"));
                    IEnumerator bgBlueMidCoroutine = TopBgBlueMidAnimation(blueMid, 64f);
                    Plugin.Instance.StartCustomCoroutine(bgBlueMidCoroutine);
                }
            }
        }

        static private void CreateTopAnimatedBlueFront(GameObject parent)
        {
            for (int i = 0; i < 2; i++)
            {
                var blueFrontSmall = AssetUtility.CreateImageChild(parent, "BlueFrontSmall" + i, new Vector2(1304 + (i * 1920), 59), Path.Combine(AssetFilePath, "Enso", "TopBg", "DonBgBlueFrontSmall.png"));
                var blueFrontBig = AssetUtility.CreateImageChild(parent, "BlueFrontBig" + i, new Vector2(201 + (i * 1920), 38), Path.Combine(AssetFilePath, "Enso", "TopBg", "DonBgBlueFrontBig.png"));
                IEnumerator bgBlueFrontSmallCoroutine = TopBgBlueFrontAnimation(blueFrontSmall, 76f);
                IEnumerator bgBlueFrontBigCoroutine = TopBgBlueFrontAnimation(blueFrontBig, 76f);
                Plugin.Instance.StartCustomCoroutine(bgBlueFrontSmallCoroutine);
                Plugin.Instance.StartCustomCoroutine(bgBlueFrontBigCoroutine);
            }
        }

        #endregion

        #region TopAnimation

        static private IEnumerator TopBgFlowersAnimation(GameObject gameObject, float speed)
        {
            while (gameObject != null)
            {
                if (EnsoPauseHook.IsPaused)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }
                var position = gameObject.transform.localPosition;
                position.x -= Time.deltaTime * speed;
                if (position.x < (1920 / -2))
                {
                    position.x += (1920 / 2) * 3;
                }
                gameObject.transform.localPosition = position;

                yield return new WaitForEndOfFrame();
            }
        }

        static private IEnumerator TopBgPetalAnimation(GameObject gameObject, float speed)
        {
            while (gameObject != null)
            {
                if (EnsoPauseHook.IsPaused)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }
                var position = gameObject.transform.localPosition;
                position.x -= Time.deltaTime * speed;
                position.y -= Time.deltaTime * (speed / 2);
                if (position.x < (1920 / -2))
                {
                    position.x += (1920 / 2) * 3;
                }
                if (position.y < -300)
                {
                    position.y += 800;
                }
                gameObject.transform.localPosition = position;

                yield return new WaitForEndOfFrame();
            }
        }

        static private IEnumerator TopBgBlueBackAnimation(GameObject gameObject, float speed)
        {
            while (gameObject != null)
            {
                if (EnsoPauseHook.IsPaused)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }
                var position = gameObject.transform.localPosition;
                position.x -= Time.deltaTime * speed;
                if (position.x < -1920)
                {
                    position.x += 1920 * 2;
                }
                gameObject.transform.localPosition = position;

                yield return new WaitForEndOfFrame();
            }
        }

        static private IEnumerator TopBgBlueMidAnimation(GameObject gameObject, float speed)
        {
            while (gameObject != null)
            {
                if (EnsoPauseHook.IsPaused)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }
                var position = gameObject.transform.localPosition;
                position.x -= Time.deltaTime * speed;
                if (position.x < -1920)
                {
                    position.x += 1920 * 2;
                }
                gameObject.transform.localPosition = position;

                yield return new WaitForEndOfFrame();
            }
        }

        static private IEnumerator TopBgBlueFrontAnimation(GameObject gameObject, float speed)
        {
            while (gameObject != null)
            {
                if (EnsoPauseHook.IsPaused)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }
                var position = gameObject.transform.localPosition;
                position.x -= Time.deltaTime * speed;
                if (position.x < -1920)
                {
                    position.x += 1920 * 2;
                }
                gameObject.transform.localPosition = position;

                yield return new WaitForEndOfFrame();
            }
        }


        static private IEnumerator TopBgAnimation(GameObject gameObject, float speed)
        {
            var image = gameObject.GetComponent<Image>();
            while (gameObject != null)
            {
                if (EnsoPauseHook.IsPaused)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }
                var position = gameObject.transform.localPosition;
                position.x -= Time.deltaTime * speed;
                if (position.x < 0 - image.mainTexture.width)
                {
                    position.x = 1920 + image.mainTexture.width;
                }
                gameObject.transform.localPosition = position;
                yield return new WaitForEndOfFrame();
            }
        }

        static private IEnumerator TopBgAnimationAndDown(GameObject gameObject, float speed)
        {
            var image = gameObject.GetComponent<Image>();
            while (gameObject != null)
            {
                if (EnsoPauseHook.IsPaused)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }
                var position = gameObject.transform.localPosition;
                position.x -= Time.deltaTime * speed;
                position.y -= Time.deltaTime * speed;
                if (position.x < 0 - image.mainTexture.width)
                {
                    position.x = 1920 + image.mainTexture.width;
                }
                gameObject.transform.localPosition = position;
                yield return new WaitForEndOfFrame();
            }
        }

        #endregion

        static public GameObject CreateBottomBg(GameObject parent)
        {
            return AssetUtility.CreateImageChild(parent, "DaniBottomBg", new Vector2(0, 0), Path.Combine(AssetFilePath, "Enso", "BottomBg.png"));
        }

        static public void CreateRequirementPanel(GameObject parent, DaniBorder border, int index)
        {

        }

        static public void CreateRequirementTypeText()
        {

        }



    }
}
