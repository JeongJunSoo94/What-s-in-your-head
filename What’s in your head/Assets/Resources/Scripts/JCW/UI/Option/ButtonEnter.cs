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
        [Header("�÷��̾� 1 ��ư")][SerializeField] private GameObject player1_Button = null;
        [Header("�÷��̾� 2 ��ư")][SerializeField] private GameObject player2_Button = null;
        [Header("�ڷΰ��� ��ư")][SerializeField] private Button backButton = null;        
        [Header("�⺻ ��ư ��������Ʈ")][SerializeField] private Sprite defaultSprite1 = null;
                                     [SerializeField] private Sprite defaultSprite2 = null;
        [Header("�Ϸ� ��ư ��������Ʈ")][SerializeField] private Sprite readySprite1 = null;        
                                     [SerializeField] private Sprite readySprite2 = null;
        [Header("ĳ���� ���� UI")] [SerializeField] private GameObject charSelectUI = null;

        private bool isReady = false;

        private Image player1_Img;
        private Image player2_Img;

        private PhotonView photonView;

        private void Awake()
        {
            player1_Img = player1_Button.GetComponent<Image>();
            player2_Img = player2_Button.GetComponent<Image>();
            photonView = gameObject.GetComponent<PhotonView>();

            backButton.onClick.AddListener(() =>
            {                
                photonView.RPC("SetEnter", RpcTarget.AllViaServer, PhotonNetwork.IsMasterClient, true);
                this.gameObject.SetActive(false);
            });

            player1_Button.GetComponent<Button>().onClick.AddListener(() =>
            {
                photonView.RPC("SetEnter", RpcTarget.AllViaServer, PhotonNetwork.IsMasterClient, false);
            });
            player2_Button.GetComponent<Button>().onClick.AddListener(() =>
            {
                photonView.RPC("SetEnter", RpcTarget.AllViaServer, PhotonNetwork.IsMasterClient, false);
            });
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                photonView.RPC("SetEnter", RpcTarget.AllViaServer, PhotonNetwork.IsMasterClient, false);
            }
            //if(player1_Img.sprite == readySprite1 && player2_Img.sprite == readySprite2)
            //{
            //    photonView.RPC(nameof(CancleEnter), RpcTarget.AllViaServer);
            //    charSelectUI.SetActive(true);
            //}
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
        }
        [PunRPC]
        private void CancleEnter()
        {
            isReady = false;
            player1_Img.sprite = defaultSprite1;
            player2_Img.sprite = defaultSprite2;
        }
    }
}
