using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JCW.UI.Options
{    
    public class ButtonShifter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("기본 버튼 스프라이트")][SerializeField] private Sprite defaultSprite = null;
        [Header("호버링 버튼 스프라이트")][SerializeField] private Sprite onButtonSprite = null;

        private Image thisButtonImg = null;
            
        private void Awake()
        {
            thisButtonImg = this.gameObject.GetComponent<Image>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            thisButtonImg.sprite = onButtonSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            thisButtonImg.sprite = defaultSprite;
        }

        public void SetDefaultSprite()
        {
            thisButtonImg.sprite = defaultSprite;
        }
    }
}

