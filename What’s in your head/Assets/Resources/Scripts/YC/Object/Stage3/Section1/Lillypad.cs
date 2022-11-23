using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.AudioCtrl;
using Photon.Pun;

namespace YC_OBJ
{
    public class Lillypad : MonoBehaviour
    {
        AudioSource audioSource;
        PhotonView pv;

        void Start()
        {
            //if (!TryGetComponent<AudioSource>(out audioSource))
            //    audioSource = transform.GetChild(4).gameObject.AddComponent<AudioSource>();
            //SoundManager.Set3DAudio(pv.ViewID, audioSource, 0.5f, 10f, true);
        }

        void Update()
        {
        
        }
    }

}
