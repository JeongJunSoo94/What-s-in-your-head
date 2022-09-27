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
                "Crouch" => "�������, ���̱�",
                "Swing" => "���� / �۶��ε� �׷���",
                "ToggleRun" => "���� ��ȯ",
                "Fire" => "�⺻ �ɷ� / �߻�",
                "Aim" => "���� �ɷ� / ����",
                "Interaction" => "��ȣ �ۿ�",
                "Cancle" => "���",
                "FindPartner" => "�ٸ� �÷��̾� ã��",
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
                "Comma" => ",",
                "Period" => ".",
                "Slash" => "/",
                "Backslash" => @"\",
                "Plus" => "+",
                "Minus" => "-",
                "BackQuote" => "`",
                "Colon" => "'",
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
                "Mouse0" => "���콺 ���� ��ư",
                "Mouse1" => "���콺 ������ ��ư",
                "Mouse2" => "���콺 �߾� ��ư",
                "Escape" => "Esc",
                "LeftArrow" => "��",
                "RightArrow" => "��",
                "UpArrow" => "��",
                "DownArrow" => "��",
                _ => codeText,
            };
            return convertText;
        }
    }
}
