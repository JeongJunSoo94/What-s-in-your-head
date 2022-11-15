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

        PhotonView photonView;

        public bool sandSackUse;

        GameObject sandSpawner;
        public Spawner spawner;
        public GameObject bullet;
        public int bulletCount = 0;
        public float attackTime;
        public int index;
        public GameObject posList;
        public List<Transform[]> targetList = new();
        public bool canAttack;

        private void Awake()
        {
            SetCharacterGameObject(gameObject, out lightObj, "Light");
            spot = lightObj.transform.GetChild(0).GetComponent<Light>();
            directional = lightObj.transform.GetChild(1).GetComponent<Light>();
            point = lightObj.transform.GetChild(2).GetComponent<Light>();
            photonView = GetComponent<PhotonView>();
            if (sandSackUse)
                InitializedSandSack();
        }

        void Update()
        {
            if (finder.targetObj.Count == 2)
            {
                if (GameManager.Instance.curPlayerHP == 0 && canAttack)
                {
                    SandAttack();
                       canAttack = false;
                }
            }
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
        public void SandAttack()
        {
            for (int i = 1; i < targetList[index].Length; i++)
            {
                GameObject bullet = spawner.Respawn(targetList[index][i].position);
                bullet.GetComponent<FallJJS>().StartCoroutineFall(attackTime);
            }
        }


        public void InitializedSandSack()
        {
            InitSpawner();
            SetList(posList);
            canAttack = true;
        }

        public void SetList(GameObject objectList)
        {
            for (int i = 0; i < objectList.transform.childCount; i++)
            {
                Transform[] allChildren = objectList.transform.GetChild(i).GetComponentsInChildren<Transform>();
                targetList.Add(allChildren);
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
            bullet.GetComponent<FallJJS>().spawner = spawner;
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
