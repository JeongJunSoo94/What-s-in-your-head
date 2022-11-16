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
        [Header("넬라 On/Off 이미지")] [SerializeField] List<Sprite> spriteList_1;
        [Header("스테디 On/Off 이미지")] [SerializeField] List<Sprite> spriteList_2;

        VideoPlayer videoPlayer;

        bool isOn1 = false;
        bool isOn2 = false;

        bool isStart = false;
        bool isNella = false;
        bool isNewGame = false;
        private void Awake()
        {
            pv = PhotonView.Get(this);
            player1_Img = transform.GetChild(1).GetChild(0).GetComponent<Image>();
            player2_Img = transform.GetChild(1).GetChild(1).GetComponent<Image>();
            videoPlayer = transform.GetChild(0).GetComponent<VideoPlayer>();
            if (GameManager.Instance.characterOwner.Count == 0)
                isNewGame = true;
            else
                isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
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
            {
                if (loadingUI != null)
                    loadingUI.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            }
            
            if(KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
            {
                if(isNewGame)
                    pv.RPC(nameof(SetSkipImage), RpcTarget.AllViaServer, PhotonNetwork.IsMasterClient);
                else
                    pv.RPC(nameof(SetSkipImage), RpcTarget.AllViaServer, isNella);
            }
            if(isOn1 && isOn2)
            {
               if(loadingUI != null)
                   loadingUI.gameObject.SetActive(true);
               this.gameObject.SetActive(false);
            }
            
        }

        IEnumerator WaitUntilCutSceneEnd()
        {
            yield return new WaitUntil(() => videoPlayer.isPlaying);
            pv.RPC(nameof(SetStart), RpcTarget.AllViaServer);            
            yield break;
        }
        [PunRPC]
        void SetStart()
        {
            isStart = true;            
        }

        [PunRPC]
        void SyncStart()
        {
            videoPlayer.gameObject.SetActive(true);
            player1_Img.gameObject.SetActive(true);
            player2_Img.gameObject.SetActive(true);
            videoPlayer.Play();
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(nameof(WaitUntilCutSceneEnd));
        }

        [PunRPC]
        void SetSkipImage(bool isMaster)
        {
            if (isMaster)
            {
                isOn1 = !isOn1;
                player1_Img.sprite = spriteList_1[isOn1 ? 1 : 0];
            }
            else
            {
                isOn2 = !isOn2;
                player2_Img.sprite = spriteList_2[isOn2 ? 1 : 0];
            }
        }
    }

}
