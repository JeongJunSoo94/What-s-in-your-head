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
            //1.리지드바디 추가해서 벨로시티로움직이기
            //2. transform으로 직접 움직이기 >> 대신 목표지점 도달 후 자연스럽게 떨어지는걸 못함
            if (Vector3.Distance(endPosistion, transform.position) < departingOffset)
            {
                isEndPosition = true;
                // 코루틴으로 일정시간 이후에 사라지기
            }
            else
            {
                isEndPosition = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // 이거 불렛처럼 트리거로 콜라이더 해야하고 그래플드 오브젝트의 디텍팅 트리거 태그는 그래플드 오브젝트와 구분해줘야함
        }
    }
}

