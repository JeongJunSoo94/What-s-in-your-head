using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object.Stage1
{
    public class BothButton : MonoBehaviour
    {
        [Header("������ ��ֹ�")] [SerializeField] Transform obstacleTF;
        [Header("��ֹ��� �̵� ��ų ���� ��ġ")] [SerializeField] Vector3 movLocalPos;
        [Header("�̵� �ӵ�")] [SerializeField] float moveSpeed = 5f;
        [Header("�������� ų������")] [SerializeField] bool isRemotePlay = false;

        public int bothCount = 0;
        bool isStart = false;
        Vector3 initPos;

        private void Awake()
        {
            initPos = transform.localPosition;
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (isRemotePlay)
                return;
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                if (collision.transform.position.y >= this.transform.position.y)
                {
                    ++bothCount;
                    if(bothCount>= 2 && !isStart)
                    {
                        isStart = true;
                        StartCoroutine(nameof(moveObstacle));
                    }
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (isRemotePlay)
                return;
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                --bothCount;
                if (bothCount <= 0)
                    StartCoroutine(nameof(Reset));
            }
        }

        public void SetBothCount(int count)
        {
            bothCount = count;
            if(bothCount >= 2 && !isStart)
            {
                isStart = true;
                StartCoroutine(nameof(moveObstacle));
            }
            if(bothCount<=0)
            {
                StartCoroutine(nameof(Reset));
            }
        }

        IEnumerator moveObstacle()
        {
            while(Vector3.SqrMagnitude(obstacleTF.localPosition - movLocalPos) > 0.1f)
            {
                obstacleTF.localPosition = Vector3.MoveTowards(obstacleTF.localPosition, movLocalPos, Time.fixedDeltaTime * moveSpeed);
                yield return null;
            }
            yield break;
        }

        IEnumerator Reset()
        {
            while (Vector3.SqrMagnitude(obstacleTF.localPosition - initPos) > 0.1f)
            {
                obstacleTF.localPosition = Vector3.MoveTowards(obstacleTF.localPosition, initPos, Time.fixedDeltaTime * moveSpeed);
                yield return null;
            }
            isStart = false;
            yield break;
        }
    }
}