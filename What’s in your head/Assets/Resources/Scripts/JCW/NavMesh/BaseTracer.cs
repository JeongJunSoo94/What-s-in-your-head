using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseTracer : MonoBehaviour
{
    private GameObject Base;
    private NavMeshAgent agent;
    private Spawner spawner;

    void Start()
    {
        spawner = this.transform.parent.gameObject.GetComponent<Spawner>();
        agent = GetComponent<NavMeshAgent>();
        Base = WIYH_Manager.Instance.base_main;
        agent.SetDestination(Base.transform.position);        
    }

    private void FixedUpdate()
    {
        if(isActiveAndEnabled)
            CalcDistTarget();
    }

    private void CalcDistTarget()
    {        
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log("���� �Ÿ� : " + agent.remainingDistance + " / �� ��ġ : " + transform.position + " ��ǥ ��ġ : " + Base.transform.position);            
            spawner.Despawn(this.gameObject);
        }
        return;

    }
}
