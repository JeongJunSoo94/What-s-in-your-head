using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class RopeSpawner : MonoBehaviour
    {
        //public List<GameObject> playerList;

        public GameObject NellaRopeAction;
        public GameObject SteadyRopeAction;

        [Header("탐지 가능 거리")] public float detectingRange = 30f;
        [Header("상호 작용 가능 거리")] public float interactableRange = 20f;

        [Header("로프 길이")] public float ropeLength = 15f;

        [Header("로프 수평 회전 속력")] public float rotationSpeed = 180f;

        [Header("로프 진자 운동 속력")] public float swingSpeed = 30f;
        [Header("수직 높이에 따른 진자 운동 속력 격차 비율(0.9에 가까울수록 변화가 큼)"), Range(0, 0.9f)] public float SwingDeltaSpeed = 0.4f;
        [Header("진자 최대 높이 도달시 멈추는 시간")] public float waitTime = 0.1f;

        [Header("로프 진자 운동 최대 각도")] public float swingAngle = 60f;

        // Start is called before the first frame update
        void Start()
        {
            InitCollider();
        }

        void InitCollider()
        {
            transform.localScale = new Vector3(1, 1, 1) * (detectingRange * 2f);
        }

        public void StartRopeAction(GameObject player, float moveToRopeSpeed)
        {
            switch (player.tag)
            {
                case "Nella":
                    {
                        NellaRopeAction.GetComponent<Rope>().player = player;
                        NellaRopeAction.GetComponent<Rope>().moveToRopeSpeed = moveToRopeSpeed;
                        NellaRopeAction.SetActive(true);
                    }
                    break;
                case "Steady":
                    {
                        SteadyRopeAction.GetComponent<Rope>().player = player;
                        SteadyRopeAction.GetComponent<Rope>().moveToRopeSpeed = moveToRopeSpeed;
                        SteadyRopeAction.SetActive(true);
                    }
                    break;
            }
            //GameObject obj = Instantiate<GameObject>(ropeAction, transform);
            //obj.GetComponent<RopeAction>().spawner = this;
            //obj.GetComponent<RopeAction>().player = player;
        }

        public float EndRopeAction(GameObject player)
        {
            switch (player.tag)
            {

                case "Nella":
                    {
                        return NellaRopeAction.GetComponent<Rope>().DeacvtivateRope(player);
                    }
                case "Steady":
                    {
                        return SteadyRopeAction.GetComponent<Rope>().DeacvtivateRope(player);
                    }
                default:
                    return 0f;
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.forward * 5f);
        }
    }
}
