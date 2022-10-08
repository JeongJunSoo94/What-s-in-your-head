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

        [Header("����� ����")] [SerializeField] private List<string> funcTexts;
        [Header("�����̴��� ��� �⺻ ���ð�")] [SerializeField] private int sliderDefaultValue = 20;

        // �¿��ư �� UI�� �������� ��ɰ�
        private Button leftButton;
        private Button rightButton;
        private GameObject funcValue;
        
        // �����̴� �ٸ� ���� ��
        private bool isSlider;

        // ��ư�� ���� ���� ���ϴ� �ٽ� ���
        private Text functionName;
        private Slider slider;

        private Text sliderValueOnUI;

        // ��ư�� ���� ��ġ
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

        // �Ƹ� �����ε�..?

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
            // �����̴��� �ƴ� ��
            if (!isSlider) { leftButton.interactable = false; }

            // ���� ��ư �Է� ��
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

            // ������ ��ư �Է� ��
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


        // ���� �ڲ� 1 �������� ������ ����.
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

