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
        protected bool isActivated = false;
        protected PhotonView photonView;

        [SerializeField] List<InteractingTargetObject> interactingTargetObjects;

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
                photonView.RPC(nameof(SetInteractable), RpcTarget.AllViaServer, false);
        }

        [PunRPC]
        protected void SetInteractable(bool isOn)
        {
            isInteractable = isOn;
        }

        [PunRPC]
        protected void SetActivation(bool isOn)
        {
            isActivated = isOn;
        }

        [PunRPC]
        protected virtual void SendActivation()
        {
            foreach(var obj in interactingTargetObjects)
            {
                obj.RecieveActivation(isActivated);
            }
        }
    }
}
