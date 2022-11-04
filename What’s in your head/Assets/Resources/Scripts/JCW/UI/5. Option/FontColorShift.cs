using JCW.AudioCtrl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JCW.UI.Options
{
    public class FontColorShift : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("시작 컬러")] [SerializeField] protected Color defaultColor = new(0,0,0,1);
        protected Text textName = null;
        protected Outline outline = null;

        virtual protected void Awake()
        {
            textName = GetComponent<Text>();
            outline = GetComponent<Outline>();
            this.enabled = false;
        }

        virtual public void OnPointerEnter(PointerEventData eventData)
        {
            SoundManager.Instance.PlayUI("hoveringtest");
            InvertFont();
        }

        virtual public void OnPointerExit(PointerEventData eventData)
        {
            InvertFont(false);
        }

        virtual public void InvertFont(bool isDefault = true)
        {
            if(isDefault)
            {
                textName.color = GetInvertColor(textName.color); 
                if (outline != null)
                    outline.enabled = false;
            }
            else
            {
                textName.color = defaultColor;
                if (outline != null)
                    outline.enabled = true;
            }
        }

        protected Color GetInvertVisible(Color color, bool isDefault = true)
        {
            if (isDefault)
                return new(color.r, color.g, color.b, 1);
            return new(color.r, color.g, color.b, 1 - color.a); 
        }

        protected Color GetInvertColor(Color originColor)
        {
            return new(1 - originColor.r, 1 - originColor.g, 1 - originColor.b, originColor.a);
        }
    }
}

