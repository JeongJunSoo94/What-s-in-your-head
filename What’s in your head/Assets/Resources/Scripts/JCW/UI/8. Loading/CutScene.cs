using JCW.UI.Options.InputBindings;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class CutScene : MonoBehaviour
    {
        PhotonView pv;
        Image player1_Img;
        Image player2_Img;
        [Header("재생할 로딩씬")] [SerializeField] LoadingUI loadingUI;

        VideoPlayer videoPlayer;

        bool isOn1 = false;
        bool isOn2 = false;

        bool isStart = false;
        private void Awake()
        {
            pv = PhotonView.Get(this);
            player1_Img = transform.GetChild(1).GetComponent<Image>();
            player2_Img = transform.GetChild(2).GetComponent<Image>();
            videoPlayer = transform.GetChild(0).GetComponent<VideoPlayer>();
        }

        private void OnEnable()
        {
            isStart = false;
            if (PhotonNetwork.IsMasterClient)
                pv.RPC(nameof(SyncStart), RpcTarget.AllViaServer);
        }

        void Update()
        {
            if(isStart && !videoPlayer.isPlaying)
                this.gameObject.SetActive(false);
            /*
            if(KeyManager.Instance.GetKeyDown(PlayerAction.))
            {
                if(PhotonNetwork.IsMasterClient)
                {
                    isOn1 = !isOn1;
                    SetSkipImage(isOn1);
                }
                else
                {
                    isOn2 = !isOn2;
                    SetSkipImage(isOn2);
                }
                isOn = !isOn;
                SetSkipImage(isOn);                
            }
            if(isOn1 && isOn2)
            {
                if(loadingUI != null)
                    loadingUI.SetActive(true);
                this.gameObject.SetActive(false);
            }
            */
        }
        [PunRPC]
        void SyncStart()
        {
            videoPlayer.gameObject.SetActive(true);
            player1_Img.gameObject.SetActive(true);
            player2_Img.gameObject.SetActive(true);
            StartCoroutine(nameof(WaitUntilEnd));
        }

        [PunRPC]
        void SetSkipImage(bool isOn)
        {
            if (PhotonNetwork.IsMasterClient)
                player1_Img.enabled = isOn;
            else
                player2_Img.enabled = isOn;
        }

        IEnumerator WaitUntilEnd()
        {
            yield return new WaitUntil(() => videoPlayer.isPlaying);
            isStart = true;
            
            yield break;
        }
    }

}
