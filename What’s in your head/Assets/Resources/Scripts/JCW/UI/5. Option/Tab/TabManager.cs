using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.Options
{
    public class TabManager : MonoBehaviour
    {
        private readonly List<Button> tabButtons = new();
        public static TabManager Instance = null;

        private int curTabIndex = 0;

        private void Awake()
        {
            if (Instance == null)
                Instance = this; 
            for (int i = 0 ; i < transform.childCount ; ++i)
            {
                tabButtons.Add(this.transform.GetChild(i).gameObject.GetComponent<Button>());
            }
        }

        private void OnEnable() { Instance = this; curTabIndex = 0; ClickTab(tabButtons[curTabIndex]); }

        // 탭 버튼 누를 시 다른 탭으로 이동
        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Tab))
        //    {
        //        curTabIndex = curTabIndex == tabButtons.Count - 1 ? 0 : curTabIndex + 1;
        //        ClickTab(tabButtons[curTabIndex]);
        //    }
        //}

        public void ClickTab(Button _button)
        {
            for (int i = 0 ; i<tabButtons.Count ; ++i)
            {
                // 눌려진 탭을 꺼주기
                if (tabButtons[i] == _button)
                {
                    curTabIndex = i;
                    _button.interactable = false;
                    _button.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    continue;
                }
                // 그 외는 켜주고, 내용물도 숨기기
                tabButtons[i].interactable = true;
                tabButtons[i].gameObject.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
}

