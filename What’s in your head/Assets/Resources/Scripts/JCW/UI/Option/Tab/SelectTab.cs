using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JCW.UI.Options
{
    public class SelectTab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private GameObject _contents = null;
        private GameObject _textObj = null;
        private Text _text = null;
        private Button _button = null;

        private int textInitSize = 0;

        private void Awake()
        {
            _button = this.gameObject.GetComponent<Button>();
            _textObj = this.gameObject.transform.GetChild(0).gameObject;
            _text = _textObj.GetComponent<Text>();
            _contents = this.gameObject.transform.GetChild(1).gameObject;
        }
        void Start()
        {
            textInitSize = _text.fontSize;
            _button.onClick.AddListener(() =>
            {
                TabManager.Instance.ClickTab(_button);
                _contents.SetActive(true);
            });
        }

        // 마우스 포인터가 닿았을 때
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerEnter == _textObj)
                _text.fontSize = (int)(textInitSize * 1.2f);
        }
        // 마우스 포인터가 떨어졌을 때
        public void OnPointerExit(PointerEventData eventData)
        {
            _text.fontSize = textInitSize;
        }
    }
}

