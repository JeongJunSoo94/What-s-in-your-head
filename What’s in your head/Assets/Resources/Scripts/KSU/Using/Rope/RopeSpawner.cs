using JCW.UI.InGame.Indicator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class RopeSpawner : MonoBehaviour
    {
        [SerializeField] GameObject NellaRopeCenter;
        [SerializeField] GameObject SteadyRopeCenter;
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
            transform.GetChild(3).GetComponent<ConvertIndicator>().SetInteractableRange(detectingRange, interactableRange);
        }

        public bool StartRopeAction(GameObject player, float moveToRopeSpeed)
        {
            switch (player.tag)
            {
                case "Nella":
                    {
                        if(NellaRopeCenter.activeSelf)
                        {
                            player.GetComponent<PlayerInteractionState>().isMoveToRope = false;
                            Debug.Log("NellaRopeCenter.activeSelf == true");
                            return false;
                        }
                        NellaRopeCenter.GetComponent<Rope>().player = player;
                        NellaRopeCenter.GetComponent<Rope>().moveToRopeSpeed = moveToRopeSpeed;
                        NellaRopeCenter.SetActive(true);
                    }
                    break;
                case "Steady":
                    {
                        if (SteadyRopeCenter.activeSelf)
                        {
                            player.GetComponent<PlayerInteractionState>().isMoveToRope = false;
                            return false;
                        }
                        SteadyRopeCenter.GetComponent<Rope>().player = player;
                        SteadyRopeCenter.GetComponent<Rope>().moveToRopeSpeed = moveToRopeSpeed;
                        SteadyRopeCenter.SetActive(true);
                    }
                    break;
            }
            return true;
        }

        public float EndRopeAction(GameObject player)
        {
            switch (player.tag)
            {
                case "Nella":
                    {
                        return NellaRopeCenter.GetComponent<Rope>().DeacvtivateRope(player);
                    }
                case "Steady":
                    {
                        return SteadyRopeCenter.GetComponent<Rope>().DeacvtivateRope(player);
                    }
                default:
                    return 0f;
            }
        }
    }
}
