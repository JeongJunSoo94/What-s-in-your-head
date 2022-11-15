using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object.Stage1
{
    public class FerrisWheel : MonoBehaviour
    {
        Transform[] cabins;
        bool isStart = false;
        [Header("회전 속도")] [SerializeField] float speed = 10f;
        
        private void Awake()
        {
            cabins = transform.GetComponentsInChildren<Transform>();
        }
        

        void FixedUpdate()
        {
            if (!isStart)
                return;
            cabins[0].Rotate(Vector3.forward, speed * Time.fixedDeltaTime, Space.World);
            for (int i=1 ; i< cabins.Length ; ++i)
            {
                cabins[i].LookAt(cabins[i].position + Vector3.forward);
            }
        }
    }

}
