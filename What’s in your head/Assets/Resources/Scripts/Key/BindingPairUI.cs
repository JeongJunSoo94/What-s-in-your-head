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
                    convertText = "앞으로 이동";
                    break;
                case "MoveBackward":
                    convertText = "뒤로 이동";
                    break;
                case "MoveLeft":
                    convertText = "왼쪽으로 이동";
                    break;
                case "MoveRight":
                    convertText = "오른쪽으로 이동";
                    break;

                case "Jump":
                    convertText = "점프";
                    break;
                case "Dash":
                    convertText = "대시";
                    break;
                case "Crouch":
                    convertText = "내려찍기, 숙이기";
                    break;
                case "Swing":
                    convertText = "스윙 / 글라인드 그래플";
                    break;
                case "ToggleRun":
                    convertText = "질주 전환";
                    break;

                case "Fire":
                    convertText = "기본 능력 / 발사";
                    break;
                case "Aim":
                    convertText = "보조 능력 / 조준";
                    break;

                case "Interaction":
                    convertText = "상호 작용";
                    break;
                case "Cancle":
                    convertText = "취소";
                    break;
                case "FindPartner":
                    convertText = "다른 플레이어 찾기";
                    break;
                case "Pause":
                    convertText = "일시정지 / 옵션";
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
                    convertText = "왼쪽 Shift";
                    break;
                case "RightShift":
                    convertText = "오른쪽 Shift";
                    break;
                case "LeftControl":
                    convertText = "왼쪽 Ctrl";
                    break;
                case "RightControl":
                    convertText = "오른쪽 Ctrl";
                    break;
                case "LeftAlt":
                    convertText = "왼쪽 ALT";
                    break;
                case "RightAlt":
                    convertText = "오른쪽 ALT";
                    break;
                case "Mouse0":
                    convertText = "마우스 왼쪽 버튼";
                    break;
                case "Mouse1":
                    convertText = "마우스 오른쪽 버튼";
                    break;
                case "Mouse2":
                    convertText = "마우스 중앙 버튼";
                    break;
                case "Escape":
                    convertText = "Esc";
                    break;
                case "LeftArrow":
                    convertText = "←";
                    break;
                case "RightArrow":
                    convertText = "→";
                    break;
                case "UpArrow":
                    convertText = "↑";
                    break;
                case "DownArrow":
                    convertText = "↓";
                    break;
                default:
                    convertText = codeText;
                    break;
            }
            return convertText;
        }
    }
}
