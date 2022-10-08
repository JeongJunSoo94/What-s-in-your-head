using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JCW.UI.Options
{
    public class SelectTab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private GameObject contents = null;
        private GameObject textObj = null;
        private Text text = null;
        private Button button = null;

        private int textInitSize = 0;

        private void Awake()
        {
            button = this.gameObject.GetComponent<Button>();
            textObj = this.gameObject.transform.GetChild(0).gameObject;
            text = textObj.GetComponent<Text>();
            contents = this.gameObject.transform.GetChild(1).gameObject;
        }
        void Start()
        {
            textInitSize = text.fontSize;
            button.onClick.AddListener(() =>
            {
                TabManager.Instance.ClickTab(button);
                contents.SetActive(true);
            });
        }

        // 마우스 포인터가 닿았을 때
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerEnter == textObj)
                text.fontSize = (int)(textInitSize * 1.2f);
        }
        // 마우스 포인터가 떨어졌을 때
        public void OnPointerExit(PointerEventData eventData)
        {
            text.fontSize = textInitSize;
        }
    }
}

