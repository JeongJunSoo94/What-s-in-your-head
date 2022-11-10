using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStage : MonoBehaviour
{
    [Header("���� ��ư")] [SerializeField] private Button leftButton = null;
    [Header("���� ��ư")] [SerializeField] private Button rightButton = null;
    [Header("�������� ���� ����Ʈ")] [SerializeField] private List<string> stages = new();
    [Header("���� �������� ����")] [SerializeField] private Text curStage;
    [Header("���� ���� ����Ʈ")] [SerializeField] private GameObject sections;

    // �ҷ��� �� �ִ� �������� ���
    int stageCount = 0;
    int latestStageType = 0;

    private void Awake()
    {
        GameManager.Instance.curStageIndex = 1;
        GameManager.Instance.curStageType = 1;
        GameManager.Instance.curSection = 0;
        string path = Application.dataPath + "/Resources/CheckPointInfo/";
        stageCount = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly).Length;
        latestStageType = Directory.GetDirectories(path + "/Stage/" + stageCount, "*", SearchOption.TopDirectoryOnly).Length; 

        leftButton.onClick.AddListener(() =>
        {
            GameManager.Instance.curStageType = 1;
            if (GameManager.Instance.curStageIndex != 0)
            {
                curStage.text = stages[--GameManager.Instance.curStageIndex];
                for (int i=0 ; i<2 ; ++i)
                {
                    sections.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = curStage.text + " " + (i + 1).ToString();
                }
                rightButton.interactable = true;
            }
            if (GameManager.Instance.curStageIndex == 0)
                leftButton.interactable = false;

        });
        rightButton.onClick.AddListener(() =>
        {
            GameManager.Instance.curStageType = 1;
            if (GameManager.Instance.curStageIndex < stageCount)
            {
                curStage.text = stages[++GameManager.Instance.curStageIndex];
                for (int i = 0 ; i < 2 ; ++i)
                {
                    sections.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = curStage.text + " " + (i + 1).ToString();
                }
                leftButton.interactable = true;
            }
            if (GameManager.Instance.curStageIndex == stageCount)
            {
                rightButton.interactable = false;
                sections.transform.GetChild(1).GetComponent<Button>().interactable = latestStageType == 2;
            }
        });
    }
}
