using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.AudioCtrl;
using Photon.Pun;

namespace YC_OBJ
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PhotonView))]
    public class Lillypad : MonoBehaviour
    {
        AudioSource audioSource;
        PhotonView pv;
        string interactionTag1 = "Nella";
        string interactionTag2 = "Steady";

        private void Awake()
        {
            audioSource = this.gameObject.GetComponent<AudioSource>();
            pv = this.gameObject.GetComponent<PhotonView>();
        }

        private void Start()
        {
            SoundManager.Set3DAudio(pv.ViewID, audioSource, 1f, 10f, false);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(interactionTag1)
                || other.CompareTag(interactionTag2))
            {
                SoundManager.Instance.PlayIndirect3D_RPC("S3S1_Lillypad", pv.ViewID);
            }
        }
    }

}
