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
    public class LoadingUI : MonoBehaviour
    {
        [Header("TIP 목록")][SerializeField] List<string> tipList = new();
        PhotonView photonView;

        bool isLoading = false;
        Image image;
        Text text;

        public static LoadingUI Instance = null;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            photonView = PhotonView.Get(this);
            image = transform.GetChild(1).GetComponent<Image>();
            text = transform.GetChild(2).GetComponent<Text>();
        }
        
        private void OnEnable()
        {
            if(PhotonNetwork.IsMasterClient)
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
                if (GameManager.Instance.curStageIndex == 1
                    && GameManager.Instance.curStageType == 1
                    && PhotonNetwork.IsMasterClient)
                {
                    PhotonManager.Instance.MakeCharacter();
                }
                this.gameObject.SetActive(false);
            }
        }
        [PunRPC]
        void StartLoading()
        {
            isLoading = true;
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(nameof(Delay));
        }
        
        // 로딩씬 보여주기 위한 딜레이
        IEnumerator Delay()
        {
            yield return new WaitForSeconds(2f);
            PhotonNetwork.LoadLevel(GameManager.Instance.curStageIndex * 2 - 2 + GameManager.Instance.curStageType);
            yield break;
        }
    }
}

