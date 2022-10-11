using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JCW.UI.Options
{    
    public class ImageShifter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("스프라이트 변경할 오브젝트")] [SerializeField] protected GameObject obj = null;
        [Header("기본 버튼 스프라이트")][SerializeField] protected Sprite defaultSprite = null;
        [Header("호버링 버튼 스프라이트")][SerializeField] protected Sprite onButtonSprite = null;

        protected Image thisImg = null;

        virtual protected void Awake()
        {
            thisImg = obj == null ? this.gameObject.GetComponent<Image>() : obj.GetComponent<Image>();
        }

        virtual public void OnPointerEnter(PointerEventData eventData)
        {
            thisImg.sprite = onButtonSprite;
        }

        virtual public void OnPointerExit(PointerEventData eventData)
        {
            thisImg.sprite = defaultSprite;
        }

        virtual public void SetDefaultSprite()
        {
            thisImg.sprite = defaultSprite;
        }
    }
}

