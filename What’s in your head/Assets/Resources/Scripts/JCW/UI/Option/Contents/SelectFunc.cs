using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectFunc : MonoBehaviour
{
    [Header("����� ����")] [SerializeField] private List<string> funcTexts;

    private Button leftButton;
    private Button rightButton;
    private Text func;
    private int index = 0;

    void Start()
    {
        if (funcTexts.Count == 0)
        {
            Debug.Log("��� ������ �����ϴ�. �Է����ּ��� !");
            return;
        }

        leftButton = gameObject.transform.GetChild(0).gameObject.GetComponent<Button>();
        func = gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
        rightButton = gameObject.transform.GetChild(2).gameObject.GetComponent<Button>();

        leftButton.interactable = false;
        func.text = funcTexts[0];


        // ���� ��ư �Է� ��
        leftButton.onClick.AddListener(() =>
        {
            func.text = funcTexts[--index];
            if (index == 0)
                leftButton.interactable = false;
            if (!rightButton.IsInteractable())
                rightButton.interactable = true;
        });

        // ������ ��ư �Է� ��
        rightButton.onClick.AddListener(() =>
        {
            func.text = funcTexts[++index];
            if (index == funcTexts.Count - 1)
                rightButton.interactable = false;
            if (!leftButton.IsInteractable())
                leftButton.interactable = true;
        });
    }
}
