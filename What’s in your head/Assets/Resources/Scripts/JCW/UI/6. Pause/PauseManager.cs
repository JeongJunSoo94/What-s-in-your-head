using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using JCW.UI.Options;

namespace JCW.UI
{
    public class PauseManager : MonoBehaviour
    {
        private enum PauseMenu { Resume, Checkpoint, Option, Exit };
        
        [Header("일시정지 메뉴 리스트")] [SerializeField] List<Button> buttonList = new();
        [Header("종료 버튼 UI")] [SerializeField] GameObject exitUI;

        private void Awake()
        {           
            if(GameManager.Instance.stopPlayerName == PhotonNetwork.LocalPlayer.NickName)
            {
                buttonList[(int)PauseMenu.Resume].onClick.AddListener(() =>
                {
                    this.gameObject.SetActive(false);
                });
            }            
            buttonList[(int)PauseMenu.Checkpoint].onClick.AddListener(() =>
            {
                Debug.Log("아직 미구현");
            });
            buttonList[(int)PauseMenu.Option].onClick.AddListener(() =>
            {
                SingletonOption.Instance.gameObject.SetActive(true);
            });
            buttonList[(int)PauseMenu.Exit].onClick.AddListener(() =>
            {
                exitUI.SetActive(true);                
            });
        }
        private void OnEnable()
        {
            Time.timeScale = 0.0f;
        }
        private void OnDisable()
        {
            Time.timeScale = 1.0f;
        }
    }
}

