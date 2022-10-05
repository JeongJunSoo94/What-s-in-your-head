using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JCW.Options
{
    public class FontColorShift : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected readonly List<Color> BlackWhite = new();
        protected List<Color> outLineInv;
        [SerializeField] protected Text textName = null;

        protected Outline outline = null;

        virtual protected void Awake()
        {
            textName = this.gameObject.GetComponent<Text>();            
            Init();
        }

        protected void Init()
        {
            outline = this.gameObject.GetComponent<Outline>();
            if (outline != null)
            {
                outLineInv = new();
                Color invColor = outline.effectColor == Color.black ? Color.white : Color.black;
                outLineInv.Add(outline.effectColor);
                outLineInv.Add(invColor);
            }

            Color invertedColor = textName.color==Color.black ? Color.white : Color.black;        
            BlackWhite.Add(textName.color);
            BlackWhite.Add(invertedColor);
        }
        virtual public void OnPointerEnter(PointerEventData eventData)
        {
            textName.color = BlackWhite[1];
            if (outline != null)
                outline.effectColor = outLineInv[1];
        }

        virtual public void OnPointerExit(PointerEventData eventData)
        {
            textName.color = BlackWhite[0];
            if (outline != null)
                outline.effectColor = outLineInv[0];
        }

    }
}

