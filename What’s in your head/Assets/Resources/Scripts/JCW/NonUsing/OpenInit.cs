using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace JCW.UI.Options
{
    public class OpenInit : MonoBehaviour
    {
        [SerializeField] private GameObject initWarning = null;

        private void Awake()
        {
            if (initWarning == null)
            {
                Debug.Log("�ʱ�ȭ ��ư�� �Ҵ���� �ʾҽ��ϴ� !!");
                return;
            }
            GetComponent<Button>().onClick.AddListener(() =>
            {
                initWarning.SetActive(true);
            });
        }
    }
}
