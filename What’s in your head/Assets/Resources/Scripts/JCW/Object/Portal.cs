using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class Portal : MonoBehaviour
    {
        [Header("·Îµù UI")] public GameObject LoadingUI;
        PhotonView photonView;

        private void Awake()
        {
            photonView = PhotonView.Get(this);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
                photonView.RPC(nameof(Loading), RpcTarget.AllViaServer);
        }

        [PunRPC]
        void Loading()
        {
            LoadingUI.SetActive(true);
        }
    }
}

