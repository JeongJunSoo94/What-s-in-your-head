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
            StartCoroutine(nameof(WaitForPlayer));
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

        IEnumerator WaitForPlayer()
        {            
            yield return new WaitUntil(() => PhotonNetwork.PlayerList.Length == 2);
            SoundManager.Instance.PlayBGM_RPC("S1S2");
            isStart = true;
        }
    }

}
