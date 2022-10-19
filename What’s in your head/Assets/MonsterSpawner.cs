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

        int spawnSeed;// ���� �迭�� ���̷� ������ �� �������� �迭�� ó�� �ε����� ����, ���� �迭�� �� ��Ұ� 6�ڸ� int�� ����(0 ~ 3)��, �����Ҷ� �� ��ġ�� �´� ������ ����

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

                        //Respawn(�� �ȿ� ����, �õ忡 ���� ������);
                    }
                    yield return new WaitForSeconds(spawnDelayTime);
                }
                else
                    yield return new WaitUntil(() => spawner.spawnCount < maxSpawnNum);
            }
        }
    }
}
