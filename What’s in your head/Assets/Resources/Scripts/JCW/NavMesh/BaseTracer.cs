using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using JCW.Spawner;

public class BaseTracer : MonoBehaviour
{
    private Vector3 BasePos;
    private NavMeshAgent agent;
    private Spawner spawner;

    private void Awake()
    {
        spawner = this.transform.parent.gameObject.GetComponent<Spawner>();
        agent = GetComponent<NavMeshAgent>();        
    }
    private void OnEnable()
    {
        this.transform.position = this.transform.parent.position;
        agent.ResetPath();
        agent.SetDestination(BasePos);
        BasePos = GameManager.Instance.base_main.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Finish"))
        {
            Debug.Log("µµ´Þ");            
            spawner.Despawn(this.gameObject);
        }
    }

}
