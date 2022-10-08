using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using JCW.Network;

namespace JCW.UI
{
    public class ButtonEnter : MonoBehaviour
    {
        [Header("플레이어 버튼")][SerializeField] private Button player1_Button = null;
                                [SerializeField] private Button player2_Button = null;
        [Header("뒤로가기 버튼")][SerializeField] private Button backButton = null;        
        [Header("기본 버튼 스프라이트")][SerializeField] private Sprite defaultSprite1 = null;
                                     [SerializeField] private Sprite defaultSprite2 = null;
        [Header("완료 버튼 스프라이트")][SerializeField] private Sprite readySprite1 = null;        
                                     [SerializeField] private Sprite readySprite2 = null;
        [Header("캐릭터 선택 UI")] [SerializeField] private GameObject charSelectUI = null;
        [Header("넘어가기 버튼")] [SerializeField] private Button moveOnButton = null;

        private bool isReady = false;

        private Image player1_Img;
        private Image player2_Img;

        private PhotonView photonView;

        private void Awake()
        {
            player1_Img = player1_Button.gameObject.GetComponent<Image>();
            player2_Img = player2_Button.gameObject.GetComponent<Image>();
            photonView = gameObject.GetComponent<PhotonView>();

            backButton.onClick.AddListener(() =>
            {
                photonView.RPC(nameof(SetEnter), RpcTarget.AllViaServer, PhotonNetwork.IsMasterClient, true);
                photonView.RPC(nameof(Leave), RpcTarget.AllViaServer);
            });

            player1_Button.onClick.AddListener(() =>
            {
                if(PhotonNetwork.IsMasterClient)
                    photonView.RPC(nameof(SetEnter), RpcTarget.AllViaServer, true, false);
            });
            player2_Button.onClick.AddListener(() =>
            {
                if(!PhotonNetwork.IsMasterClient)
                    photonView.RPC(nameof(SetEnter), RpcTarget.AllViaServer, false, false);
            });

            moveOnButton.onClick.AddListener(() =>
            {
                photonView.RPC(nameof(CancleEnter), RpcTarget.AllViaServer);
                photonView.RPC(nameof(MoveOn), RpcTarget.AllViaServer);                
            });
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                photonView.RPC(nameof(SetEnter), RpcTarget.AllViaServer, PhotonNetwork.IsMasterClient, false);
            }
        }

        [PunRPC]
        private void SetEnter(bool isMaster, bool leave = false)
        {
            isReady = !isReady;
            if (leave && isReady)
                isReady = !isReady;

            if (isMaster)
                player1_Img.sprite = isReady ? readySprite1 : defaultSprite1;
            else
                player2_Img.sprite = isReady ? readySprite2 : defaultSprite2;

            if (player1_Img.sprite == readySprite1
                && player2_Img.sprite == readySprite2)
                moveOnButton.gameObject.SetActive(true);
            else
                moveOnButton.gameObject.SetActive(false );

        }
        [PunRPC]
        private void CancleEnter()
        {
            isReady = false;
            player1_Img.sprite = defaultSprite1;
            player2_Img.sprite = defaultSprite2;
        }

        [PunRPC]
        private void MoveOn()
        {
            charSelectUI.SetActive(true);
            moveOnButton.gameObject.SetActive(false);
        }

        [PunRPC]
        private void Leave()
        {
            PhotonNetwork.LeaveRoom();
            this.gameObject.SetActive(false);
        }
    }
}
