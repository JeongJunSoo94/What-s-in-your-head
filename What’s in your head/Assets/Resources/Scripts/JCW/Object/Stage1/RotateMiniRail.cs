using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object.Stage1
{
    public class RotateMiniRail : MonoBehaviour
    {
        [Header("회전 속도")] [SerializeField] float speed = 10f;
        bool isStart = false;
        Transform tf;

        private void Awake()
        {
            tf = this.transform;
            StartCoroutine(nameof(WaitForPlayer));
        }
        void FixedUpdate()
        {
            if (!isStart)
                return;
            tf.Rotate(Vector3.up, speed * -10 * Time.deltaTime, Space.World);
        }

        IEnumerator WaitForPlayer()
        {
            //yield return null;
            yield return new WaitUntil(() => PhotonNetwork.PlayerList.Length == 2);
            isStart = true;
        }
    }

}
