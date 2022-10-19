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
        [SerializeField] float onceSpawnNum;
        [SerializeField] float maxSpawnNum;

        Spawner spawner;

        int spawnSeed;// 패턴 배열의 길이로 나누고 그 나머지는 배열의 처음 인덱스를 뜻함, 패턴 배열은 한 요소가 6자리 int형 숫자(0 ~ 3)고, 스폰할때 그 위치에 맞는 곳에서 스폰

        bool isGameEnd = false;

        // Start is called before the first frame update
        void Start()
        {
            var random = new System.Random(Guid.NewGuid().GetHashCode());
            spawner = GetComponentInChildren<Spawner>();
        }

        // Update is called once per frame
        void Update()
        {
            if(isGameEnd)
            {
                StopAllCoroutines();
                spawner.StopSpawn();
            }
        }

        private void OnEnable()
        {
            StartCoroutine(nameof(DelayStart));
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
                    for(int i = 0; i < onceSpawnNum; ++i)
                    {
                        if (spawner.spawnCount == maxSpawnNum)
                            break;

                        //Respawn(이 안에 벡터, 시드에 따라 결정됨);
                    }
                    yield return new WaitForSeconds(spawnDelayTime);
                }
                else
                    yield return new WaitUntil(() => spawner.spawnCount < maxSpawnNum);
            }
        }
    }
}
