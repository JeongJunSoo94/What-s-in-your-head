using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace JCW.Options
{
    public class InitializeSettings : MonoBehaviour
    {

        [Header("적용 버튼")][SerializeField] GameObject applyObj = null;

        Button initializeButton = null;

        private void Awake()
        {
            
            initializeButton = this.gameObject.GetComponent<Button>();
            initializeButton.onClick.AddListener(() =>
            {
                InitializeValue();
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
