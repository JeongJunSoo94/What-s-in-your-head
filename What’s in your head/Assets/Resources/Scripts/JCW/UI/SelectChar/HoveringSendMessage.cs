using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JCW.UI
{
    public class HoveringSendMessage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("넬라의 경우 true로 설정")] [SerializeField] private bool isNella = false;
        private CharShifter UI_Script = null;

        private void Awake()
        {
            UI_Script = this.gameObject.transform.parent.gameObject.GetComponent<CharShifter>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UI_Script.ChangeSprite(isNella, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UI_Script.ChangeSprite(isNella, false);
        }


    }
}
