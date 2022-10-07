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
        [Header("�⺻ ��ư ��������Ʈ")] [SerializeField] protected Sprite NellaDefaultSprite = null;
                                      [SerializeField] protected Sprite SteadyDefaultSprite = null;

        [Header("ȣ���� ��ư ��������Ʈ")] [SerializeField] protected Sprite NellaOnButtonSprite = null;
                                       [SerializeField] protected Sprite SteadyOnButtonSprite = null;

        [Header("���� ��ư ��������Ʈ")] [SerializeField] protected Sprite NellaSelectSprite = null;
                                    [SerializeField] protected Sprite SteadySelectSprite = null;
        [Header("�ڶ�/���׵� ��ư")] [SerializeField] private Button NellaButton = null;
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
            // �÷��̾ ������ ä�� ������
            if (isExit)
            {
                // �����ߴ� ��ư�� �ʱ�ȭ�ϰ� ����.
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
                    // ���õ��� ���� �ڶ� ��ư�� �������� ��
                    if (NellaButtonOwner.text == "")
                    {
                        NellaButtonOwner.text = playerName;
                        NellaImg.sprite = NellaSelectSprite;
                        // ���׵� ��ư�� �̹� �����߾��� ��
                        if (SteadyButtonOwner.text == playerName)
                        {
                            SteadyButtonOwner.text = "";
                            SteadyImg.sprite = SteadyDefaultSprite;
                        }
                    }
                    // ������ �����ߴ� �ڶ� ��ư�� �ٽ� ������ ��
                    else if (NellaButtonOwner.text == playerName)
                    {
                        NellaButtonOwner.text = "";
                        NellaImg.sprite = SteadyDefaultSprite;
                    }
                }
                else
                {
                    // ���õ��� ���� ���׵� ��ư�� �������� ��
                    if (SteadyButtonOwner.text == "")
                    {
                        SteadyButtonOwner.text = playerName;
                        SteadyImg.sprite = SteadySelectSprite;
                        // �ڶ� ��ư�� �̹� �����߾��� ��
                        if (NellaButtonOwner.text == playerName)
                        {
                            NellaButtonOwner.text = "";
                            NellaImg.sprite = NellaDefaultSprite;
                        }
                    }
                    // ������ �����ߴ� ���׵� ��ư�� �ٽ� ������ ��
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
