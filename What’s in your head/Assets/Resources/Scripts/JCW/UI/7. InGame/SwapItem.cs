using System.Collections.Generic;
using KSU;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class SwapItem : MonoBehaviour
    {
        [Header("스테이지 2")] [SerializeField] List<Sprite> Weapon2;
        [Header("스테이지 3")] [SerializeField] List<Sprite> Weapon3;

        int curStageIndex;
        PhotonView photonView;
        Image thisImg;

        // 무기 목록
        readonly Dictionary<int, List<Sprite>> weaponList = new();

        private void Awake()
        {
            curStageIndex = GameManager.Instance.curStageIndex;
            if (curStageIndex != 2 && curStageIndex != 3)
            {
                this.gameObject.SetActive(false);
                return;
            }

            photonView = GetComponent<PhotonView>();
            thisImg = GetComponent<Image>();

            weaponList.Add(2, Weapon2);
            weaponList.Add(3, Weapon3);

            thisImg.sprite = weaponList[curStageIndex][0];
        }

        public void SetSwap(int index)
        {
            photonView.RPC(nameof(SetSwap_RPC), RpcTarget.AllViaServer, index);
        }

        [PunRPC]
        void SetSwap_RPC(int index)
        {
            thisImg.sprite = weaponList[curStageIndex][index];
        }
    }
}
