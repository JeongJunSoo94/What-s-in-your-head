using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] [Header("스폰 시간")] private float spawnTime = 2.5f;
    [SerializeField] [Header("스폰할 몬스터")] private GameObject tracer = null;


    void Start()
    {
        StartCoroutine(Spawn(tracer));
    }       
    

    IEnumerator Spawn(GameObject prefab)
    {
        while(true)
        {
            yield return new WaitForSeconds(spawnTime);
            Instantiate(prefab, this.transform);
        }
        
    }
}
