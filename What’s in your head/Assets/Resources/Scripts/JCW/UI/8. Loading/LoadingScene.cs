using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using JCW.Network;

namespace JCW.UI
{
    [RequireComponent(typeof(PhotonView))]
    public class LoadingScene : MonoBehaviour
    {
        [Header("TIP 목록")][SerializeField] List<string> tipList = new();
        PhotonView photonView;

        bool isLoading = false;
        Image image;
        Text text;

        public static LoadingScene Instance = null;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            photonView = PhotonView.Get(this);
            image = transform.GetChild(1).GetComponent<Image>();
            text = transform.GetChild(2).GetComponent<Text>();
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnEnable()
        {
            photonView.RPC(nameof(StartLoading), RpcTarget.AllViaServer);
            var random = new System.Random(Guid.NewGuid().GetHashCode());
            text.text = "TIP : " + tipList[random.Next(0, tipList.Count)];
        }

        void Update()
        {
            if (!isLoading)
                return;

            image.fillAmount = PhotonNetwork.LevelLoadingProgress;
            if (image.fillAmount >= 0.98f)
            {
                image.fillAmount = 0f;
                isLoading = false;
                this.gameObject.SetActive(false);
                if (GameManager.Instance.curStageIndex == 1 && GameManager.Instance.curStageType == 1
                    && PhotonNetwork.IsMasterClient)
                    PhotonManager.Instance.MakeCharacter();
            }
        }

        [PunRPC]
        void StartLoading()
        {
            isLoading = true;
            if (PhotonNetwork.IsMasterClient)
            {
                //PhotonNetwork.LoadLevel(++GameManager.Instance.curStageIndex);
                PhotonNetwork.LoadLevel(GameManager.Instance.curStageIndex * 2 - 2 + GameManager.Instance.curStageType);
                
                    
                // 포탈에서 다음 스테이지 값과 섹션, 타입들을 설정해줘야할 듯.

                // 플레이어들의 위치값 특정 위치로 옮기기.
                //GameManager.Instance.otherPlayerTF.position =                 
                //GameManager.Instance.myPlayerTF.position = 
            }
        }
    }
}

