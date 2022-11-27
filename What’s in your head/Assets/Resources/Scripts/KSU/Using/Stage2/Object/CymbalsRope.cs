using JCW.AudioCtrl;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.AutoAim.Object.Stage2
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    public class CymbalsRope : CymbalsReciever
    {
        PhotonView pv;
        AudioSource audioSource;
        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            pv = GetComponent<PhotonView>();
            audioSource = GetComponent<AudioSource>();
            SoundManager.Set3DAudio(pv.ViewID, audioSource, 1f, 25f);
        }

        [PunRPC]
        protected override void SetActive()
        {
            isActivated = true;
            SendInfo();
            animator.SetBool("isCut", true);
            SoundManager.Instance.Play3D("S2_Steady_RopeCut", pv.ViewID);
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