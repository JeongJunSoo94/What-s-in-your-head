using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using JCW.UI.Options;
using UnityEngine.SceneManagement;
using YC.CameraManager_;

namespace JCW.UI
{
    public class PauseManager : MonoBehaviour
    {
        private enum PauseMenu { Resume, Checkpoint, Option, Exit };
        
        [Header("�Ͻ����� �޴� ����Ʈ")] [SerializeField] List<Button> buttonList = new();
        [Header("üũ����Ʈ ��ư UI")] [SerializeField] GameObject checkPointUI;
        [Header("�ɼ� ��ư UI")] [SerializeField] GameObject optionUI;
        [Header("���� ��ư UI")] [SerializeField] GameObject exitUI;

        public int childOnIndex = 0;

        public static PauseManager Instance = null;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this)
                Destroy(this.gameObject);

            buttonList[(int)PauseMenu.Resume].onClick.AddListener(() =>
            {
                this.gameObject.SetActive(false);
            });

            buttonList[(int)PauseMenu.Checkpoint].onClick.AddListener(() =>
            {
                checkPointUI.SetActive(true); childOnIndex = 1;
            });
            buttonList[(int)PauseMenu.Option].onClick.AddListener(() => { optionUI.SetActive(true); childOnIndex = 2; });
            buttonList[(int)PauseMenu.Exit].onClick.AddListener(() =>  { exitUI.SetActive(true); childOnIndex = 3; });
        }

        private void OnEnable()
        {
            if (SceneManager.GetActiveScene().name == "MainTitle")
                gameObject.SetActive(false);
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                if (GameManager.Instance.myPlayerTF)
                    GameManager.Instance.myPlayerTF.GetComponent<PlayerState>().isOutOfControl = true;
                if (CameraManager.Instance)
                    CameraManager.Instance.BlockCinemachineInput(true);
            }
        }

        private void OnDisable()
        {
            if (SceneManager.GetActiveScene().name != "MainTitle")
            {
                if(GameManager.Instance.myPlayerTF)
                    GameManager.Instance.myPlayerTF.GetComponent<PlayerState>().isOutOfControl = false;
                if(CameraManager.Instance)
                    CameraManager.Instance.BlockCinemachineInput(false);
            }
        }

        public void CloseUI()
        {
            switch(childOnIndex)
            {
                case 0:
                    this.gameObject.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                case 1:
                    checkPointUI.SetActive(false);
                    childOnIndex = 0;
                    break;
                case 2:
                    optionUI.SetActive(false);
                    childOnIndex = 0;
                    break;
                case 3:
                    exitUI.SetActive(false);
                    childOnIndex = 0;
                    break;
            }
        }
    }
}

