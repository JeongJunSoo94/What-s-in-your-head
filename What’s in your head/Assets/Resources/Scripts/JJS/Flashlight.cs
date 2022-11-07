using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS
{
    public class Flashlight : MonoBehaviour
    {
        public GameObject light;
        public Light spot;
        public Light point;
        public ConeFindTarget finder;
        public ConeFindTarget finderSpot;
        private void Awake()
        {
            spot = light.transform.GetChild(0).GetComponent<Light>();
            point = light.transform.GetChild(1).GetComponent<Light>();
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

        public bool SpotTargetCheck()
        {
            return finderSpot.DiscoveryTargetBool() ? true : false;
        }

        public void SetLightColor(Color color)
        {
            spot.color = color;
            point.color = color;
        }
    }

}
