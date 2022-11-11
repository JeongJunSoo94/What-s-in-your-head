using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Object.Stage2
{
    public class CymbalsRope : CymbalsReciever
    {
        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
        }

        // Update is called once per frame
        //void FixedUpdate()
        //{
        //    if(isActivated)
        //    {
        //        CutOff();
        //    }
        //}
        //void CutOff()
        //{
        //    ropeSpeed += gravity * Time.fixedDeltaTime;
        //    upperRope.transform.position = upperRope.transform.position - Vector3.up * ropeSpeed;
        //    underRope.transform.position = underRope.transform.position - Vector3.up * ropeSpeed;
        //}

        [PunRPC]
        protected override void SetActive()
        {
            isActivated = true;
            SendInfo();
            animator.SetBool("isCut", true);
            StartCoroutine(nameof(DelayActivation));
        }

        protected override IEnumerator DelayActivation()
        {
            yield return new WaitForSeconds(activationDelayTime);
            Destroy(this.gameObject);
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