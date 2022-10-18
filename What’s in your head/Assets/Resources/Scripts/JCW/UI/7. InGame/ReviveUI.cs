using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YC.Camera_;
using YC.Camera_Single;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class ReviveUI : MonoBehaviour
    {
        Camera mainCamera;
        bool isNella;
        PhotonView photonView;

        void Awake()
        {
            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
                mainCamera = transform.parent.transform.parent.GetComponent<CameraController>().FindCamera(); // 멀티용
            else
                mainCamera = transform.parent.transform.parent.GetComponent<CameraController_Single>().FindCamera(); // 싱글용            

            GetComponent<Canvas>().worldCamera = mainCamera;
            GetComponent<Canvas>().planeDistance = 0.15f;

            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];

            photonView = GetComponent<PhotonView>();
        }

        private void OnEnable()
        {
            if(photonView.IsMine)
            {
                if ((bool)(GameManager.Instance.isAlive[isNella]))
                    transform.GetChild(1).gameObject.SetActive(true);
                else
                    transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                if ((bool)(GameManager.Instance.isAlive[isNella]))
                    transform.GetChild(0).gameObject.SetActive(true);
                else
                    transform.GetChild(1).gameObject.SetActive(true);
            }
            

            //GetComponent<PhotonView>().RPC(nameof(TurnOnUI_RPC), RpcTarget.AllViaServer);
        }
        [PunRPC]
        private void TurnOnUI_RPC()
        {
            if ((bool)(GameManager.Instance.isAlive[isNella]))
                transform.GetChild(1).gameObject.SetActive(true);
            else
                transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}

