using System.Collections;
using System.Collections.Generic;
using KSU;
using Photon.Pun;
using UnityEngine;


namespace JCW.Object.Stage1
{
    [RequireComponent(typeof(PhotonView))]
    public class SimpleMovingPlatform : MonoBehaviour
    {
        [Header("이동 지점 목록")] [SerializeField] List<Vector3> posList;
        [Header("이동 속도")] [SerializeField] float movingSpeed = 10f;

        Transform curTF;
        int curIndex = 0;
        int offset = 1;
        PhotonView photonView;

        bool isStart = false;

        private void Awake()
        {
            curTF = transform;
            photonView = PhotonView.Get(this);
            if (posList.Count != 0)
                curTF.position = posList[0];
            if (photonView.IsMine)
                StartCoroutine(nameof(WaitForPlayers));
        }


        void FixedUpdate()
        {
            if (posList.Count == 0 || !photonView.IsMine || !isStart)
                return;
            if (Vector3.SqrMagnitude(curTF.position - posList[curIndex]) <= 0.05f)
            {
                curTF.position = posList[curIndex];
                curIndex += offset;
                if (curIndex > posList.Count - 1)
                {
                    offset = -1;
                    --curIndex;
                }
                else if (curIndex < 0)
                {
                    curIndex = 0;
                    offset = 1;
                }
            }
            curTF.position = Vector3.MoveTowards(curTF.position, posList[curIndex], Time.fixedDeltaTime * movingSpeed);
        }       


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (other.transform.position.y >= transform.position.y)
                    other.transform.parent = this.gameObject.transform;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player.characterState.isMine)
                    player.EscapeFromParent();
            }
        }

        IEnumerator WaitForPlayers()
        {
            yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene());
            isStart = true;
            yield break;
        }
    }
}

