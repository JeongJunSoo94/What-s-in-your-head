using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction
{
    public class ActivationChecker : InteractableObject
    {
        public void AddInteractingTargetObjects()
        {
            InteractionMediator mediator;
            if (transform.parent != null && transform.parent.parent != null)
            {
                if (transform.parent.parent.TryGetComponent<InteractionMediator>(out mediator))
                {
                    interactingTargetObjects.Add(mediator);
                    mediator.AddSender(this);
                }
            }
        }
        private void OnEnable()
        {
            if(interactingTargetObjects.Count > 0)
            {
                isActivated = false;
                photonView.RPC(nameof(SendActivation), RpcTarget.AllViaServer);
            }

        }
        private void OnDisable()
        {
            if(interactingTargetObjects.Count > 0)
            {
                isActivated = true;
                photonView.RPC(nameof(SendActivation), RpcTarget.AllViaServer);
            }
        }
    }
}