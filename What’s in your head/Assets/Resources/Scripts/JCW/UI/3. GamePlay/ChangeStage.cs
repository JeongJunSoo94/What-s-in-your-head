using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStage : MonoBehaviour
{
    [Header("���� ��ư")] [SerializeField] private Button leftButton = null;
    [Header("���� ��ư")] [SerializeField] private Button rightButton = null;
    [Header("�������� ���� ����Ʈ")] [SerializeField] private List<string> stages = new();
    [Header("���� �������� ����")] [SerializeField] private Text curStage;
    [Header("���� ���� ����Ʈ")] [SerializeField] private GameObject sections;

    private void Awake()
    {
        leftButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.curStageIndex != 0)
            {
                curStage.text = stages[--GameManager.Instance.curStageIndex];
                for (int i=0 ; i<3 ; ++i)
                {
                    sections.transform.GetChild(i).gameObject.
                        transform.GetChild(0).gameObject.GetComponent<Text>().text = curStage.text + " " + (i + 1).ToString();
                }
                rightButton.interactable = true;
            }
            if (GameManager.Instance.curStageIndex == 0)
                leftButton.interactable = false;

        });
        rightButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.curStageIndex != stages.Count-1)
            {
                curStage.text = stages[++GameManager.Instance.curStageIndex];
                for (int i = 0 ; i < 3 ; ++i)
                {
                    sections.transform.GetChild(i).gameObject.
                        transform.GetChild(0).gameObject.GetComponent<Text>().text = curStage.text + " " + (i + 1).ToString();
                }
                leftButton.interactable = true;
            }
            if (GameManager.Instance.curStageIndex == stages.Count - 1)
                rightButton.interactable = false;
        });
    }
}
