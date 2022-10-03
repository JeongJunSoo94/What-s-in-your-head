using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] [Header("스폰 시간")] [Range(0.0f,10.0f)] private float spawnTime = 2.5f;
    [SerializeField] [Header("스폰할 몬스터")] private GameObject tracer = null;
    [SerializeField] [Header("최대 몬스터 수")] [Range(0,50)] private int count = 20;

    Queue<GameObject> monsterQueue;
    /*[HideInInspector]*/   public int spawnCount = 0;

    private Vector3 BasePos;

    void Start()
    {
        monsterQueue = new Queue<GameObject>();
        for (int i = 0 ; i<count ; ++i)
        {
            GameObject spawned = Instantiate(tracer, this.transform);
            spawned.SetActive(false);
            monsterQueue.Enqueue(spawned);
        }
        StartCoroutine(nameof(Spawn));
    }

    public void Despawn(GameObject monster)
    {
        --spawnCount;        
        monster.SetActive(false);        
        monsterQueue.Enqueue(monster);
    }
    IEnumerator Spawn()
    {
        while (true)
        {
            if (spawnCount < count)
            {
                yield return new WaitForSeconds(spawnTime);
                ++spawnCount;
                monsterQueue.Dequeue().SetActive(true);

            }
            else
                yield return new WaitUntil(() => spawnCount < count);
        }

    }

}
