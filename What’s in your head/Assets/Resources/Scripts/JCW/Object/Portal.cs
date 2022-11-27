using System.Collections;
using System.Collections.Generic;
using JCW.UI;
using KSU.Object.Interaction;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class Portal : InteractableObject
    {
        [Header("사용 키로 넘어갈 것인지")][SerializeField] bool doUseInteraction = true;

        bool canStart = true;
        int touchCount = 0;

        private void FixedUpdate()
        {
            if (isInteractable || !canStart || !doUseInteraction)
                return;

            canStart = false;
            if (touchCount >= 2)
                photonView.RPC(nameof(Loading), RpcTarget.AllViaServer);
            else
                canStart = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                if (!doUseInteraction)
                    photonView.RPC(nameof(Loading), RpcTarget.AllViaServer);
                else
                {
                    touchCount = touchCount >= 2 ? 2 : touchCount + 1;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((other.CompareTag("Nella") || other.CompareTag("Steady"))
                && other.GetComponent<PlayerState>().isMine)
            {
                if (doUseInteraction)
                    touchCount = touchCount < 0 ? 0 : touchCount - 1;
            }
        }

        [PunRPC]
        void Loading()
        {
            canStart = true;
            LoadingUI.Instance.gameObject.SetActive(true);
        }
    }
}

