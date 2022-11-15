using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YC.Effect
{
    public class DashEffect : MonoBehaviour
    {
        GameObject particleObj;

        private void Awake()
        {
            particleObj = this.gameObject;
        }
        public void PlayEffect()
        {
            particleObj.SetActive(true);
        }

      

        public void StopEffect()
        {
            particleObj.SetActive(false);
        }
    }
}
