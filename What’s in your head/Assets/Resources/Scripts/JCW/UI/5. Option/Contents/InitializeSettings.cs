using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace JCW.UI.Options
{
    public class InitializeSettings : MonoBehaviour
    {

        [Header("���� ��ư")][SerializeField] GameObject applyObj = null;
        [Header("�ʱ�ȭ ��ư")][SerializeField] Button resetButton = null;
        [Header("�ڷΰ��� ��ư")][SerializeField] Button backButton = null;

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
                Debug.Log("�ɼ� �ʱ� ���ð� �ҷ����� ����. �ʱ� ������ ���� �� �ҷ����� ����");
                applyObj.SendMessage("SaveToFile", true);                
            }
            applyObj.SendMessage("LoadFromFile", true);
        }
    }
}
