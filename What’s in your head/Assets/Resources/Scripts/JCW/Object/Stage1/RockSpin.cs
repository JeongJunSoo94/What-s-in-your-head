using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object.Stage1
{
    public class RockSpin : MonoBehaviour
    {
        [Header("회전 속도")] [SerializeField] float speed = 10f;
        Transform myTF;
        Transform seatTF;

        bool isStart = false;

        private void Awake()
        {
            myTF = this.transform;
            seatTF = myTF.GetChild(1);
            StartCoroutine(nameof(WaitForPlayer));
        }

        private void FixedUpdate()
        {
            if (!isStart)
                return;

            myTF.Rotate(Vector3.right, -speed * Time.fixedDeltaTime, Space.World);
            seatTF.Rotate(Vector3.right, speed * 2f * Time.fixedDeltaTime, Space.World);
        }        

        IEnumerator WaitForPlayer()
        {
            //yield return null;
            yield return new WaitUntil(() => PhotonNetwork.PlayerList.Length == 2);
            isStart = true;
            yield break;
        }
    }

}
