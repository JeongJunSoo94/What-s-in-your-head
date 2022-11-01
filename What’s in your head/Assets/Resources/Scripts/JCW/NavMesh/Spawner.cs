using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Spawner
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] [Header("���� �ð�")] [Range(0.0f, 10.0f)] private float spawnTime = 2.5f;
        [SerializeField] [Header("������ ������Ʈ")] public GameObject obj = null;
        [Header("�ִ� ������Ʈ ��")] [Range(0, 50)] public int count = 20;

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

        // �̸� ������Ʈ�� �����ؼ� �ڽ����� ��Ƶ�.
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