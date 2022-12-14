using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JCW.Network;
using Photon.Pun;

namespace JCW.UI
{    
    public class ExitManager : MonoBehaviour
    {
        [Header("일시정지 타이틀")] [SerializeField] GameObject titleObj;
        [Header("일시정지 메뉴")] [SerializeField] GameObject menuObj;
        [Header("종료 메뉴")] 
        [SerializeField] Button mainMenu;
        [SerializeField] Button exitMenu;
        [SerializeField] Button backMenu;

        private void Awake()
        {
            mainMenu.onClick.AddListener(() =>
            {
                GameManager.Instance.GoMainMenu_RPC();
                this.gameObject.SetActive(false);
                //PhotonManager.Instance.ChangeStage();
            });
            exitMenu.onClick.AddListener(() =>
            {
                PhotonManager.Instance.DestroyCurrentRoom_RPC();
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
