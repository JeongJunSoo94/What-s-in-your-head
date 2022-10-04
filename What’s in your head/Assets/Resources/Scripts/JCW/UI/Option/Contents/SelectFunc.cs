using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectFunc : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("기능의 개수")] [SerializeField] private List<string> funcTexts;

    private Button leftButton;
    private Button rightButton;

    private Text func;

    private GameObject funcValueObj;
    private Slider slider;


    private Text titleName;

    private int index = 0;

    private Image thisImg;

    private readonly List<Color> BlackWhite = new();
    private readonly List<Color> visInvis = new();

    private void Awake()
    {
        BlackWhite.Add(new Color(0, 0, 0, 1));
        BlackWhite.Add(new Color(1, 1, 1, 1));
        visInvis.Add(new Color(1, 1, 1, 1));
        visInvis.Add(new Color(1, 1, 1, 0));
    }

    private void OnEnable()
    {
        if (funcTexts.Count != 0 && func != null)
        {
            for (int i=0 ; i<funcTexts.Count ; ++i)
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
        thisImg = this.gameObject.GetComponent<Image>();
        thisImg.color = visInvis[1];

        titleName = gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        leftButton = gameObject.transform.GetChild(1).gameObject.GetComponent<Button>();
        
        rightButton = gameObject.transform.GetChild(3).gameObject.GetComponent<Button>();

        
        // 슬라이더가 아닐 때
        if (funcTexts.Count != 0)
        {
            leftButton.interactable = false;
            func = gameObject.transform.GetChild(2).gameObject.GetComponent<Text>();
            //func.text = funcTexts[0];
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
            if(funcTexts.Count == 0)
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        thisImg.color = visInvis[0];
        titleName.color = BlackWhite[1];
        if (funcTexts.Count != 0)
            func.color = BlackWhite[1];
        else
            funcValueObj.GetComponent<Text>().color = BlackWhite[1];

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        thisImg.color = visInvis[1];
        titleName.color = BlackWhite[0];
        if (funcTexts.Count != 0)
            func.color = BlackWhite[0];
        else
            funcValueObj.GetComponent<Text>().color = BlackWhite[0];
    }
}
