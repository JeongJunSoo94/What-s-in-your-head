using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

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

        private void Awake()
        {
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
            if (image.fillAmount >= 0.97f)
            {
                image.fillAmount = 0f;
                isLoading = false;
                this.gameObject.SetActive(false);
            }
        }
        [PunRPC]
        void StartLoading()
        {
            isLoading = true;
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(++GameManager.Instance.curStageIndex);

                // 플레이어들의 위치값 특정 위치로 옮기기.
                //GameManager.Instance.otherPlayerTF.position =                 
                //GameManager.Instance.myPlayerTF.position = 
            }
        }
    }
}

