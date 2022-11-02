using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using JCW.Spawner;
using Photon.Pun;
using JCW.Network;
using YC.Photon;

namespace KSU
{
    [RequireComponent(typeof(PhotonView))]
    public class MonsterSpawner : MonoBehaviour
    {
        [Header("������ ������ ���"), SerializeField] string spawnedObjectDirectory;
        [Header("_______���� ���� ��_______")]
        [Header("Ȱ��ȭ �� ó�� ���� �����ϴ� �� �ɸ��� �ð�"), SerializeField] float firstSpawnDelayTime;
        [Header("������ �� Ÿ��"), SerializeField] float spawnDelayTime;
        [Header("�� ���� ������ ���� ��"), Range(0,9), SerializeField] int onceSpawnNum;
        [Header("������ ������ �� ��"), Range(0,20), SerializeField] int maxSpawnNum;
        List<GameObject> monsterList = new();
        Queue<GameObject> DespawnedMonsterQueue = new();

        int curSpawnCount;

        [Header("���� ���� ��ġ ����Ʈ(10�� ����)"), SerializeField] List<Vector3> spawnPosition;
        [Header("���� ���� �������� �ִ� 9�ڸ�(�ڸ� ��: �� ���� �����ϴ� ���� ��) ������ ���� ����Ʈ"), SerializeField] List<int> spawnPattern; // ���� �迭�� �� ��Ұ� 6�ڸ� int�� ����(0 ~ 3)��, �����Ҷ� �� ��ġ�� �´� ������ ����
        int patternCount;

        [SerializeField] int spawnSeed;// ���� �迭�� ���̷� ������ �� �������� �迭�� ó�� �ε����� ����, SerializeField�� �׽�Ʈ�� �ϼ� �� ���� ����

        bool isGameEnd = false;
        PhotonView photonView;

        // Start is called before the first frame update
        private void Awake()
        {
            photonView = PhotonView.Get(this);
            StartCoroutine(nameof(InitSpwanerCo));
        }
        
        IEnumerator InitSpwanerCo()
        {
            yield return new WaitUntil(() => GameManager.Instance != null);
            yield return new WaitUntil(() => GameManager.Instance.characterOwner.Count >= 1);
            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
            {
                InitSpawner();
                StartSpawn();
            }
           //     while (true)
           // {
           //     yield return new WaitForSeconds(2f);
           //     if(GameManager.Instance != null)
           //     {
           //         if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
           //         {
           //             InitSpawner();
           //             StartSpawn();
           //             yield break;
           //         }
           //         else
           //         {
           //             yield break;
           //         }
           //     }
           //     else
           //     {
           //         Debug.Log("��� null");
           //     }
           // }
        }

        // Update is called once per frame
        void Update()
        {
            if(isGameEnd)
            {
                this.gameObject.SetActive(false);
            }
        }

        public void StartSpawn()
        {
            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
            {
                ResetSpawner();
                StartCoroutine(nameof(DelayStart));
            }
        }

        public void EndSpawn()
        {
            if (GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient])
            {
                StopAllCoroutines();
                StopSpawn();
            }
        }

        void InitSpawner()
        {
            curSpawnCount = 0;
            patternCount = 0;
            for (int i = 0; i < maxSpawnNum; ++i)
            {
                GameObject monster = PhotonNetwork.Instantiate(spawnedObjectDirectory, this.transform.position, this.transform.rotation, 0);
                monster.transform.parent = this.gameObject.transform;
                monster.SetActive(false);
                monsterList.Add(monster);
                DespawnedMonsterQueue.Enqueue(monster);
            }
        }

        void ResetSpawner()
        {
            curSpawnCount = 0;
            patternCount = 0;
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
                if (curSpawnCount < maxSpawnNum)
                {
                    if (patternCount > spawnPattern.Count)
                        patternCount = 0;

                    int spawnPatternNum = spawnPattern[patternCount];
                    for (int i = 0; i < onceSpawnNum; ++i)
                    {
                        if (curSpawnCount == maxSpawnNum)
                            break;
                        photonView.RPC(nameof(Spawn), RpcTarget.AllViaServer, spawnPosition[(spawnPatternNum % 10) % spawnPosition.Count]);
                        //Respawn(spawnPosition[(spawnPatternNum % 10) % spawnPosition.Count]);
                        spawnPatternNum /= 10;
                    }
                    yield return new WaitForSeconds(spawnDelayTime);
                }
                else
                    yield return new WaitUntil(() => curSpawnCount < maxSpawnNum);
            }
        }

        void Respawn(Vector3 spawnPosition)
        {
            curSpawnCount++;
            GameObject monster = DespawnedMonsterQueue.Dequeue();
            monster.transform.position = spawnPosition;
            monster.SetActive(true);
        }
        [PunRPC]
        void Spawn(Vector3 spawnPosition)
        {
            Respawn(spawnPosition);
        }

        public void Despawn(GameObject monster)
        {
            monster.SetActive(false);
            monster.transform.position = this.transform.position;
            DespawnedMonsterQueue.Enqueue(monster);
            curSpawnCount--;
        }

        void StopSpawn()
        {
            for (int i = 0; i < maxSpawnNum; ++i)
            {
                if(monsterList[i].activeSelf)
                {
                    Despawn(monsterList[i]);
                }
            }
        }
    }
}
