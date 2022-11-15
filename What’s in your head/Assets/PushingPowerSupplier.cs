using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction.Stage1
{
    public class PushingPowerSupplier : InteractableObject
    {
        private void OnTriggerEnter(Collider other)
        {
            photonView.RPC(nameof(SendActivation), Photon.Pun.RpcTarget.AllViaServer, true);
        }

        private void OnTriggerExit(Collider other)
        {
            photonView.RPC(nameof(SendActivation), Photon.Pun.RpcTarget.AllViaServer, false);
        }
    }
}
