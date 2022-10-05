using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace JCW.Options
{
    public class InitializeSettings : MonoBehaviour
    {

        [Header("���� ��ư")][SerializeField] GameObject applyObj = null;

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
                Debug.Log("�ɼ� �ʱ� ���ð� �ҷ����� ����. �ʱ� ������ ���� �� �ҷ����� ����");
                applyObj.SendMessage("SaveToFile", true);                
            }
            applyObj.SendMessage("LoadFromFile", true);
        }
    }
}
