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

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            photonView = PhotonView.Get(this);
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
            GameManager.Instance.SetCharOnScene_RPC(false);
            DialogManager.Instance.ResetDialogs();
            isLoading = false;
            isMainTitle = SceneManager.GetActiveScene().name == "MainTitle";
            Debug.Log("���� �� �̸� : " + SceneManager.GetActiveScene().name);
            PhotonNetwork.LevelLoadingProgress = 0f;
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(nameof(WaitForStable));
                       
            var random = new System.Random(Guid.NewGuid().GetHashCode());
            text.text = "TIP : " + tipList[random.Next(0, tipList.Count)];
            bgImg.sprite = bgList[GameManager.Instance.curStageIndex];
            SoundManager.Instance.StopBGM();

            GameManager.Instance.ResetDefault_RPC();
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
                Debug.Log("�� �ҷ����� �Ϸ�");
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
                if (GameManager.Instance.curStageIndex == 4 && GameManager.Instance.curStageType == 1
                    && PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel(0);
                    return;
                }
            }
            if (PhotonNetwork.IsMasterClient)
            {
                int sceneNum = 4 * (GameManager.Instance.curStageIndex - 1) + 1 + GameManager.Instance.curStageType;
                PhotonNetwork.LoadLevel(sceneNum);
                //StartCoroutine(nameof(Delay));
            }
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
                //Debug.Log("���� ���� ���� �ҷ������ִ� ���¶� ��� : " + PhotonNetwork.LevelLoadingProgress);
                yield return null;
            }
            photonView.RPC(nameof(StartLoading), RpcTarget.AllViaServer);
        }
    }
}

