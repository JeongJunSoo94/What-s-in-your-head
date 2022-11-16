using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class SwitchingTrampolin : MonoBehaviour
    {
        [SerializeField] List<GameObject> trampolins;

        private void Start()
        {
            trampolins[0].SetActive(true);
            trampolins[1].SetActive(false);
        }

        void SwitchTrampolin()
        {
            for (int i = 0; i < trampolins.Count; ++i)
            {
                trampolins[i].SetActive(!trampolins[i].activeSelf);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
                SwitchTrampolin();
        }
    }
}
