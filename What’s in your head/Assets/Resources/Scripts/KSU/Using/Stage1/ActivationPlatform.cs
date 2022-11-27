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
            StartCoroutine(nameof(WaitForPlayer));
        }

        public override void RecieveActivation(InteractableObject sender, bool isActive)
        {
            if (isActive)
            {
                photonView.RPC(nameof(SetActiveSelf), RpcTarget.AllViaServer);
            }
        }

        IEnumerator WaitForPlayer()
        {
            yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene());
            this.gameObject.SetActive(false);
            yield break;
        }

    [PunRPC]
        void SetActiveSelf()
        {
            this.gameObject.SetActive(true);
        }
    }
}
