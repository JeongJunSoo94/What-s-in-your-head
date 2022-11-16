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
            if(detectingTrigger != null)
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
                isInteractable = false;
                photonView.RPC(nameof(SetInteractable), RpcTarget.AllViaServer, false);
                photonView.RPC(nameof(SetActivation), RpcTarget.AllViaServer, !isActivated);
                photonView.RPC(nameof(SendActivation), RpcTarget.AllViaServer);
            }
        }

        [PunRPC]
        protected void SetInteractable(bool isOn)
        {
            isInteractable = isOn;
        }

        [PunRPC]
        protected virtual void SetActivation(bool isOn)
        {
            isActivated = isOn;
        }

        [PunRPC]
        protected virtual void SendActivation()
        {
            if (interactingTargetObjects.Count > 0)
            {
                foreach (var obj in interactingTargetObjects)
                {
                    obj.RecieveActivation(this, isActivated);
                }
            }
        }
    }
}
