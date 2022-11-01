using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object
{
    public class Fall : MonoBehaviour
    {
        [Header("�߶� �ʱ� �ӵ�")] [SerializeField] float fallSpeed;
        [Header("�߶� ���ӵ�")] [SerializeField] float fallAccelerateSpeed;
        Transform groundPlatform;

        public bool isStart = false;
        Vector3 finalPos;
        void Start()
        {
            groundPlatform = transform.GetChild(0).GetComponent<SandSackShadow>().groundPlatform;
            finalPos = transform.position;
            finalPos.y = groundPlatform.position.y;
        }

        void Update()
        {
            if (!isStart)
                return;
            transform.position = Vector3.MoveTowards(transform.position, finalPos, Time.deltaTime * fallSpeed);
            fallSpeed += Time.deltaTime * fallAccelerateSpeed;
        }


        private void OnTriggerEnter(Collider other)
        {
            switch(other.tag)
            {
                case "Nella":
                case "Steady":
                    Debug.Log("���ָӴ� ĳ���� ��� ����");
                    break;
                case "Platform":
                    Debug.Log("���ָӴ� ���� �߶�");
                    isStart = false;
                    Destroy(this.gameObject);
                    break;
                default:
                    break;
            }
        }
    }

}
