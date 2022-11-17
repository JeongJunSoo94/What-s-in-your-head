using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object.Stage1
{
    [RequireComponent(typeof(PhotonView))]
    public class RotateMiniRail : MonoBehaviour
    {
        [Header("회전 속도")] [SerializeField] float speed = 10f;
        Transform tf;
        PhotonView pv;

        private void Awake()
        {
            tf = this.transform;
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
            while (true)
            {
                tf.Rotate(Vector3.up, speed * -10 * Time.deltaTime, Space.World);
                yield return null;
            }
        }
    }

}
