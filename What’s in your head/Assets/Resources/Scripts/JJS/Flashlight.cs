using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JJS
{
    public class Flashlight : MonoBehaviour
    {
        GameObject lightObj;
        public Light spot;
        public Light directional;
        public Light point;
        public ConeFindTarget finder;
        public ConeFindTarget finderSpot;

        private void Awake()
        {
            SetCharacterGameObject(gameObject,out lightObj, "Light");
            spot = lightObj.transform.GetChild(0).GetComponent<Light>();
            directional = lightObj.transform.GetChild(1).GetComponent<Light>();
            point = lightObj.transform.GetChild(2).GetComponent<Light>();
        }

        private void Update()
        {
            
        }

        public void LightEnable(bool enable)
        {
            lightObj.SetActive(enable);
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
            directional.color = color;
            point.color = color;
        }
        public void SetCharacterGameObject(GameObject findObject, out GameObject discoverObject, string findName)
        {
            discoverObject = null;
            Transform[] allChildren = findObject.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name == findName)
                {
                    discoverObject = child.gameObject;
                    return;
                }
            }
        }
    }

}
