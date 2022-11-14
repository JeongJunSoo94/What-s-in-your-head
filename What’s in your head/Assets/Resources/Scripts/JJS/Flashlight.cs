using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using JCW.Spawner;
using JCW.Object;

namespace JJS
{
    [RequireComponent(typeof(PhotonView))]
    public class Flashlight : MonoBehaviour
    {
        GameObject lightObj;
        public Light spot;
        public Light directional;
        public Light point;
        public ConeFindTarget finder;
        public ConeFindTarget finderSpot;
        GameObject sandSpawner;
        public Spawner spawner;
        public GameObject bullet;
        public int bulletCount = 0;
        public float attackTime;
        private void Awake()
        {
            SetCharacterGameObject(gameObject, out lightObj, "Light");
            spot = lightObj.transform.GetChild(0).GetComponent<Light>();
            directional = lightObj.transform.GetChild(1).GetComponent<Light>();
            point = lightObj.transform.GetChild(2).GetComponent<Light>();
            InitSpawner();
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

        [PunRPC]
        public void SandAttack(string name)
        {
            foreach (var obj in finder.targetObj)
            {
                if (obj.name == name)
                {
                }
            }
        }

   

        public void InitSpawner()
        {
            if (sandSpawner == null)
            {
                sandSpawner = new GameObject("SandSpawner");
                sandSpawner.AddComponent<Spawner>();
            }
            spawner = sandSpawner.GetComponent<Spawner>();
            InitSand();
            spawner.obj = bullet;
            spawner.count = bulletCount;
            spawner.spawnCount = 0;
        }

        public void InitSand()
        {
            bullet.GetComponent<FallKSU>();
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
