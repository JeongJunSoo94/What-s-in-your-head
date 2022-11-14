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
        [Header("다른 캐릭터 버튼")] [SerializeField] private GameObject otherObj = null;

        [Header("기본 버튼 스프라이트")] [SerializeField] protected Sprite defaultSprite = null;
        [Header("호버링 버튼 스프라이트")] [SerializeField] protected Sprite onButtonSprite = null;
        [Header("선택 버튼 스프라이트")] [SerializeField] protected Sprite selectSprite = null;
        [Header("로딩 UI")] [SerializeField] GameObject loadingUI;
        private GameObject selectCharacter = null;

        private Image thisImg;

        // 각 버튼을 누른 사람의 닉네임을 체크하기 위한 변수
        private Text curButtonOwner;
        private Text otherButtonOwner;

        // 각 버튼의 캐릭터 이름과 설명의 투명도를 바꾸기 위한 변수.
        private Text charName;
        private Text charDesc;

        // 스테디 전용 컬러
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

            // 버튼을 누른 사람
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
            // 선택되지 않은 현재 버튼을 선택했을 때
            if (curButtonOwner.text == "")
            {
                if (GameManager.Instance.characterOwner.ContainsKey(isMaster))
                    GameManager.Instance.characterOwner.Remove(isMaster);
                GameManager.Instance.characterOwner.Add(isMaster, isNella);
                curButtonOwner.text = playerName;
                thisImg.sprite = selectSprite;

                ownPlayer[0].SetActive(false);
                ownPlayer[1].SetActive(false);
                selectCharacter.transform.GetChild(0).gameObject.GetComponent<Text>().text = isMaster ? "플레이어 1" : "플레이어2";
                selectCharacter.SetActive(true);

                // 스테디일 경우에만 폰트 하얀색으로 변경
                if (!isNella)
                {
                    charName.color = bwColors[1];
                    charDesc.color = bwColors[1];
                }
                // 다른 버튼을 이미 선택했었을 때
                if (otherButtonOwner.text == playerName)
                {
                    otherObj.SendMessage("DeSelectSprite", false);
                }
                else if (otherButtonOwner.text != "")
                {
                    foreach (bool ism in GameManager.Instance.characterOwner.Keys)
                    {
                        string player = ism ? "플레이어 1" : "플레이어 2";
                        string character = GameManager.Instance.characterOwner[ism] ? "넬라" : "스테디";
                        Debug.Log(player + " : " + character);
                    }
                    //LoadingScene.Instance.gameObject.SetActive(true);
                    loadingUI.SetActive(true);
                    GameManager.Instance.AddAliveState(GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient], true);
                    this.enabled = false;
                }
            }
            // 본인이 선택했던 캐릭터 버튼을 다시 눌렀을 때
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
