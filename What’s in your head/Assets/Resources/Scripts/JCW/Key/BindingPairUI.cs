using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace JCW.InputBindings
{
    public class BindingPairUI : MonoBehaviour
    {
        public Text actionLabel;
        public Text codeLabel;
        public Button codeButton;

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
                "MoveForward" => "앞으로 이동",
                "MoveBackward" => "뒤로 이동",
                "MoveLeft" => "왼쪽으로 이동",
                "MoveRight" => "오른쪽으로 이동",
                "Jump" => "점프",
                "Dash" => "대시",
                "Swing" => "스윙 / 글라인드 그래플",
                "ToggleRun" => "질주 전환",
                "Fire" => "기본 능력 / 발사",
                "Aim" => "보조 능력 / 조준",
                "Interaction" => "상호 작용",
                "Cancle" => "취소",
                "FindPartner" => "다른 플레이어 찾기",
                "Chat" => "채팅창 열기 / 닫기",
                "Pause" => "일시정지 / 옵션",
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
                "LeftShift" => "왼쪽 Shift",
                "RightShift" => "오른쪽 Shift",
                "LeftControl" => "왼쪽 Ctrl",
                "RightControl" => "오른쪽 Ctrl",
                "LeftAlt" => "왼쪽 ALT",
                "RightAlt" => "오른쪽 ALT",
                "Mouse0" => "마우스 왼쪽 버튼",
                "Mouse1" => "마우스 오른쪽 버튼",
                "Mouse2" => "마우스 중앙 버튼",
                "Escape" => "Esc",
                "LeftArrow" => "←",
                "RightArrow" => "→",
                "UpArrow" => "↑",
                "DownArrow" => "↓",
                _ => codeText,
            };
            return convertText;
        }
    }
}
