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
            string convertText = "";
            switch(actionText)
            {
                case "MoveForward":
                    convertText = "������ �̵�";
                    break;
                case "MoveBackward":
                    convertText = "�ڷ� �̵�";
                    break;
                case "MoveLeft":
                    convertText = "�������� �̵�";
                    break;
                case "MoveRight":
                    convertText = "���������� �̵�";
                    break;

                case "Jump":
                    convertText = "����";
                    break;
                case "Dash":
                    convertText = "���";
                    break;
                case "Crouch":
                    convertText = "�������, ���̱�";
                    break;
                case "Swing":
                    convertText = "���� / �۶��ε� �׷���";
                    break;
                case "ToggleRun":
                    convertText = "���� ��ȯ";
                    break;

                case "Fire":
                    convertText = "�⺻ �ɷ� / �߻�";
                    break;
                case "Aim":
                    convertText = "���� �ɷ� / ����";
                    break;

                case "Interaction":
                    convertText = "��ȣ �ۿ�";
                    break;
                case "Cancle":
                    convertText = "���";
                    break;
                case "FindPartner":
                    convertText = "�ٸ� �÷��̾� ã��";
                    break;
                case "Pause":
                    convertText = "�Ͻ����� / �ɼ�";
                    break;

                default:
                    convertText = actionText;
                    break;
            }
            return convertText;
        }
        private string ConvertCodeLabel(string codeText)
        {
            string convertText = "";
            switch (codeText)
            {
                case "Return":
                    convertText = "Enter";
                    break;
                case "Comma":
                    convertText = ",";
                    break;
                case "Period":
                    convertText = ".";
                    break;
                case "Slash":
                    convertText = "/";
                    break;
                case "Backslash":
                    convertText = @"\";
                    break;
                case "Plus":
                    convertText = "+";
                    break;
                case "Minus":
                    convertText = "-";
                    break;
                case "BackQuote":
                    convertText = "`";
                    break;
                case "Colon":
                    convertText = "'";
                    break;
                case "Semicolon":
                    convertText = ";";
                    break;
                case "LeftBracket":
                    convertText = "[";
                    break;
                case "RightBracket":
                    convertText = "]";
                    break;
                case "Equals":
                    convertText = "=";
                    break;
                case "Alpha0":
                    convertText = "0";
                    break;
                case "Alpha1":
                    convertText = "1";
                    break;
                case "Alpha2":
                    convertText = "2";
                    break;
                case "Alpha3":
                    convertText = "3";
                    break;
                case "Alpha4":
                    convertText = "4";
                    break;
                case "Alpha5":
                    convertText = "5";
                    break;
                case "Alpha6":
                    convertText = "6";
                    break;
                case "Alpha7":
                    convertText = "7";
                    break;
                case "Alpha8":
                    convertText = "8";
                    break;
                case "Alpha9":
                    convertText = "9";
                    break;
                case "LeftShift":
                    convertText = "���� Shift";
                    break;
                case "RightShift":
                    convertText = "������ Shift";
                    break;
                case "LeftControl":
                    convertText = "���� Ctrl";
                    break;
                case "RightControl":
                    convertText = "������ Ctrl";
                    break;
                case "LeftAlt":
                    convertText = "���� ALT";
                    break;
                case "RightAlt":
                    convertText = "������ ALT";
                    break;
                case "Mouse0":
                    convertText = "���콺 ���� ��ư";
                    break;
                case "Mouse1":
                    convertText = "���콺 ������ ��ư";
                    break;
                case "Mouse2":
                    convertText = "���콺 �߾� ��ư";
                    break;
                case "Escape":
                    convertText = "Esc";
                    break;
                case "LeftArrow":
                    convertText = "��";
                    break;
                case "RightArrow":
                    convertText = "��";
                    break;
                case "UpArrow":
                    convertText = "��";
                    break;
                case "DownArrow":
                    convertText = "��";
                    break;
                default:
                    convertText = codeText;
                    break;
            }
            return convertText;
        }
    }
}
