using KSU;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JCW.Object
{
    public class PlayObject : MonoBehaviour
    {
        [Header("��ȣ ���� �� �̵� �ӵ�")] [SerializeField] float recieveMovingSpeed;
        [Header("��ȣ ���� �� ���� �ӵ�")] [SerializeField] float comebackMovingSpeed;
        [Header("���� �� ��� ����")] [SerializeField] bool isLethal;
        [Header("�̵� ���")] [SerializeField] List<Vector3> positionList;

        public bool isActive = false;
        int curIndex = 0;
        int maxIndex = 0;

        bool isStart = false;

        private void Awake()
        {
            if (positionList.Count == 0)
                positionList.Add(transform.position);
            transform.position = positionList[0];
            maxIndex = positionList.Count - 1;
        }

        void Update()
        {
            if (isActive)
            {
                if(!isStart)
                {
                    StopAllCoroutines();
                    StartCoroutine(nameof(MoveToEnd));
                    isStart = true;
                }
            }
            else if (isStart)
            {
                StopAllCoroutines();
                isStart = false;
                StartCoroutine(nameof(MoveToFirst));                
            }
        }

        public void SetPositionToList(Vector3 pos)
        {
            Debug.Log("����Ʈ�� �߰�");
            positionList.Add(pos);
        }

        public void ClearList()
        {
            positionList.Clear();
        }

        IEnumerator MoveToEnd()
        {
            if (positionList.Count <= 1)
                yield break;
            
            // ���� �ε����� �ε��� ���̶��, �̸� �̵����ѳ���.
            while(true)
            {
                if (transform.position == positionList[curIndex])
                    break;
                transform.position = Vector3.MoveTowards(transform.position, positionList[curIndex], Time.deltaTime * recieveMovingSpeed);
                if (Vector3.SqrMagnitude(transform.position - positionList[curIndex]) <= 0.05f)
                    transform.position = positionList[curIndex];
                yield return null;
            }

            while(curIndex < maxIndex)
            {
                transform.position = Vector3.MoveTowards(transform.position, positionList[curIndex+1], Time.deltaTime * recieveMovingSpeed);
                if (Vector3.SqrMagnitude(transform.position - positionList[curIndex+1]) <= 0.05f)
                    transform.position = positionList[++curIndex];
                yield return null;
            }

            yield break;
        }

        IEnumerator MoveToFirst()
        {
            if (positionList.Count <= 1)
                yield break;

            // ���� �ε����� �ε��� ���̶��, �̸� �̵����ѳ���.
            while (true)
            {
                if (transform.position == positionList[curIndex])
                    break;
                transform.position = Vector3.MoveTowards(transform.position, positionList[curIndex], Time.deltaTime * comebackMovingSpeed);
                if (Vector3.SqrMagnitude(transform.position - positionList[curIndex]) <= 0.05f)
                    transform.position = positionList[curIndex];
                yield return null;
            }

            while (curIndex > 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, positionList[curIndex - 1], Time.deltaTime * comebackMovingSpeed);
                if (Vector3.SqrMagnitude(transform.position - positionList[curIndex - 1]) <= 0.05f)
                    transform.position = positionList[--curIndex];
                yield return null;
            }

            this.enabled = false;

            yield break;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                Debug.Log("������Ʈ ����");
                Transform playerTF = collision.gameObject.transform;
                if (isLethal)
                    playerTF.GetComponent<PlayerController>().Resurrect();
                else if (playerTF.position.y >= this.transform.position.y)
                    playerTF.parent = this.transform;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Nella") || collision.gameObject.CompareTag("Steady"))
            {
                Transform playerTF = collision.gameObject.transform;
                if (playerTF.IsChildOf(transform))
                    playerTF.parent = null;
            }
        }
    }

}
