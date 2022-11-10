using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Object.Stage2
{
    public class CymbalsButton : CymbalsReciever
    {
        Animator animator;
        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }
        [PunRPC]
        protected override void SetActive()
        {
            isActivated = true;
            SendInfo();
            StartCoroutine(nameof(DelayActivation));
            animator.SetBool("isActivated", true);
        }
        protected override IEnumerator DelayActivation()
        {
            yield return new WaitForSeconds(activationDelayTime);
            isActivated = false;
            animator.SetBool("isActivated", false);
            SendInfo();
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Cymbals") && !isActivated)
            {
                photonView.RPC(nameof(SetActive), RpcTarget.AllViaServer);
            }
        }
    }
}
