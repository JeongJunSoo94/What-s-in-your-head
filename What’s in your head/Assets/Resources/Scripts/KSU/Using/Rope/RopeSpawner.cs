using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class RopeSpawner : MonoBehaviour
    {
        [SerializeField] GameObject NellaRopeCentor;
        [SerializeField] GameObject SteadyRopeCentor;
        [SerializeField] GameObject detector;

        //[Header("UI 콜라이더")] [SerializeField] private GameObject UIObj;
        [Header("_______변경 가능 값_______")]
        [Header("탐지 가능 거리")] 
        public float detectingRange = 30f;
        [Header("상호 작용 가능 거리")] 
        public float interactableRange = 20f;
        [Header("로프 길이")] 
        public float ropeLength = 15f;
        [Header("로프 수평 회전 속력")] 
        public float rotationSpeed = 180f;
        [Header("로프 수평 회전 오차 값(회전 속력이 커서 멈추지 않을 때 증가해야함)")] 
        public float rotationOffset = 2f;
        [Header("로프 진자 운동 속력")] 
        public float swingSpeed = 30f;
        [Header("수직 높이에 따른 진자 운동 속력 격차 비율(0.9에 가까울수록 변화가 큼)"), Range(0, 0.9f)] 
        public float SwingDeltaSpeed = 0.4f;
        [Header("진자 최대 높이 도달시 멈추는 시간")] 
        public float waitTime = 0.1f;
        [Header("로프 진자 운동 최대 각도")] 
        public float swingAngle = 60f;

        // Start is called before the first frame update
        void Start()
        {
            detector.transform.localScale = new Vector3(1, 1, 1) * (detectingRange * 2f);
            //  .SendMessage("SetInteractableRange", interactableRange);
        }

        public void StartRopeAction(GameObject player, float moveToRopeSpeed)
        {
            switch (player.tag)
            {
                case "Nella":
                    {
                        NellaRopeCentor.GetComponent<Rope>().player = player;
                        NellaRopeCentor.GetComponent<Rope>().moveToRopeSpeed = moveToRopeSpeed;
                        NellaRopeCentor.SetActive(true);
                    }
                    break;
                case "Steady":
                    {
                        SteadyRopeCentor.GetComponent<Rope>().player = player;
                        SteadyRopeCentor.GetComponent<Rope>().moveToRopeSpeed = moveToRopeSpeed;
                        SteadyRopeCentor.SetActive(true);
                    }
                    break;
            }
        }

        public float EndRopeAction(GameObject player)
        {
            switch (player.tag)
            {

                case "Nella":
                    {
                        return NellaRopeCentor.GetComponent<Rope>().DeacvtivateRope(player);
                    }
                case "Steady":
                    {
                        return SteadyRopeCentor.GetComponent<Rope>().DeacvtivateRope(player);
                    }
                default:
                    return 0f;
            }
        }
    }
}
