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
                Debug.Log("초기화 버튼이 할당되지 않았습니다 !!");
                return;
            }
            GetComponent<Button>().onClick.AddListener(() =>
            {
                initWarning.SetActive(true);
            });
        }
    }
}
