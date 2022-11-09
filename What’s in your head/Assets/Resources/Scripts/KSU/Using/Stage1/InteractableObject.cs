using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction
{
    [RequireComponent(typeof(PhotonView))]
    public class InteractableObject : MonoBehaviour
    {
        [SerializeField] protected GameObject detectingTrigger;
        public float detectingRange = 20f;
        public float interactableRange = 5f;

        protected bool isInteractable = true; 
        protected PhotonView photonView;

        // Start is called before the first frame update
        virtual protected void Awake()
        {
            photonView = GetComponent<PhotonView>();
            SetDetectingTrigger();
        }

        virtual protected void SetDetectingTrigger()
        {
            detectingTrigger.transform.localScale = Vector3.one * detectingRange * 2f;
        }

        public virtual void StartInteraction()
        {
            if (isInteractable)
            {
                if(photonView.IsMine)
                    photonView.RPC(nameof(SetInteractable), RpcTarget.AllViaServer, false);
                // 상호작용 시작
            }
        }

        [PunRPC]
        protected void SetInteractable(bool isOn)
        {
            isInteractable = isOn;
        }
    }
}
