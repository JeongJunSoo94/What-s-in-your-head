using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

namespace JCW.UI
{
    [RequireComponent(typeof(PhotonView))]
    public class CharacterSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("다른 캐릭터 버튼")] [SerializeField] private GameObject otherObj = null;

        [Header("기본 버튼 스프라이트")] [SerializeField] protected Sprite defaultSprite = null;
        [Header("호버링 버튼 스프라이트")] [SerializeField] protected Sprite onButtonSprite = null;
        [Header("선택 버튼 스프라이트")] [SerializeField] protected Sprite selectSprite = null;

        private Image thisImg;

        // 각 버튼을 누른 사람의 닉네임을 체크하기 위한 변수
        private Text curButtonOwner;
        private Text otherButtonOwner;

        private PhotonView photonView;


        private void Awake()
        {
            thisImg = this.gameObject.GetComponent<Image>();
            photonView = this.gameObject.GetComponent<PhotonView>();
            curButtonOwner = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
            otherButtonOwner = otherObj.transform.GetChild(0).gameObject.GetComponent<Text>();
            this.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                //photonView.RPC(nameof(SelectSprite), RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.NickName);
                SelectSprite(PhotonNetwork.LocalPlayer.NickName);
            });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //photonView.RPC(nameof(ChangeSprite), RpcTarget.AllViaServer, true);
            ChangeSprite(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //photonView.RPC(nameof(ChangeSprite), RpcTarget.AllViaServer, false);
            ChangeSprite(false);
        }

        [PunRPC]
        public void ChangeSprite(bool isHovering)
        {
            if (thisImg.sprite != selectSprite)
                thisImg.sprite = isHovering ? onButtonSprite : defaultSprite;
        }

        // PunRPC라서 다 같이 실행되는데, 서로 동시에 할당하려고 하는지 테스트 필요
        // 또한 DeSelect를 따로 구분해서 Deselect를 부르는 RPC 코드도 필요
        [PunRPC]
        public void SelectSprite(string playerName)
        {
            // 선택되지 않은 현재 버튼을 선택했을 때
            if (curButtonOwner.text == "")
            {
                curButtonOwner.text = playerName;
                thisImg.sprite = selectSprite;
                // 다른 버튼을 이미 선택했었을 때
                if (otherButtonOwner.text == playerName)
                    otherObj.SendMessage("DeSelectSprite");
            }
            // 본인이 선택했던 넬라 버튼을 다시 눌렀을 때
            else if (curButtonOwner.text == playerName)
            {
                //photonView.RPC(nameof(DeSelectSprite), RpcTarget.AllViaServer);
                DeSelectSprite();
            }
        }

        [PunRPC]
        public void DeSelectSprite()
        {
            curButtonOwner.text = "";
            thisImg.sprite = defaultSprite;
        }
    }
}
