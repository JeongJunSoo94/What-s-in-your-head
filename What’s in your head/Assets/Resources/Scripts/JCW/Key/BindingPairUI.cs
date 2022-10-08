using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace JCW.UI.Options.InputBindings
{
    public class BindingPairUI : FontColorShift
    {
        public Text actionLabel;
        public Text codeLabel;
        public Button codeButton;

        private Image highlight;

        override protected void Awake()
        {
            textName = actionLabel;
            highlight = gameObject.GetComponent<Image>();
        }

        public void InitLabels(string actionText, string codeText)
        {
            actionLabel.text = ConvertActionLabel(actionText);
            codeLabel.text = ConvertCodeLabel(codeText);
        }

        public void SetCodeLabel(string text)
        {
            codeLabel.text = ConvertCodeLabel(text);
        }

        public void AddButtonListener(UnityAction method)
        {
            codeButton.onClick.AddListener(method);
        }

        private string ConvertActionLabel(string actionText)
        {
            string convertText = actionText switch
            {
                "MoveForward" => "������ �̵�",
                "MoveBackward" => "�ڷ� �̵�",
                "MoveLeft" => "�������� �̵�",
                "MoveRight" => "���������� �̵�",
                "Jump" => "����",
                "Dash" => "���",
                "ToggleRun" => "�޸��� ��ȯ",
                "Fire" => "�ɷ� 1",
                "Aim" => "�ɷ� 2",
                "Interaction" => "��ȣ �ۿ�",
                "Chat" => "ä��â ���� / �ݱ�",
                "Pause" => "�Ͻ����� / �ɼ�",
                _ => actionText,
            };
            return convertText;
        }
        private string ConvertCodeLabel(string codeText)
        {
            string convertText = codeText switch
            {
                "None" => "",
                "Return" => "Enter",
                "CapsLock" => "Caps Lock",
                "Comma" => ",",
                "Period" => ".",
                "Slash" => "/",
                "Backslash" => @"\",
                "Plus" => "+",
                "Minus" => "-",
                "BackQuote" => "`",
                "Quote" => "'",
                "Semicolon" => ";",
                "LeftBracket" => "[",
                "RightBracket" => "]",
                "Equals" => "=",
                "Alpha0" => "0",
                "Alpha1" => "1",
                "Alpha2" => "2",
                "Alpha3" => "3",
                "Alpha4" => "4",
                "Alpha5" => "5",
                "Alpha6" => "6",
                "Alpha7" => "7",
                "Alpha8" => "8",
                "Alpha9" => "9",
                "LeftShift" => "���� Shift",
                "RightShift" => "������ Shift",
                "LeftControl" => "���� Ctrl",
                "RightControl" => "������ Ctrl",
                "LeftAlt" => "���� ALT",
                "RightAlt" => "������ ALT",
                "Mouse0" => "���콺 ��Ŭ��",
                "Mouse1" => "���콺 ��Ŭ��",
                "Mouse2" => "���콺 �� ��ư",
                "Escape" => "Esc",
                "LeftArrow" => "��",
                "RightArrow" => "��",
                "UpArrow" => "��",
                "DownArrow" => "��",
                _ => codeText,
            };
            return convertText;
        }

        override protected void InvertFont()
        {
            SetVisibleInvert(highlight);
            actionLabel.color = GetInvertColor(actionLabel.color);
        }
    }
}
