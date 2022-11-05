using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Spawner
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] [Header("스폰 시간")] [Range(0.0f, 10.0f)] private float spawnTime = 2.5f;
        [SerializeField] [Header("스폰할 오브젝트")] public GameObject obj = null;
        [Header("최대 오브젝트 수")] [Range(0, 50)] public int count = 20;

        Queue<GameObject> objQueue;
        /*[HideInInspector]*/
        public int spawnCount = 0;

        public bool spawnType;

        void Start()
        {
            SpawnInit();
            if (spawnType)
            {
                RespawnCoroutine();
            }
        }
        public void RespawnCoroutine()
        {
            StartCoroutine(nameof(Spawn));
        }

        public GameObject Respawn(Vector3 pos)
        {
            GameObject gameObject = null;

            if (spawnCount < count)
            {
                ++spawnCount;
                gameObject = objQueue.Dequeue();
                gameObject.transform.position = pos;
                gameObject.SetActive(true);
            }
            return gameObject;
        }

        public GameObject Respawn(Vector3 pos, Quaternion rotation)
        {
            GameObject gameObject = null;

            if (spawnCount < count)
            {
                ++spawnCount;
                gameObject = objQueue.Dequeue();
                gameObject.transform.position = pos;
                gameObject.transform.rotation = rotation;
                gameObject.SetActive(true);
            }
            return gameObject;
        }

        // 미리 오브젝트를 생성해서 자식으로 담아둠.
        public void SpawnInit()
        {
            if (objQueue != null)
                return;
            objQueue = new Queue<GameObject>();
            for (int i = 0 ; i < count ; ++i)
            {
                GameObject spawned = Instantiate(obj, this.transform);
                spawned.SetActive(false);
                objQueue.Enqueue(spawned);
            }
        }

        public void Despawn(GameObject spawnObj)
        {
            --spawnCount;
            spawnObj.SetActive(false);
            objQueue.Enqueue(spawnObj);
        }
        IEnumerator Spawn()
        {
            while (true)
            {
                if (spawnCount < count)
                {
                    yield return new WaitForSeconds(spawnTime);
                    ++spawnCount;
                    objQueue.Dequeue().SetActive(true);
                }
                else
                    yield return new WaitUntil(() => spawnCount < count);
            }
        }

        public void StopSpawn()
        {
            StopCoroutine(nameof(Spawn));
            for (int i=0; i< transform.childCount; ++i)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    Despawn(transform.GetChild(i).gameObject);
                }
            }
        }
    }
}