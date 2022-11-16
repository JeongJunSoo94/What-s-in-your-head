using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JCW.Network;

namespace JCW.UI
{
    public class ExitManager : MonoBehaviour
    {
        [Header("�Ͻ����� Ÿ��Ʋ")] [SerializeField] GameObject titleObj;
        [Header("�Ͻ����� �޴�")] [SerializeField] GameObject menuObj;
        [Header("���� �޴�")] 
        [SerializeField] Button mainMenu;
        [SerializeField] Button exitMenu;
        [SerializeField] Button backMenu;

        private void Awake()
        {
            mainMenu.onClick.AddListener(() =>
            {
                GameManager.Instance.curStageIndex = 0;
                GameManager.Instance.curStageType = 1;
                LoadingUI.Instance.gameObject.SetActive(true);
                //PhotonManager.Instance.ChangeStage();
            });
            exitMenu.onClick.AddListener(() =>
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
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
