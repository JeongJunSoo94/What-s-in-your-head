using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JCW.UI.Options
{
    public class FontColorShift : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected Text textName = null;
        protected Outline outline = null;

        virtual protected void Awake()
        {
            textName = this.gameObject.GetComponent<Text>();
            outline = this.gameObject.GetComponent<Outline>();
        }

        virtual public void OnPointerEnter(PointerEventData eventData)
        {
            InvertFont();
        }

        virtual public void OnPointerExit(PointerEventData eventData)
        {
            InvertFont();
        }

        virtual protected void InvertFont()
        {
            textName.color = GetInvertColor(textName.color);
            if (outline != null)
                outline.effectColor = GetInvertColor(outline.effectColor);
        }

        protected void SetVisibleInvert(Image img)
        {
            Color InvertVisColor = new(img.color.r, img.color.g, img.color.b, 1 - img.color.a);
            img.color = InvertVisColor;
        }

        protected Color GetInvertColor(Color originColor)
        {
            Color InvertColor = new(1 - originColor.r, 1 - originColor.g, 1 - originColor.b, originColor.a);
            return InvertColor;
        }

    }
}

