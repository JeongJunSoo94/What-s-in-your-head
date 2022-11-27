using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.InGame
{
    public class TutorialUI : MonoBehaviour
    {
        [Header("커지는 속도")] [SerializeField] float increasingSpeed = 12f;
        public static TutorialUI Instance = null;
        Image nellaImg;
        Image steadyImg;
        RectTransform nellaRT;
        RectTransform steadyRT;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
            nellaImg    = transform.GetChild(0).GetComponent<Image>();
            nellaRT     = nellaImg.GetComponent<RectTransform>();
            steadyImg   = transform.GetChild(1).GetComponent<Image>();
            steadyRT    = steadyImg.GetComponent<RectTransform>();
        }

        public void ChangeSprite(bool isNella, Sprite sprite)
        {
            if(isNella)
            {
                nellaImg.sprite = sprite;
                nellaRT.sizeDelta = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y);
            }
            else
            {
                steadyImg.sprite = sprite;
                steadyRT.sizeDelta = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y);
            }
        }

        public void SetImage(bool isNella, bool isOn)
        {
            StopAllCoroutines();
            if(isOn)
            {
                nellaImg.enabled = false;
                steadyImg.enabled = false;
            }            
            StartCoroutine(SetSize(isNella, isOn));
        }


        IEnumerator SetSize(bool isNella, bool isOn)
        {
            if (isOn)
            {
                if (isNella)
                {
                    nellaImg.enabled = true;
                    while (nellaRT.localScale.x < 1f)
                    {
                        nellaRT.localScale = Vector3.MoveTowards(nellaRT.localScale, Vector3.one, increasingSpeed * Time.deltaTime);
                        if (nellaRT.localScale.x > 1f)
                            nellaRT.localScale = Vector3.one;
                        yield return null;
                    }
                }
                else
                {
                    steadyImg.enabled = true;
                    while (steadyRT.localScale.x < 1f)
                    {
                        steadyRT.localScale = Vector3.MoveTowards(steadyRT.localScale, Vector3.one, increasingSpeed * Time.deltaTime);
                        if (steadyRT.localScale.x > 1f)
                            steadyRT.localScale = Vector3.one;
                        yield return null;
                    }
                }                
            }
            else
            {
                if (isNella)
                {
                    while (nellaRT.localScale.x > 0f)
                    {
                        nellaRT.localScale = Vector3.MoveTowards(nellaRT.localScale, Vector3.zero, increasingSpeed * Time.deltaTime);
                        if (nellaRT.localScale.x < 0f)
                            nellaRT.localScale = Vector3.zero;
                        yield return null;
                    }
                    nellaImg.enabled = false;
                }
                else
                {
                    while (steadyRT.localScale.x > 0f)
                    {
                        steadyRT.localScale = Vector3.MoveTowards(steadyRT.localScale, Vector3.zero, increasingSpeed * Time.deltaTime);
                        if (steadyRT.localScale.x < 0f)
                            steadyRT.localScale = Vector3.zero;
                        yield return null;
                    }
                    steadyImg.enabled = false;
                }
            }

            yield break;
        }
    }

}
