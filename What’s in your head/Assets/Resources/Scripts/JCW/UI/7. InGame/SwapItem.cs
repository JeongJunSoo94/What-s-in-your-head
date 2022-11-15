using System.Collections.Generic;
using JCW.AudioCtrl;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class SwapItem : MonoBehaviour
    {
        [Header("스테이지 3")] [SerializeField] List<Sprite> Weapon;

        int curStageIndex;
        PhotonView photonView;
        Image thisImg;


        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            thisImg = GetComponent<Image>();

            thisImg.sprite = Weapon[0];
        }

        public void SetSwap(int index)
        {
            SoundManager.Instance.PlayEffect("WeaponChange");
            photonView.RPC(nameof(SetSwap_RPC), RpcTarget.AllViaServer, index);
        }

        [PunRPC]
        void SetSwap_RPC(int index)
        {            
            thisImg.sprite = Weapon[index];
        }
    }
}
