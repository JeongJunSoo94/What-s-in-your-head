using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS
{
    public class Flashlight : MonoBehaviour
    {
        public GameObject light;
        public ConeFindTarget finder;
        private void Awake()
        {
        }
        private void Update()
        {

        }

        public void LightEnable(bool enable)
        {
            light.SetActive(enable);
        }

        public bool TargetCheck()
        {
            return finder.DiscoveryTargetBool() ? true : false;
        }
    }

}
