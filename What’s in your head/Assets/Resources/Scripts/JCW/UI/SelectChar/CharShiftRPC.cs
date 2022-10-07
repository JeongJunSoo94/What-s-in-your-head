using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JCW.UI.Options;
using Photon.Pun;
using Photon.Realtime;

namespace JCW.UI
{
    [RequireComponent(typeof(PhotonView))]
    public class CharShiftRPC : MonoBehaviour
    {
        [Header("기본 버튼 스프라이트")] [SerializeField] protected Sprite NellaDefaultSprite = null;
                                      [SerializeField] protected Sprite SteadyDefaultSprite = null;

        [Header("호버링 버튼 스프라이트")] [SerializeField] protected Sprite NellaOnButtonSprite = null;
                                       [SerializeField] protected Sprite SteadyOnButtonSprite = null;

        [Header("선택 버튼 스프라이트")] [SerializeField] protected Sprite NellaSelectSprite = null;
                                    [SerializeField] protected Sprite SteadySelectSprite = null;
        [Header("넬라/스테디 버튼")] [SerializeField] private Button NellaButton = null;
                                    [SerializeField] private Button SteadyButton = null;

        private Image NellaImg = null;
        private Image SteadyImg = null;

        private PhotonView photonView;
        private Text NellaButtonOwner;
        private Text SteadyButtonOwner;


        private void Awake()
        {
            NellaImg = NellaButton.gameObject.GetComponent<Image>();
            SteadyImg = SteadyButton.gameObject.GetComponent<Image>();

            NellaButtonOwner = NellaButton.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
            SteadyButtonOwner = SteadyButton.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();

            photonView = this.gameObject.GetComponent<PhotonView>();
        }


        public void SetDefaultSprite()
        {
            photonView.RPC("SelectSprite", RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.NickName, true);
        }

        public void ChangeSpriteRPC(bool isNella, bool isHovering)
        {
            photonView.RPC("ChangeSprite", RpcTarget.AllViaServer, isNella, isHovering);
        }

        [PunRPC]
        public void ChangeSprite(bool isNella, bool isHovering)
        {
            if(isNella)
            {
                if(NellaImg.sprite != NellaSelectSprite)
                    NellaImg.sprite = isHovering ? NellaOnButtonSprite : NellaDefaultSprite;
            }
            else
            {
                if (SteadyImg.sprite != SteadySelectSprite)
                    SteadyImg.sprite = isHovering ? SteadyOnButtonSprite : SteadyDefaultSprite;
            }
        }

        public void SelectSpriteRPC(string playerName, bool isExit, bool isNella)
        {
            photonView.RPC("SelectSprite", RpcTarget.AllViaServer, playerName, isExit, isNella);
        }

        [PunRPC]
        public void SelectSprite(string playerName, bool isExit, bool isNella)
        {
            // 플레이어가 선택한 채로 나가면
            if (isExit)
            {
                // 선택했던 버튼을 초기화하고 나감.
                if (NellaButtonOwner.text == playerName)
                {
                    NellaButtonOwner.text = "";
                    NellaImg.sprite = NellaDefaultSprite;
                }
                else if (SteadyButtonOwner.text == playerName)
                {
                    SteadyButtonOwner.text = "";
                    SteadyImg.sprite = SteadyDefaultSprite;
                }
            }
            else
            {
                if(isNella)
                {
                    // 선택되지 않은 넬라 버튼을 선택했을 때
                    if (NellaButtonOwner.text == "")
                    {
                        NellaButtonOwner.text = playerName;
                        NellaImg.sprite = NellaSelectSprite;
                        // 스테디 버튼을 이미 선택했었을 때
                        if (SteadyButtonOwner.text == playerName)
                        {
                            SteadyButtonOwner.text = "";
                            SteadyImg.sprite = SteadyDefaultSprite;
                        }
                    }
                    // 본인이 선택했던 넬라 버튼을 다시 눌렀을 때
                    else if (NellaButtonOwner.text == playerName)
                    {
                        NellaButtonOwner.text = "";
                        NellaImg.sprite = SteadyDefaultSprite;
                    }
                }
                else
                {
                    // 선택되지 않은 스테디 버튼을 선택했을 때
                    if (SteadyButtonOwner.text == "")
                    {
                        SteadyButtonOwner.text = playerName;
                        SteadyImg.sprite = SteadySelectSprite;
                        // 넬라 버튼을 이미 선택했었을 때
                        if (NellaButtonOwner.text == playerName)
                        {
                            NellaButtonOwner.text = "";
                            NellaImg.sprite = NellaDefaultSprite;
                        }
                    }
                    // 본인이 선택했던 스테디 버튼을 다시 눌렀을 때
                    else if (SteadyButtonOwner.text == playerName)
                    {
                        SteadyButtonOwner.text = "";
                        SteadyImg.sprite = SteadyDefaultSprite;
                    }
                }                
            }
        }
    }
}
