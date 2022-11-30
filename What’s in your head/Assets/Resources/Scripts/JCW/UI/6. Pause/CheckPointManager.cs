using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JCW.Network;
using KSU;
using Photon.Pun;

namespace JCW.UI
{
    public class CheckPointManager : MonoBehaviour
    {
        [Header("�Ͻ����� Ÿ��Ʋ")] [SerializeField] GameObject titleObj;
        [Header("�Ͻ����� �޴�")] [SerializeField] GameObject menuObj;
        [Header("üũ����Ʈ �޴�")]
        [SerializeField] Button loadMenu;
        [SerializeField] Button backMenu;
        private void Awake()
        {
            loadMenu.onClick.AddListener(() =>
            {
                if (GameManager.Instance.curSection > 1)
                    GameManager.Instance.LoadCheckPoint();
                this.gameObject.SetActive(false);
                titleObj.transform.parent.gameObject.SetActive(false);
            });
            backMenu.onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });
        }
        private void OnEnable()
        {
            titleObj.SetActive(false);
            menuObj.SetActive(false);
        }
        private void OnDisable()
        {
            titleObj.SetActive(true);
            menuObj.SetActive(true);
        }
    }
}
