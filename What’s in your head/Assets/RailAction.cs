using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.Utility;

public class RailAction : MonoBehaviour
{
    [SerializeField, Tooltip("돌리 카트")] GameObject cart;
    Cinemachine.CinemachineSmoothPath track;

    // Start is called before the first frame update
    void Start()
    {
        track = GetComponentInChildren<Cinemachine.CinemachineSmoothPath>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //void CreateCart()
    //{
    //    foreach(var wayPoint in track.m_Waypoints)
    //    {
    //        wayPoint.position;
    //
    //        Vector3.Distance(wayPoint.position, )
    //    }
    //}
}
