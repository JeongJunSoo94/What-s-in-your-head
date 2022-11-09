using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object.Stage1
{
    public class SimpleMovingPlatform : MonoBehaviour
    {
        [Header("이동 지점 목록")] [SerializeField] List<Vector3> posList;
        [Header("이동 속도")] [SerializeField] float movingSpeed = 10f;

        Transform curTF;
        int curIndex = 0;
        int offset = 1;

        private void Awake()
        {
            curTF = transform;
            if(posList.Count != 0)
                curTF.position = posList[0];
        }


        void FixedUpdate()
        {
            if (posList.Count == 0)
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
        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                Transform playerTF = collision.gameObject.transform;
                if (playerTF.parent != this.transform &&
                    playerTF.position.y >= this.transform.position.y)
                    playerTF.parent = this.transform;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                Transform playerTF = collision.gameObject.transform;                
                if (playerTF.IsChildOf(transform))
                {
                    playerTF.parent = null;
                }
            }
        }
    }
}

