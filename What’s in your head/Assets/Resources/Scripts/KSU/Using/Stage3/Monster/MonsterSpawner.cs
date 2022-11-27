using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using JCW.Spawner;
using Photon.Pun;
using JCW.Network;
using YC.Photon;
using JCW.UI.Options.InputBindings;
using KSU.AutoAim.Object.Monster;
using KSU.Object.Interaction;

namespace KSU
{
    [RequireComponent(typeof(PhotonView))]
    public class MonsterSpawner : MonoBehaviour
    {
        [Header("스폰할 몬스터의 경로"), SerializeField] string spawnedObjectDirectory;
        [Header("_______변경 가능 값_______")]
        [Header("활성화 후 처음 몬스터 스폰하는 데 걸리는 시간"), SerializeField] float firstSpawnDelayTime;
        [Header("리스폰 쿨 타임"), SerializeField] float spawnDelayTime;
        [Header("한 번에 스폰할 몬스터 수"), Range(0,9), SerializeField] int onceSpawnNum;
        [Header("스폰할 몬스터의 총 수"), Range(0,20), SerializeField] int maxSpawnNum;
        List<GameObject> monsterList = new();
        Queue<GameObject> DespawnedMonsterQueue = new();

        int curSpawnCount;

        [Header("몬스터 스폰 위치 리스트(10개 이하)"), SerializeField] List<Transform> spawnPosition;
        [Header("스폰 패턴 무작위의 최대 9자리(자릿 수: 한 번에 스폰하는 몬스터 수) 이하의 숫자 리스트"), SerializeField] List<int> spawnPattern; // 패턴 배열은 한 요소가 6자리 int형 숫자(0 ~ 3)고, 스폰할때 그 위치에 맞는 곳에서 스폰
        int patternCount;

        [SerializeField] int spawnSeed;// 패턴 배열의 길이로 나누고 그 나머지는 배열의 처음 인덱스를 뜻함, SerializeField는 테스트용 완성 후 지울 예정

        bool isGameEnd = false;
        PhotonView photonView;

        [SerializeField] GameObject alice;
        [SerializeField] bool isDefense = true;
        bool isStartCoroutine = false;

        // Start is called before the first frame update
        private void Awake()
        {
            photonView = PhotonView.Get(this);
            spawnSeed = GameManager.Instance.randomSeed;
            StartCoroutine(nameof(WaitForPlayer));
        }

        IEnumerator InitSpwanerCo()
        {
            isStartCoroutine = true;
            InitSpawner();
            StartSpawn();
            yield break;
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
            if (PhotonNetwork.IsMasterClient)
            {
                ResetSpawner();
                StartCoroutine(nameof(DelayStart));
            }
        }

        public void EndSpawn()
        {
            if (PhotonNetwork.IsMasterClient)
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
                ActivationChecker checker;
                if (monster.TryGetComponent<ActivationChecker>(out checker))
                    checker.AddInteractingTargetObjects();
                DefenseMonster monsterBehavior = monster.GetComponent<DefenseMonster>();
                monsterBehavior.Disappear();
                if (alice != null)
                {
                    monsterBehavior.SetTargetObject(alice.transform);
                    monsterBehavior.SetTopViewMode();
                }
                monsterList.Add(monster);
                DespawnedMonsterQueue.Enqueue(monster);
            }
        }
        void ResetSpawner()
        {
            curSpawnCount = 0;
            patternCount = 0;
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
                if (curSpawnCount < maxSpawnNum)
                {
                    if (patternCount >= spawnPattern.Count)
                        patternCount = 0;

                    int spawnPatternNum = spawnPattern[patternCount];
                    for (int i = 0; i < onceSpawnNum; ++i)
                    {
                        if (curSpawnCount == maxSpawnNum)
                            break;
                        photonView.RPC(nameof(Spawn), RpcTarget.AllViaServer, spawnPosition[(spawnPatternNum % 10) % spawnPosition.Count].position);
                        //Respawn(spawnPosition[(spawnPatternNum % 10) % spawnPosition.Count]);
                        spawnPatternNum /= 10;
                    }
                    patternCount++;
                    if (isDefense)
                        yield return new WaitForSeconds(spawnDelayTime);
                    else
                        yield break;
                }
                else
                    yield return new WaitUntil(() => curSpawnCount < maxSpawnNum);
            }
        }

        void Respawn(Vector3 spawnPosition)
        {
            curSpawnCount++;
            if(PhotonNetwork.IsMasterClient)
            {
                GameObject monster = DespawnedMonsterQueue.Dequeue();
                monster.GetComponent<DefenseMonster>().ActiveMonster(spawnPosition);
            }
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
            if(PhotonNetwork.IsMasterClient)
            {
                DespawnedMonsterQueue.Enqueue(monster);
            }
            curSpawnCount--;
            if (curSpawnCount < 0)
                curSpawnCount = 0;
        }

        void StopSpawn()
        {
            if(PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < maxSpawnNum; ++i)
                {
                    if (monsterList[i].activeSelf)
                    {
                        monsterList[i].GetComponent<DefenseMonster>().Disappear();
                    }
                }
            }
        }

        protected IEnumerator WaitForPlayer()
        {
            yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene());

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(nameof(InitSpwanerCo));
            }

            yield break;
        }
    }
}
