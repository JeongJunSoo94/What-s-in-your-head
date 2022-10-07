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
        [Header("�ٸ� ĳ���� ��ư")] [SerializeField] private GameObject otherObj = null;

        [Header("�⺻ ��ư ��������Ʈ")] [SerializeField] protected Sprite defaultSprite = null;
        [Header("ȣ���� ��ư ��������Ʈ")] [SerializeField] protected Sprite onButtonSprite = null;
        [Header("���� ��ư ��������Ʈ")] [SerializeField] protected Sprite selectSprite = null;

        private Image thisImg;

        // �� ��ư�� ���� ����� �г����� üũ�ϱ� ���� ����
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

        // PunRPC�� �� ���� ����Ǵµ�, ���� ���ÿ� �Ҵ��Ϸ��� �ϴ��� �׽�Ʈ �ʿ�
        // ���� DeSelect�� ���� �����ؼ� Deselect�� �θ��� RPC �ڵ嵵 �ʿ�
        [PunRPC]
        public void SelectSprite(string playerName)
        {
            // ���õ��� ���� ���� ��ư�� �������� ��
            if (curButtonOwner.text == "")
            {
                curButtonOwner.text = playerName;
                thisImg.sprite = selectSprite;
                // �ٸ� ��ư�� �̹� �����߾��� ��
                if (otherButtonOwner.text == playerName)
                    otherObj.SendMessage("DeSelectSprite");
            }
            // ������ �����ߴ� �ڶ� ��ư�� �ٽ� ������ ��
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
