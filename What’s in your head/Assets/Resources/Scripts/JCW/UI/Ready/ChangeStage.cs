using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStage : MonoBehaviour
{
    [Header("좌측 버튼")] [SerializeField] private Button leftButton = null;
    [Header("우측 버튼")] [SerializeField] private Button rightButton = null;
    [Header("스테이지 제목 리스트")] [SerializeField] private List<string> stages = new();
    [Header("현재 스테이지 제목")] [SerializeField] private Text curStage;
    [Header("섹션 제목 리스트")] [SerializeField] private GameObject sections;

    private int stageIndex = 0;

    private void Awake()
    {
        leftButton.onClick.AddListener(() =>
        {
            if (stageIndex != 0)
            {
                curStage.text = stages[--stageIndex];
                for (int i=0 ; i<3 ; ++i)
                {
                    sections.transform.GetChild(i).gameObject.
                    transform.GetChild(0).gameObject.GetComponent<Text>().text = curStage.text + " " + (i + 1).ToString();
                }
                rightButton.interactable = true;
            }
            if (stageIndex == 0)
                leftButton.interactable = false;

        });
        rightButton.onClick.AddListener(() =>
        {
            if (stageIndex != stages.Count-1)
            {
                curStage.text = stages[++stageIndex];
                for (int i = 0 ; i < 3 ; ++i)
                {
                    sections.transform.GetChild(i).gameObject.
                    transform.GetChild(0).gameObject.GetComponent<Text>().text = curStage.text + " " + (i + 1).ToString();
                }
                leftButton.interactable = true;
            }
            if (stageIndex == stages.Count - 1)
                rightButton.interactable = false;
        });
    }


    void Update()
    {
        
    }
}
