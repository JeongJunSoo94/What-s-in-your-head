using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseTracer : MonoBehaviour
{
    private GameObject Base;
    private NavMeshAgent agent;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Base = WIYH_Manager.Instance.base_main;
        agent.SetDestination(Base.transform.position);
    }
}
