using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object.Stage1
{
    [RequireComponent(typeof(PhotonView))]
    public class RockSpin : MonoBehaviour
    {
        [Header("ȸ�� �ӵ�")] [SerializeField] float speed = 10f;
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
            Debug.Log("������ - �÷��̾� 2�� ��� ����");
            yield return new WaitUntil(() => PhotonNetwork.PlayerList.Length == 2);
            Debug.Log("������ - �÷��̾� 2�� ��� ��");
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
