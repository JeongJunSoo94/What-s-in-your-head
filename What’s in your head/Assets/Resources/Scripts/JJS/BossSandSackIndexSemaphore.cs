using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS
{
    public class BossSandSackIndexSemaphore : MonoBehaviour
    {
        public Flashlight boss;

        public int index;
        private void Awake()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            boss.index = index;
        }
    }
}
