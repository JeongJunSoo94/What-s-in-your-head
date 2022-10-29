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
        [SerializeField] List<int> spawnPattern; // 패턴 배열은 한 요소가 6자리 int형 숫자(0 ~ 3)고, 스폰할때 그 위치에 맞는 곳에서 스폰
        int patternCount;

        Spawner spawner;

        [SerializeField] int spawnSeed;// 패턴 배열의 길이로 나누고 그 나머지는 배열의 처음 인덱스를 뜻함, SerializeField는 테스트용 완성 후 지울 예정

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

        public void SetSpawnSeed(int seed) // 이거 말고 게임매니저한테서 시드 받아오는 함수로 변경되는게 나아 보임
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
