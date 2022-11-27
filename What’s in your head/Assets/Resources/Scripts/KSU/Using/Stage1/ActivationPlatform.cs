using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object.Interaction
{
    [RequireComponent(typeof(PhotonView))]
    public class ActivationPlatform : InteractingTargetObject
    {
        PhotonView photonView;
        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
        }

        public override void RecieveActivation(InteractableObject sender, bool isActive)
        {
            if (isActive)
            {
                photonView.RPC(nameof(SetActiveSelf), RpcTarget.AllViaServer);
            }
        }

        [PunRPC]
        void SetActiveSelf()
        {
            this.gameObject.SetActive(true);
        }
    }
}
