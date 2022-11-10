using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Object.Stage2
{
    public class CymbalsRope : CymbalsReciever
    {
        [SerializeField] GameObject upperRope;
        [SerializeField] GameObject midRope;
        [SerializeField] GameObject underRope;

        public float ropeSpeed = 0f;
        public float gravity = -9.8f;
        public float terminalSpeed = 20f;

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(isActivated)
            {
                CutOff();
            }
        }
        void CutOff()
        {
            ropeSpeed += gravity * Time.fixedDeltaTime;
            upperRope.transform.position = upperRope.transform.position - Vector3.up * ropeSpeed;
            underRope.transform.position = underRope.transform.position - Vector3.up * ropeSpeed;
        }

        [PunRPC]
        protected override void SetActive()
        {
            isActivated = true;
            SendInfo();
            midRope.SetActive(false);
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