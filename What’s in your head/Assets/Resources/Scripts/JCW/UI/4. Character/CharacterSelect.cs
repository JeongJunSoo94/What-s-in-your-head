using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using JCW.Network;

namespace JCW.UI
{
    [RequireComponent(typeof(PhotonView))]
    public class CharacterSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("�ٸ� ĳ���� ��ư")] [SerializeField] private GameObject otherObj = null;

        [Header("�⺻ ��ư ��������Ʈ")] [SerializeField] protected Sprite defaultSprite = null;
        [Header("ȣ���� ��ư ��������Ʈ")] [SerializeField] protected Sprite onButtonSprite = null;
        [Header("���� ��ư ��������Ʈ")] [SerializeField] protected Sprite selectSprite = null;
        [Header("�ε� UI")] [SerializeField] GameObject loadingUI;
        private GameObject selectCharacter = null;

        private Image thisImg;

        // �� ��ư�� ���� ����� �г����� üũ�ϱ� ���� ����
        private Text curButtonOwner;
        private Text otherButtonOwner;

        // �� ��ư�� ĳ���� �̸��� ������ ������ �ٲٱ� ���� ����.
        private Text charName;
        private Text charDesc;

        // ���׵� ���� �÷�
        private readonly List<Color> bwColors = new();

        private PhotonView photonView;

        private readonly List<GameObject> ownPlayer = new();

        private bool isNella = false;


        private void Awake()
        {
            bwColors.Add(new Color(0, 0, 0, 1));
            bwColors.Add(new Color(1, 1, 1, 1));

            if (this.gameObject.name.Contains("Nella"))
                isNella = true;

            thisImg = GetComponent<Image>();
            photonView = GetComponent<PhotonView>();

            // ��ư�� ���� ���
            curButtonOwner = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
            otherButtonOwner = otherObj.transform.GetChild(0).gameObject.GetComponent<Text>();

            ownPlayer.Add(curButtonOwner.gameObject.transform.GetChild(0).gameObject);
            ownPlayer.Add(curButtonOwner.gameObject.transform.GetChild(1).gameObject);

            selectCharacter = this.gameObject.transform.GetChild(2).gameObject;

            GameObject character = this.gameObject.transform.GetChild(3).gameObject;
            charName = character.GetComponent<Text>();
            charDesc = character.transform.GetChild(0).gameObject.GetComponent<Text>();

            GetComponent<Button>().onClick.AddListener(() =>
            {
                photonView.RPC(nameof(SelectSprite), RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.NickName, PhotonNetwork.IsMasterClient);
            });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            photonView.RPC(nameof(ChangeSprite), RpcTarget.AllViaServer, true, PhotonNetwork.IsMasterClient);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(this.gameObject.activeSelf)
                photonView.RPC(nameof(ChangeSprite), RpcTarget.AllViaServer, false, PhotonNetwork.IsMasterClient);
        }

        [PunRPC]
        public void ChangeSprite(bool isHovering, bool isMaster)
        {
            if (thisImg.sprite != selectSprite)
            {
                if (isHovering)
                {
                    thisImg.sprite = onButtonSprite;
                    SetVisible(charName);
                    SetVisible(charDesc);
                    if(isMaster)
                        ownPlayer[0].SetActive(true);
                    else
                        ownPlayer[1].SetActive(true);
                }
                else
                {
                    if(!ownPlayer[0].activeSelf || !ownPlayer[1].activeSelf)
                    {
                        thisImg.sprite = defaultSprite;
                        SetVisible(charName, false);
                        SetVisible(charDesc, false);
                    }
                    if (isMaster)
                        ownPlayer[0].SetActive(false);
                    else
                        ownPlayer[1].SetActive(false);
                }
            }            
        }


        [PunRPC]
        public void SelectSprite(string playerName, bool isMaster)
        {            
            // ���õ��� ���� ���� ��ư�� �������� ��
            if (curButtonOwner.text == "")
            {
                if (GameManager.Instance.characterOwner.ContainsKey(isMaster))
                    GameManager.Instance.characterOwner.Remove(isMaster);
                GameManager.Instance.characterOwner.Add(isMaster, isNella);
                curButtonOwner.text = playerName;
                thisImg.sprite = selectSprite;

                ownPlayer[0].SetActive(false);
                ownPlayer[1].SetActive(false);
                selectCharacter.transform.GetChild(0).gameObject.GetComponent<Text>().text = isMaster ? "�÷��̾� 1" : "�÷��̾�2";
                selectCharacter.SetActive(true);

                // ���׵��� ��쿡�� ��Ʈ �Ͼ������ ����
                if (!isNella)
                {
                    charName.color = bwColors[1];
                    charDesc.color = bwColors[1];
                }
                // �ٸ� ��ư�� �̹� �����߾��� ��
                if (otherButtonOwner.text == playerName)
                {
                    otherObj.SendMessage("DeSelectSprite", false);
                }
                else if (otherButtonOwner.text != "")
                {
                    foreach (bool ism in GameManager.Instance.characterOwner.Keys)
                    {
                        string player = ism ? "�÷��̾� 1" : "�÷��̾� 2";
                        string character = GameManager.Instance.characterOwner[ism] ? "�ڶ�" : "���׵�";
                        Debug.Log(player + " : " + character);
                    }
                    //LoadingScene.Instance.gameObject.SetActive(true);
                    loadingUI.SetActive(true);
                    GameManager.Instance.AddAliveState(GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient], true);
                    this.enabled = false;
                }
            }
            // ������ �����ߴ� ĳ���� ��ư�� �ٽ� ������ ��
            else if (curButtonOwner.text == playerName)
            {
                GameManager.Instance.characterOwner.Remove(isMaster);
                DeSelectSprite();
                if (isMaster)
                    ownPlayer[0].SetActive(true);
                else
                    ownPlayer[1].SetActive(true);
            }
        }

        public void DeSelectSprite(bool isDeactiveSelf = true)
        {
            photonView.RPC(nameof(DeSelectSpriteRPC), RpcTarget.AllViaServer, isDeactiveSelf);
            selectCharacter.SetActive(false);
        }


        [PunRPC]
        public void DeSelectSpriteRPC(bool isDeactiveSelf)
        {            
            curButtonOwner.text = "";
            thisImg.sprite = isDeactiveSelf ? onButtonSprite : defaultSprite;
            if (!isNella)
            {
                charName.color = bwColors[0];
                charDesc.color = bwColors[0];
            }
        }

        void SetVisible(Text _text ,bool isVisible = true)
        {
            Color setColor = _text.color;
            setColor.a = isVisible ? 1 : 0;
            _text.color = setColor;
        }
    }
}
