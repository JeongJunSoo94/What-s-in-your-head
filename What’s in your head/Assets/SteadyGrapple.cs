using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class SteadyGrapple : MonoBehaviour
    {

        Vector3 endPosistion;

        public float departingOffset = 0.2f;
        public bool isEndPosition = false;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (isEndPosition)
            {

            }
            else
            {
                MoveToEndPosition();
            }

        }

        public void InitGrapple(Vector3 endPos)
        {
            endPosistion = endPos;
            isEndPosition = false;
        }

        void MoveToEndPosition()
        {
            //1.������ٵ� �߰��ؼ� ���ν�Ƽ�ο����̱�
            //2. transform���� ���� �����̱� >> ��� ��ǥ���� ���� �� �ڿ������� �������°� ����
            if (Vector3.Distance(endPosistion, transform.position) < departingOffset)
            {
                isEndPosition = true;
                // �ڷ�ƾ���� �����ð� ���Ŀ� �������
            }
            else
            {
                isEndPosition = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // �̰� �ҷ�ó�� Ʈ���ŷ� �ݶ��̴� �ؾ��ϰ� �׷��õ� ������Ʈ�� ������ Ʈ���� �±״� �׷��õ� ������Ʈ�� �����������
        }
    }
}

