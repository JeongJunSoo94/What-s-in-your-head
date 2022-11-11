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

        bool isStart = false;

        private void Awake()
        {
            photonView = PhotonView.Get(this);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if(other.GetComponent<PlayerState>().isMine && !isStart)
                {
                    isStart = true;
                    photonView.RPC(nameof(Loading), RpcTarget.AllViaServer);
                }
            }
        }

        [PunRPC]
        void Loading()
        {
            isStart = true;
            if (GameManager.Instance.curStageType == 1)
                ++GameManager.Instance.curStageType;
            else
            {
                --GameManager.Instance.curStageType;
                ++GameManager.Instance.curStageIndex;
            }
            LoadingUI.SetActive(true);
        }
    }
}

