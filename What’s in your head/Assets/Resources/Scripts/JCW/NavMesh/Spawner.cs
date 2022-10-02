using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] [Header("���� �ð�")] private float spawnTime = 2.5f;
    [SerializeField] [Header("������ ����")] private GameObject tracer = null;


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
