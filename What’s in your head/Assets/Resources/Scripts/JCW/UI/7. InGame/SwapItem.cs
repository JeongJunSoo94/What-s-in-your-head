using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class SwapItem : MonoBehaviour
    {
        [Header("스테이지 1")] [SerializeField] Sprite firstWeapon1;
                             [SerializeField] Sprite secondWeapon1;
        [Header("스테이지 2")] [SerializeField] Sprite firstWeapon2;
                             [SerializeField] Sprite secondWeapon2;
        [Header("스테이지 3")] [SerializeField] Sprite firstWeapon3;
                             [SerializeField] Sprite secondWeapon3;
        [Header("스테이지 4")] [SerializeField] Sprite firstWeapon4;
                             [SerializeField] Sprite secondWeapon4; 

        int curStageIndex;
        PhotonView photonView;
        bool isSwap = false;
        Image thisImg;

        // 첫번째 무기를 들었을 때와 두번째 무기를 들었을 때의 이미지
        Sprite firstWeapon;
        Sprite secondWeapon;

        // 현재 넬라인지
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

            switch (curStageIndex)
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

        private void Update()
        {
            if (!photonView.IsMine)
                return;
            if (Input.GetKeyDown(KeyCode.Tab))
                SwapUI();
        }

        private void LateUpdate()
        {
            //if (photonView.IsMine)
                //SetImg((bool)GameManager.Instance.isAlive[isNella]);
        }

        public void SetImg(bool isOn)
        {
            // Update에서 자주 부르는 RPC를 막기 위함
            if ((bool)GameManager.Instance.isAlive[isNella] == isOn)
                return;
            photonView.RPC(nameof(SetImg_RPC), RpcTarget.AllViaServer, isOn);
        }

        [PunRPC]
        void SetImg_RPC(bool isOn)
        {
            GetComponent<Image>().enabled = isOn;
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
    }
}
