using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using JCW.Network;
using JCW.AudioCtrl;
using UnityEngine.SceneManagement;

namespace JCW.UI
{
    [RequireComponent(typeof(PhotonView))]
    public class LoadingUI : MonoBehaviour
    {
        [Header("TIP 목록")][SerializeField] List<string> tipList = new();
        [Header("스테이지 별 이미지 목록")][SerializeField] List<Sprite> bgList = new();

        PhotonView photonView;

        bool isLoading = false;
        Image image;
        Text text;

        public static LoadingUI Instance = null;
        Image bgImg;
        bool isMainTitle = false;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            photonView = PhotonView.Get(this);
            bgImg = transform.GetChild(0).GetComponent<Image>();
            image = transform.GetChild(1).GetComponent<Image>();
            text = transform.GetChild(2).GetComponent<Text>();
        }
        
        private void OnEnable()
        {
            isLoading = false;
            isMainTitle = SceneManager.GetActiveScene().name == "MainTitle";
            Debug.Log("현재 씬 이름 : " + SceneManager.GetActiveScene().name);
            PhotonNetwork.LevelLoadingProgress = 0f;
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(nameof(WaitForStable));
                       
            var random = new System.Random(Guid.NewGuid().GetHashCode());
            text.text = "TIP : " + tipList[random.Next(0, tipList.Count)];
            bgImg.sprite = bgList[GameManager.Instance.curStageIndex];
            SoundManager.Instance.StopBGM();
        }

        void Update()
        {
            Debug.Log("로딩 UI 시작 시 진행율 : " +(float)PhotonNetwork.LevelLoadingProgress*100f);
            if (!isLoading)
                return;

            image.fillAmount = PhotonNetwork.LevelLoadingProgress;
            if (image.fillAmount >= 0.9f)
            {
                image.fillAmount = 0f;
                // 임의로 만들어줌
                PhotonNetwork.LevelLoadingProgress = 0f;
                isLoading = false;
                Debug.Log("씬 불러오기 완료");
                //if (isMainTitle && PhotonNetwork.IsMasterClient)
                //{
                //    Debug.Log("캐릭터 만들어주기");
                //    PhotonManager.Instance.MakeCharacter();
                //}
                this.gameObject.SetActive(false);
            }
        }
        [PunRPC]
        void StartLoading()
        {
            Debug.Log("로딩 시작 진입");
            isLoading = true;
            if (!isMainTitle)
            {
                Debug.Log("메인 메뉴가 아니므로 더해주기");
                GameManager.Instance.curStageType = GameManager.Instance.curStageType >= 3 ? 0 : GameManager.Instance.curStageType + 1;
                GameManager.Instance.curStageIndex = GameManager.Instance.curStageType == 0 ? GameManager.Instance.curStageIndex + 1 : GameManager.Instance.curStageIndex;
                Debug.Log("S" + GameManager.Instance.curStageIndex + "S" + GameManager.Instance.curStageType);
            }
            if (PhotonNetwork.IsMasterClient)
            {
                int sceneNum = 4 * (GameManager.Instance.curStageIndex - 1) + 1 + GameManager.Instance.curStageType;
                Debug.Log("씬 넘버 : " + sceneNum + " 에 접근");
                PhotonNetwork.LoadLevel(sceneNum);
                //StartCoroutine(nameof(Delay));
            }
        }
        
        // 로딩씬 보여주기 위한 딜레이        
        IEnumerator Delay()
        {
            Debug.Log("대충 딜레이 1초 주기");
            yield return new WaitForSeconds(1f);
            // 한번 실행된 이후로는 위의 Wait문을 안 탐 왜지?
            //PhotonNetwork.LoadLevel(GameManager.Instance.curStageIndex * 2 - 2 + GameManager.Instance.curStageType);            
            int sceneNum = 4 * (GameManager.Instance.curStageIndex - 1) + 1 + GameManager.Instance.curStageType;
            Debug.Log("씬 넘버 : " + sceneNum +  " 에 접근");
            PhotonNetwork.LoadLevel(sceneNum);
            yield break;
        }

        IEnumerator WaitForStable()
        {
            while(PhotonNetwork.LevelLoadingProgress > 0f)
            {
                Debug.Log("포톤 씬이 현재 불러와져있는 상태라 대기 : " + PhotonNetwork.LevelLoadingProgress);
                yield return null;
            }
            photonView.RPC(nameof(StartLoading), RpcTarget.AllViaServer);
        }
    }
}

