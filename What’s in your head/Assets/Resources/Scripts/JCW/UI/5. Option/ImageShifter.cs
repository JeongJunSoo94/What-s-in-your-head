using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JCW.UI.Options
{    
    public class ImageShifter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("��������Ʈ ������ ������Ʈ")] [SerializeField] protected GameObject obj = null;
        [Header("�⺻ ��ư ��������Ʈ")][SerializeField] protected Sprite defaultSprite = null;
        [Header("ȣ���� ��ư ��������Ʈ")][SerializeField] protected Sprite onButtonSprite = null;

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

