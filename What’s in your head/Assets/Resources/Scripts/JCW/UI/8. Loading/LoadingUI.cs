using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using JCW.Network;
using JCW.AudioCtrl;
using UnityEngine.SceneManagement;
using JCW.Dialog;

namespace JCW.UI
{
    [RequireComponent(typeof(PhotonView))]
    public class LoadingUI : MonoBehaviour
    {
        [Header("TIP ���")][SerializeField] List<string> tipList = new();
        [Header("�������� �� �̹��� ���")][SerializeField] List<Sprite> bgList = new();

        PhotonView photonView;

        bool isLoading = false;
        Image image;
        Text text;

        public static LoadingUI Instance = null;
        Image bgImg;
        [HideInInspector] public bool isMainTitle = false;

        bool isFirst = true;
        WaitForSeconds ws;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            photonView = GetComponent<PhotonView>();
            ws = new(0.8f);
            bgImg = transform.GetChild(0).GetComponent<Image>();
            image = transform.GetChild(1).GetComponent<Image>();
            text = transform.GetChild(2).GetComponent<Text>();
        }
        
        private void OnEnable()
        {
            if(isFirst)
            {
                isFirst = false;
                gameObject.SetActive(false);
                return;
            }
            DialogManager.Instance.ResetDialogs();
            isLoading = false;
            isMainTitle = SceneManager.GetActiveScene().name == "MainTitle";
            Debug.Log("���� �� �̸� : " + SceneManager.GetActiveScene().name);
            PhotonNetwork.LevelLoadingProgress = 0f;
                       
            var random = new System.Random(Guid.NewGuid().GetHashCode());
            text.text = "TIP : " + tipList[random.Next(0, tipList.Count)];
            bgImg.sprite = bgList[GameManager.Instance.curStageIndex];
            SoundManager.Instance.StopBGM();

            GameManager.Instance.ResetDefault();
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(nameof(WaitForStable));
        }

        void Update()
        {
            if (!isLoading)
                return;

            image.fillAmount = PhotonNetwork.LevelLoadingProgress;
            if (image.fillAmount >= 0.9f)
            {
                image.fillAmount = 0f;
                // ���Ƿ� �������
                PhotonNetwork.LevelLoadingProgress = 0f;
                isLoading = false;
                this.gameObject.SetActive(false);
            }
        }
        [PunRPC]
        void StartLoading()
        {
            isLoading = true;
            if (!isMainTitle)
            {
                GameManager.Instance.curStageType = GameManager.Instance.curStageType >= 3 ? 0 : GameManager.Instance.curStageType + 1;
                GameManager.Instance.curStageIndex = GameManager.Instance.curStageType == 0 ? GameManager.Instance.curStageIndex + 1 : GameManager.Instance.curStageIndex;
                if (GameManager.Instance.curStageIndex == 4 && GameManager.Instance.curStageType == 2
                    && PhotonNetwork.IsMasterClient)
                {
                    GameManager.Instance.GoMainMenu_RPC();
                    return;
                }
            }
            if (PhotonNetwork.IsMasterClient)
            {                
                int sceneNum = 4 * (GameManager.Instance.curStageIndex - 1) + 1 + GameManager.Instance.curStageType;
                if (GameManager.Instance.curStageIndex <=3 &&
                    (GameManager.Instance.curStageType == 1 || GameManager.Instance.curStageType == 2))
                {
                    StartCoroutine(nameof(DelayStage), sceneNum);
                }
                else
                    PhotonNetwork.LoadLevel(sceneNum);
                //StartCoroutine(nameof(Delay));
            }
        }

        IEnumerator DelayStage(int sceneNum)
        {
            yield return ws;
            PhotonNetwork.LoadLevel(sceneNum);
        }
        
        // �ε��� �����ֱ� ���� ������        
       //IEnumerator Delay()
       //{
       //   //Debug.Log("���� ������ 1�� �ֱ�");
       //    yield return new WaitForSeconds(1f);
       //    // �ѹ� ����� ���ķδ� ���� Wait���� �� Ž ����?
       //    //PhotonNetwork.LoadLevel(GameManager.Instance.curStageIndex * 2 - 2 + GameManager.Instance.curStageType);            
       //    int sceneNum = 4 * (GameManager.Instance.curStageIndex - 1) + 1 + GameManager.Instance.curStageType;
       //    Debug.Log("�� �ѹ� : " + sceneNum +  " �� ����");
       //    PhotonNetwork.LoadLevel(sceneNum);
       //    yield break;
       //}

        IEnumerator WaitForStable()
        {
            while(PhotonNetwork.LevelLoadingProgress > 0f)
            {
                yield return null;
            }
            photonView.RPC(nameof(StartLoading), RpcTarget.AllViaServer);
        }
    }
}

