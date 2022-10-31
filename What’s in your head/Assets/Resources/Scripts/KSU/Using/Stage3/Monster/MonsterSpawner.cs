using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using JCW.Spawner;

namespace KSU
{
    public class MonsterSpawner : MonoBehaviour
    {
        [SerializeField] float firstSpawnDelayTime;
        [SerializeField] float spawnDelayTime;
        [SerializeField] int onceSpawnNum;
        [SerializeField] int maxSpawnNum;
        [SerializeField] GameObject spawnedObject;

        [SerializeField] List<GameObject> spawnPosition;
        [SerializeField] List<int> spawnPattern; // ���� �迭�� �� ��Ұ� 6�ڸ� int�� ����(0 ~ 3)��, �����Ҷ� �� ��ġ�� �´� ������ ����
        int patternCount;

        Spawner spawner;

        [SerializeField] int spawnSeed;// ���� �迭�� ���̷� ������ �� �������� �迭�� ó�� �ε����� ����, SerializeField�� �׽�Ʈ�� �ϼ� �� ���� ����

        bool isGameEnd = false;

        // Start is called before the first frame update
        void Start()
        {
            var random = new System.Random(Guid.NewGuid().GetHashCode());
            spawner = GetComponentInChildren<Spawner>();
            spawner.count = maxSpawnNum;
            spawner.obj = spawnedObject;
        }

        // Update is called once per frame
        void Update()
        {
            if(isGameEnd)
            {
                this.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(nameof(DelayStart));
            patternCount = 0;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            spawner.StopSpawn();
        }

        public void SetSpawnSeed(int seed) // �̰� ���� ���ӸŴ������׼� �õ� �޾ƿ��� �Լ��� ����Ǵ°� ���� ����
        {
            spawnSeed = seed;
        }

        IEnumerator DelayStart()
        {
            yield return new WaitForSeconds(firstSpawnDelayTime);
            StartCoroutine(nameof(DelaySpawn));
        }

        IEnumerator DelaySpawn()
        {
            while (true)
            {
                if (spawner.spawnCount < maxSpawnNum)
                {
                    if (patternCount > spawnPattern.Count)
                        patternCount = 0;

                    int spawnPatternNum = spawnPattern[patternCount];
                    for (int i = 0; i < onceSpawnNum; ++i)
                    {
                        if (spawner.spawnCount == maxSpawnNum)
                            break;
                        
                        spawner.Respawn(spawnPosition[(spawnPatternNum % 10) % spawnPosition.Count].transform.position);
                        spawnPatternNum /= 10;
                    }
                    yield return new WaitForSeconds(spawnDelayTime);
                }
                else
                    yield return new WaitUntil(() => spawner.spawnCount < maxSpawnNum);
            }
        }
    }
}
