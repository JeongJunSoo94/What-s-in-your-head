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
        [Header("플레이어 1 버튼")][SerializeField] private GameObject player1_Button = null;
        [Header("플레이어 2 버튼")][SerializeField] private GameObject player2_Button = null;
        [Header("기본 버튼 스프라이트")][SerializeField] private Sprite defaultSprite1 = null;
                                        [SerializeField] private Sprite defaultSprite2 = null;
        [Header("완료 버튼 스프라이트")][SerializeField] private Sprite readySprite1 = null;
                                        [SerializeField] private Sprite readySprite2 = null;

        private bool isReady = false;

        private Image player1_Img;
        private Image player2_Img;

        private void Awake()
        {
            player1_Img = player1_Button.GetComponent<Image>();
            player2_Img = player2_Button.GetComponent<Image>();

            player1_Button.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    isReady = !isReady;
                    player1_Img.sprite = isReady ? readySprite1 : defaultSprite1;
                }
            });
            player2_Button.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    isReady = !isReady;
                    player2_Img.sprite = isReady ? readySprite2 : defaultSprite2;
                }                    
            });
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                isReady = !isReady;
                if (PhotonNetwork.IsMasterClient)
                    player1_Img.sprite = isReady ? readySprite1 : defaultSprite1;
                else
                    player2_Img.sprite = isReady ? readySprite2 : defaultSprite2;
            }
        }
    }
}
