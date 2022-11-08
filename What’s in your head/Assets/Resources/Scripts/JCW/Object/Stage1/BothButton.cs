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

        public int bothCount = 0;
        bool isStart = false;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                if (collision.transform.position.y >= this.transform.position.y)
                {
                    ++bothCount;
                    if(bothCount>=2 && !isStart)
                    {
                        isStart = true;
                        StartCoroutine(nameof(moveObstacle));
                    }
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                --bothCount;
            }
        }

        IEnumerator moveObstacle()
        {
            while(Vector3.SqrMagnitude(obstacleTF.localPosition - movLocalPos) > 0.1f)
            {
                obstacleTF.localPosition = Vector3.MoveTowards(obstacleTF.localPosition, movLocalPos, Time.fixedDeltaTime * moveSpeed);
                yield return null;
            }

            Destroy(this);
            yield break;
        }
    }
}