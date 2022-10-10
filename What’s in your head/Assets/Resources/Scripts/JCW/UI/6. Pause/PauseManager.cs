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
        [Header("체크포인트 버튼 UI")] [SerializeField] GameObject checkPointUI;
        [Header("옵션 버튼 UI")] [SerializeField] GameObject optionUI;
        [Header("종료 버튼 UI")] [SerializeField] GameObject exitUI;

        public static PauseManager Instance = null;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else if (Instance != this)
                Destroy(this.gameObject);

            if (GameManager.Instance.stopPlayerName == PhotonNetwork.LocalPlayer.NickName)
            {
                buttonList[(int)PauseMenu.Resume].onClick.AddListener(() =>
                {
                    this.gameObject.SetActive(false);
                });
            }            
            buttonList[(int)PauseMenu.Checkpoint].onClick.AddListener(() =>
            {
                checkPointUI.SetActive(true);
            });
            buttonList[(int)PauseMenu.Option].onClick.AddListener(() => { optionUI.SetActive(true); });
            buttonList[(int)PauseMenu.Exit].onClick.AddListener(() =>  { exitUI.SetActive(true);   });
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

