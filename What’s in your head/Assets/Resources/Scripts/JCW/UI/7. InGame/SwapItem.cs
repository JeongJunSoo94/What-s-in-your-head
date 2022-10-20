using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class SwapItem : MonoBehaviour
    {
        [Header("HP UI�� ���� �� �з����� ����")] [SerializeField] float offset = 150f;
        [Header("�������� 1")] [SerializeField] Sprite firstWeapon1;
                             [SerializeField] Sprite secondWeapon1;
        [Header("�������� 2")] [SerializeField] Sprite firstWeapon2;
                             [SerializeField] Sprite secondWeapon2;
        [Header("�������� 3")] [SerializeField] Sprite firstWeapon3;
                             [SerializeField] Sprite secondWeapon3;
        [Header("�������� 4")] [SerializeField] Sprite firstWeapon4;
                             [SerializeField] Sprite secondWeapon4;

        int curStageIndex;
        PhotonView photonView;
        bool isSwap = false;
        Image thisImg;

        // ù��° ���⸦ ����� ���� �ι�° ���⸦ ����� ���� �̹���
        Sprite firstWeapon;
        Sprite secondWeapon;

        // ���� �ڶ�����
        bool isNella;

        private void Awake()
        {
            curStageIndex = GameManager.Instance.curStageIndex;
            photonView = GetComponent<PhotonView>();
            thisImg = GetComponent<Image>();
            if (GameManager.Instance.characterOwner.Count == 0)
                isNella = true;
            else
                isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];

            switch(curStageIndex)
            {
                case 1:
                    firstWeapon = firstWeapon1;
                    secondWeapon = secondWeapon1;
                    break;
                case 2:
                    firstWeapon = firstWeapon2;
                    secondWeapon = secondWeapon2;
                    break;
                case 3:
                    firstWeapon = firstWeapon3;
                    secondWeapon = secondWeapon3;
                    break;
                case 4:
                    firstWeapon = firstWeapon4;
                    secondWeapon = secondWeapon4;
                    break;
            }

            thisImg.sprite = firstWeapon;
        }


        public void SwapUI()
        {
            photonView.RPC(nameof(SwapUI_RPC), RpcTarget.AllViaServer);
        }

        [PunRPC]
        void SwapUI_RPC()
        {
            isSwap = !isSwap;
            thisImg.sprite = isSwap ? secondWeapon : firstWeapon;
        }

        public void MoveSideUI(bool isOn)
        {
            if (photonView==null)
                photonView = GetComponent<PhotonView>();
            photonView.RPC(nameof(MoveSideUI_RPC), RpcTarget.AllViaServer, isOn);
        }

        void MoveSideUI_RPC(bool isOn)
        {
            Debug.Log("���ѳ��� ����");
            Vector2 ogPos = GetComponent<RectTransform>().anchoredPosition;
            float tempOffset = isOn ? offset : -offset;
            Vector2 movePos = isNella ? new Vector2(ogPos.x + tempOffset, ogPos.y) : new Vector2(ogPos.x - tempOffset, ogPos.y);
            GetComponent<RectTransform>().anchoredPosition = movePos;
        }
    }
}
