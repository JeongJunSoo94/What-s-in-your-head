using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerTracer : MonoBehaviour
{
    private List<GameObject> Players;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Players = new List<GameObject>
        {
            WIYH_Manager.Instance.player1,
            WIYH_Manager.Instance.player2
        };
    }

    private void FixedUpdate()
    {
        agent.SetDestination(CalcDistTarget());
    }

    private Vector3 CalcDistTarget()
    {
        NavMeshPath[] meshPath = new NavMeshPath[] { new NavMeshPath(), new NavMeshPath() };
        agent.CalculatePath(Players[0].transform.position, meshPath[0]);
        agent.CalculatePath(Players[1].transform.position, meshPath[1]);

        
        int cornerLength1 = meshPath[0].corners.Length;
        int cornerLength2 = meshPath[1].corners.Length;

        // meshPath의 corners 개수는 출발지와 목적지를 제외하므로 2를 추가
        Vector3[] wayPoints1 = new Vector3[cornerLength1 + 2];
        Vector3[] wayPoints2 = new Vector3[cornerLength2 + 2];

        wayPoints1[0] = transform.position;        wayPoints2[0] = transform.position;
        wayPoints1[cornerLength1 + 1] = Players[0].transform.position;
        wayPoints2[cornerLength2 + 1] = Players[1].transform.position;

        float[] distance = new float[] { 0.0f, 0.0f };
        for (int i=0 ; i< cornerLength1 ; ++i)
        {
            wayPoints1[i + 1] = meshPath[0].corners[i];
            distance[0] += Vector3.Distance(wayPoints1[i], wayPoints1[i + 1]);
        }
        for (int i=0 ; i< cornerLength2 ; ++i)
        {
            wayPoints2[i + 1] = meshPath[1].corners[i];
            distance[1] += Vector3.Distance(wayPoints2[i], wayPoints2[i + 1]);
        }

        if (distance[0] == 0)
            distance[0] = 10000f;
        if (distance[1] == 0)
            distance[1] = 10000f;

        //return Players[0].transform.position;        
        return distance[0] > distance[1] ? Players[1].transform.position : Players[0].transform.position;

    }
}
