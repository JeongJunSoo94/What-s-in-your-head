using JCW.AudioCtrl;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.Options
{
    public class SelectFunc : FontColorShift
    {
        protected enum FuncChild { Name, Left, Func, Right };

        [Header("����� ����")] [SerializeField] private List<string> funcTexts;
        [Header("�����̴� �ּڰ� / �ִ�")] [SerializeField] [Range(0, 100)] private int min = 0;
                                        [SerializeField] [Range(0, 100)] private int max = 100;

        // �¿��ư �� UI�� �������� ��ɰ�
        private Button leftButton;
        private Button rightButton;
        private GameObject funcValue;
        
        // �����̴� �ٸ� ���� ��
        private bool isSlider;

        // ��ư�� ���� ���� ���ϴ� �ٽ� ���
        private Text functionName;
        private Slider slider;

        //private Text sliderValueOnUI;
        private InputField sliderValueOnUI;
        private Text sliderValueText;

        // ��ư�� ���� ��ġ
        private int index = 0;

        private Image hoveringImg;

        override protected void Awake()
        {
            textName = transform.GetChild((int)FuncChild.Name).GetComponent<Text>();
            leftButton = transform.GetChild((int)FuncChild.Left).GetComponent<Button>();
            rightButton = transform.GetChild((int)FuncChild.Right).GetComponent<Button>();
            funcValue = transform.GetChild((int)FuncChild.Func).gameObject;

            isSlider = funcTexts.Count==0;

            hoveringImg = GetComponent<Image>();
            if (isSlider)
            {
                slider = funcValue.GetComponent<Slider>();
                slider.minValue = min;
                slider.maxValue = max;
                //sliderValueOnUI = slider.transform.GetChild(0).GetComponentInChildren<Text>();
                sliderValueOnUI = slider.transform.GetChild(0).GetComponent<InputField>();
                sliderValueText = sliderValueOnUI.GetComponentInChildren<Text>();
                sliderValueOnUI.onEndEdit.AddListener((string _text) =>
                {
                    int tempValue = int.Parse(_text);
                    if (tempValue > 100)
                        tempValue = 100;
                    else if (tempValue < 0)
                        tempValue = 0;
                    slider.value = tempValue;
                });
            }
            else
                functionName = funcValue.GetComponent<Text>();
        }

        private void OnEnable()
        {
            if (!isSlider)
            {
                index = funcTexts.FindIndex(func => func.Contains(functionName.text));
                if (index == -1)
                    index = 0;
            }
        }

        void Start()
        {
            // �����̴��� �ƴ� ��
            if (!isSlider) { leftButton.interactable = false; }

            // ���� ��ư �Է� ��
            leftButton.onClick.AddListener(() =>
            {
                if (isSlider)
                {
                    slider.value -= 1;
                    index = (int)(slider.value);
                    sliderValueOnUI.text = index.ToString();
                }
                else { functionName.text = funcTexts[--index]; }

                if (index == 0)                 leftButton.interactable = false;
                if (!rightButton.interactable)  rightButton.interactable = true;
            });

            // ������ ��ư �Է� ��
            rightButton.onClick.AddListener(() =>
            {
                if (isSlider)
                {
                    slider.value += 1;
                    index = (int)(slider.value);
                    sliderValueOnUI.text = index.ToString();
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


        private void Update()
        {
            if (isSlider && index != (int)slider.value)
            {
                index = (int)slider.value;
                sliderValueOnUI.text = index.ToString();
            }
        }

        override public void InvertFont(bool isDefault = true)
        {
            base.InvertFont(isDefault);
            hoveringImg.color = GetInvertVisible(hoveringImg.color, isDefault);            
            if (!isSlider)
                functionName.color = textName.color;
            else
                sliderValueText.color = textName.color;
        }
    }
}

