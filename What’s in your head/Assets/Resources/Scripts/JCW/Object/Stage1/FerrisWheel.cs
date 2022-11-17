using System.Collections;
using System.Collections.Generic;
using JCW.AudioCtrl;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object.Stage1
{
    [RequireComponent(typeof(PhotonView))]
    public class FerrisWheel : MonoBehaviour
    {
        Transform[] cabins;
        [Header("회전 속도")] [SerializeField] float speed = 10f;
        PhotonView pv;

        private void Awake()
        {
            cabins = transform.GetComponentsInChildren<Transform>();
            pv = PhotonView.Get(this);
            StartCoroutine(nameof(WaitForPlayer));
        }

        IEnumerator WaitForPlayer()
        {
            yield return new WaitUntil(() => PhotonNetwork.PlayerList.Length == 2);
            pv.RPC(nameof(StartFunc), RpcTarget.AllViaServer);

            yield break;
        }

        [PunRPC]
        void StartFunc()
        {
            StartCoroutine(nameof(RotateObj));
        }

        IEnumerator RotateObj()
        {
            while(true)
            {
                cabins[0].Rotate(Vector3.forward, speed * Time.deltaTime, Space.World);
                for (int i = 1 ; i < cabins.Length ; ++i)
                {
                    cabins[i].LookAt(cabins[i].position + Vector3.forward);
                }
                yield return null;
            }            
        }
    }

}
