using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace JCW.InputBindings
{
    public class BindingPairUI : MonoBehaviour
    {
        public bool selected = false;

        public Text actionLabel;
        public Text codeLabel;
        public Button codeButton;
        private Image buttonImage;

        private void Start()
        {
            buttonImage = codeButton.image;
        }

        public void Select()
        {
            selected = true;
            buttonImage.color = Color.green;
        }

        public void Deselect()
        {
            selected = false;
            buttonImage.color = Color.white;
        }

        public void InitLabels(string actionText, string codeText)
        {
            actionLabel.text = actionText;
            codeLabel.text = codeText;
        }

        public void SetCodeLabel(string text)
        {
            codeLabel.text = text;
        }

        public void AddButtonListener(UnityAction method)
        {
            codeButton.onClick.AddListener(method);
        }
    }
}
