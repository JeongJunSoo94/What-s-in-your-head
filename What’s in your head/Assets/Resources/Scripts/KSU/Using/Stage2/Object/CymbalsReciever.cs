using KSU.Object.Stage2;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Object.Stage2
{
    [RequireComponent(typeof(PhotonView))]
    public class CymbalsReciever : AutoAimTargetObject
    {
        protected bool isActivated = false;
        protected PhotonView photonView;
        protected Animator animator;
        [SerializeField] protected float activationDelayTime = 5f;
        [SerializeField] protected List<LinkedObjectWithReciever> linkedObjects;


        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            photonView = PhotonView.Get(this);
            animator = GetComponent<Animator>();
        }

        protected void SendInfo()
        {
            foreach (var obj in linkedObjects)
            {
                obj.RecieveInfo(isActivated);
            }
        }
        [PunRPC]
        protected virtual void SetActive()
        {
            isActivated = true;
            SendInfo();
            StartCoroutine(nameof(DelayActivation));
        }

        protected virtual IEnumerator DelayActivation()
        {
            yield return new WaitForSeconds(activationDelayTime);
            isActivated = false;
            SendInfo();
        }
    }
}