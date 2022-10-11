using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace JCW.UI.Options
{
    public class InitializeSettings : MonoBehaviour
    {

        [Header("적용 버튼")][SerializeField] GameObject applyObj = null;
        [Header("초기화 버튼")][SerializeField] Button resetButton = null;
        [Header("뒤로가기 버튼")][SerializeField] Button backButton = null;

        private void Awake()
        {
            resetButton.onClick.AddListener(() =>
            {
                InitializeValue();
                this.gameObject.SetActive(false);
            });

            backButton.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });
        }

        void InitializeValue()
        {
            if (!File.Exists(Application.dataPath + "/Resources/Options/OptionValuesInit.json"))
            {
                Debug.Log("옵션 초기 세팅값 불러오기 실패. 초기 데이터 저장 후 불러오기 실행");
                applyObj.SendMessage("SaveToFile", true);                
            }
            applyObj.SendMessage("LoadFromFile", true);
        }
    }
}
