using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.Utility;

public class RailAction : MonoBehaviour
{
    [SerializeField, Tooltip("돌리 카트")] GameObject cart;
    Cinemachine.CinemachineSmoothPath track;
    public GameObject box;

    // Start is called before the first frame update
    void Start()
    {
        track = GetComponentInChildren<Cinemachine.CinemachineSmoothPath>();
        MakeCollider();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void MakeCollider()
    {
        if (track.m_Waypoints.Length > 1)
        {
            for (int i = 1; i < track.m_Waypoints.Length; i++)
            {
                Vector3 start = track.m_Waypoints[i - 1].position;
                Vector3 end = track.m_Waypoints[i].position;
                float length = Vector3.Distance(start, end);

                GameObject obj = Instantiate(box, ((start + end) / 2), Quaternion.Euler(Vector3.zero));
                obj.layer = LayerMask.NameToLayer("Interactable");
                obj.transform.parent = gameObject.transform;
                obj.transform.localScale = new Vector3(1, 1, length);
                obj.transform.LookAt(end);
                obj.AddComponent<BoxCollider>();
            }
        }
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
