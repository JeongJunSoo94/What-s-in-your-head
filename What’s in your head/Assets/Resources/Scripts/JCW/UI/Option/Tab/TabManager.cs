using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JCW.UI.Options
{
    public class TabManager : MonoBehaviour
    {
        private readonly List<Button> tabButtons = new();
        public static TabManager Instance = null;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(this.gameObject);
        }

        private void Start()
        {
            for (int i = 0 ; i < transform.childCount ; ++i)
            {
                tabButtons.Add(this.transform.GetChild(i).gameObject.GetComponent<Button>());
            }
        }

        public void ClickTab(Button _button)
        {
            for (int i = 0 ; i<tabButtons.Count ; ++i)
            {
                // ������ ���� ���ֱ�
                if (tabButtons[i] == _button)
                {
                    _button.interactable = false;
                    continue;
                }
                // �� �ܴ� ���ְ�, ���빰�� �����
                tabButtons[i].interactable = true;
                tabButtons[i].gameObject.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
}

