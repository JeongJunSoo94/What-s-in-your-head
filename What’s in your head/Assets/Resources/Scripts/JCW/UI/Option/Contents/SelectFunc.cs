using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JCW.UI.Options
{
    public class SelectFunc : FontColorShift
    {
        [Header("기능의 개수")] [SerializeField] private List<string> funcTexts;

        private Button leftButton;
        private Button rightButton;

        private Text func;

        private GameObject funcValueObj;
        private Slider slider;

        private int index = 0;

        private Image thisImg;

        private readonly List<Color> visInvis = new();

        override protected void Awake()
        {
            textName = gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
            Init();
            visInvis.Add(new Color(1, 1, 1, 1));
            visInvis.Add(new Color(1, 1, 1, 0));

            thisImg = this.gameObject.GetComponent<Image>();
            thisImg.color = visInvis[1];

            
            leftButton = gameObject.transform.GetChild(1).gameObject.GetComponent<Button>();

            rightButton = gameObject.transform.GetChild(3).gameObject.GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (funcTexts.Count > 1)
            {
                if (index == 0)
                {
                    leftButton.interactable = false;
                    rightButton.interactable = true;
                }
                else if (index == funcTexts.Count - 1)
                {
                    leftButton.interactable = true;
                    rightButton.interactable = false;
                }
            }


            if (funcTexts.Count != 0 && func != null)
            {
                for (int i = 0 ; i<funcTexts.Count ; ++i)
                {
                    if (func.text == funcTexts[i])
                    {
                        index = i;
                        return;
                    }
                }
            }
        }

        void Start()
        {
            // 슬라이더가 아닐 때
            if (funcTexts.Count != 0)
            {
                leftButton.interactable = false;
                func = gameObject.transform.GetChild(2).gameObject.GetComponent<Text>();
                Destroy(funcValueObj);
            }
            else
            {
                Destroy(func);
                funcValueObj = gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject;
                slider = funcValueObj.transform.parent.gameObject.GetComponent<Slider>();
            }

            // 왼쪽 버튼 입력 시
            leftButton.onClick.AddListener(() =>
            {
                if (funcTexts.Count == 0)
                {
                    slider.value -= 0.01f;
                    index = (int)(slider.value * 100);
                    if (index == 0)
                        leftButton.interactable = false;
                    if (!rightButton.IsInteractable())
                        rightButton.interactable = true;
                }
                else
                {
                    func.text = funcTexts[--index];
                    if (index == 0)
                        leftButton.interactable = false;
                    if (!rightButton.IsInteractable())
                        rightButton.interactable = true;
                }
            });

            // 오른쪽 버튼 입력 시
            rightButton.onClick.AddListener(() =>
            {
                if (funcTexts.Count == 0)
                {
                    slider.value += 0.01f;
                    index = (int)(slider.value * 100);
                    if (index == 100)
                        rightButton.interactable = false;
                    if (!leftButton.IsInteractable())
                        leftButton.interactable = true;
                }
                else
                {
                    func.text = funcTexts[++index];
                    if (index == funcTexts.Count - 1)
                        rightButton.interactable = false;
                    if (funcTexts.Count != 0 && !leftButton.IsInteractable())
                        leftButton.interactable = true;
                }
            });
        }

        override public void OnPointerEnter(PointerEventData eventData)
        {
            thisImg.color = visInvis[0];
            base.OnPointerEnter(eventData);
            if (funcTexts.Count != 0)
                func.color = BlackWhite[1];
            else
                funcValueObj.GetComponent<Text>().color = BlackWhite[1];

        }

        override public void OnPointerExit(PointerEventData eventData)
        {
            thisImg.color = visInvis[1];
            base.OnPointerExit(eventData);
            if (funcTexts.Count != 0)
                func.color = BlackWhite[0];
            else
                funcValueObj.GetComponent<Text>().color = BlackWhite[0];
        }
    }
}

