using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JCW.Object
{
    public class Fall : MonoBehaviour
    {
        [Header("�߶� �ʱ� �ӵ�")] [SerializeField] float fallSpeed;
        [Header("�߶� ���ӵ�")] [SerializeField] float fallAccelerateSpeed;
        [Header("�ٷ� �Ʒ��� �÷���")] [SerializeField] Transform groundPlatform;
        //Transform groundPlatform;

        public bool isStart = false;
        Vector3 finalPos;
        void Start()
        {
            if (!groundPlatform)
            {
                Debug.Log("�߶��� �÷����� �������� �ʾҽ��ϴ�.");
                Destroy(this.gameObject);
                return;
            }
            transform.GetChild(0).GetComponent<SandSackShadow>().groundPlatform = this.groundPlatform;
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

                    //����� �߶� �� ������Ʈ ��ü�� ��������, ����Ʈ �����ϴ� �Լ��� ���� �Ŀ� ����Ʈ�� ���� ���Ŀ� ���ְų� SetActive(false)�ؾ� �ҵ�.
                    Destroy(this.gameObject);
                    break;
                default:
                    break;
            }
        }
    }

}
