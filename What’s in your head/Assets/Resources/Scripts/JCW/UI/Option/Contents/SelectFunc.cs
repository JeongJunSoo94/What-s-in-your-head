using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JCW.UI.Options
{    
    public class SelectFunc : FontColorShift
    {
        protected enum funcChild { Name, Left, Func, Right };

        [Header("기능의 개수")] [SerializeField] private List<string> funcTexts;
        [Header("슬라이더일 경우 기본 세팅값")] [SerializeField] private int sliderDefaultValue = 20;

        // 좌우버튼 및 UI에 보여지는 기능값
        private Button leftButton;
        private Button rightButton;
        private GameObject funcValue;
        
        // 슬라이더 바를 쓰는 지
        private bool isSlider;

        // 버튼에 의해 값이 변하는 핵심 기능
        private Text functionName;
        private Slider slider;

        private Text sliderValueOnUI;

        // 버튼의 현재 위치
        private int index = 0;

        private Image hoveringImg;

        override protected void Awake()
        {
            textName = gameObject.transform.GetChild((int)funcChild.Name).gameObject.GetComponent<Text>();
            leftButton = gameObject.transform.GetChild((int)funcChild.Left).gameObject.GetComponent<Button>();
            rightButton = gameObject.transform.GetChild((int)funcChild.Right).gameObject.GetComponent<Button>();
            funcValue = gameObject.transform.GetChild((int)funcChild.Func).gameObject;

            isSlider = funcTexts.Count==0 ? true : false;

            hoveringImg = this.gameObject.GetComponent<Image>();
            if (isSlider)
            {
                index = sliderDefaultValue;
                slider = funcValue.GetComponent<Slider>();
                sliderValueOnUI = slider.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
            }
            else
                functionName = funcValue.GetComponent<Text>();
        }

        // 아마 문제인듯..?

        private void OnEnable()
        {
            if (!isSlider)
            {
                index = funcTexts.FindIndex(func => func.Contains(functionName.text));
                if (index == -1)
                    index = 0;
            }
            else { slider.value = index/100f; }
        }

        void Start()
        {
            // 슬라이더가 아닐 때
            if (!isSlider) { leftButton.interactable = false; }

            // 왼쪽 버튼 입력 시
            leftButton.onClick.AddListener(() =>
            {
                if (isSlider)
                {
                    slider.value -= 0.01f;
                    index = (int)(slider.value * 100);
                }
                else { functionName.text = funcTexts[--index]; }

                if (index == 0)                 leftButton.interactable = false;
                if (!rightButton.interactable)  rightButton.interactable = true;
            });

            // 오른쪽 버튼 입력 시
            rightButton.onClick.AddListener(() =>
            {
                if (isSlider)
                {
                    slider.value += 0.01f;
                    index = (int)(slider.value * 100);
                }
                else { functionName.text = funcTexts[++index]; }

                if (index == 100 || index == funcTexts.Count - 1)
                    rightButton.interactable = false;
                                
                if (!leftButton.interactable)
                    leftButton.interactable = true;

            });
        }
        private void FixedUpdate()
        {
            leftButton.interactable = index != 0;
            rightButton.interactable = index != funcTexts.Count - 1 && index != 100;            
        }


        // 값이 자꾸 1 낮아지는 현상이 있음.
        private void Update()
        {
            if (isSlider)
                index = (int)(100* slider.value);
        }

        override protected void InvertFont()
        {
            base.InvertFont();
            SetVisibleInvert(hoveringImg);
            if (!isSlider)
                functionName.color = GetInvertColor(functionName.color);
            else
                sliderValueOnUI.color = GetInvertColor(sliderValueOnUI.color);
        }
    }
}

