using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object.Stage1
{
    [RequireComponent(typeof(PhotonView))]
    public class RockSpin : MonoBehaviour
    {
        [Header("회전 속도")] [SerializeField] float speed = 10f;
        Transform myTF;
        Transform seatTF;

        PhotonView pv;

        private void Awake()
        {
            myTF = this.transform;
            seatTF = myTF.GetChild(1);
            pv = PhotonView.Get(this);
            StartCoroutine(nameof(WaitForPlayer));
        }


        IEnumerator WaitForPlayer()
        {
            yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene());
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
                myTF.Rotate(Vector3.right, -speed * Time.deltaTime, Space.World);
                seatTF.Rotate(Vector3.right, speed * 2f * Time.deltaTime, Space.World);
                yield return null;
            }
        }
    }

}
