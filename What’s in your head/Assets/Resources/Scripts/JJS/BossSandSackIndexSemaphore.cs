using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS
{
    public class BossSandSackIndexSemaphore : MonoBehaviour
    {
        public Flashlight boss;

        public int index;
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
                boss.index = index;
        }
    }
}
