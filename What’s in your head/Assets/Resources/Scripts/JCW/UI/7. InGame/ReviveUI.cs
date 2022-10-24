using Photon.Pun;
using UnityEngine;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class ReviveUI : MonoBehaviour
    {
        bool isNella;
        PhotonView photonView;

        void Awake()
        {
           
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            photonView = GetComponent<PhotonView>();
        }

        private void OnEnable()
        {
            if(photonView.IsMine)
                photonView.RPC(nameof(TurnOnUI_RPC), RpcTarget.AllViaServer, (bool)GameManager.Instance.isAlive[isNella]);
        }
        [PunRPC]
        private void TurnOnUI_RPC(bool _isAlive)
        {
            if (_isAlive)
            {
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
}

