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

        static public ReviveUI Instance = null;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
                Destroy(this.gameObject);


            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
                mainCamera = transform.parent.GetComponent<CameraController>().FindCamera(); // 멀티용
            else
                mainCamera = transform.parent.GetComponent<CameraController_Single>().FindCamera(); // 싱글용            

            GetComponent<Canvas>().worldCamera = mainCamera;
            GetComponent<Canvas>().planeDistance = 0.15f;
        }

        public void TurnOnUI()
        {
            GetComponent<PhotonView>().RPC(nameof(TurnOnUI_RPC), RpcTarget.All);
        }

        [PunRPC]
        private void TurnOnUI_RPC()
        {
            if (GameManager.Instance.isAlive)
                transform.GetChild(1).gameObject.SetActive(true);
            else
                transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}

