using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.Utility;

using YC.Camera_;
using YC.Camera_Single;

namespace KSU
{
    public class Rail : MonoBehaviour
    {
        public GameObject Nella;
        public GameObject Steady;

        public GameObject NellaCart;
        public GameObject SteadyCart;

        public float NellaDestiation;
        public float SteadyDestiation;

        public bool isNellaCartActive = false;
        public bool isSteadyCartActive = false;

        Cinemachine.CinemachineSmoothPath track;
        public GameObject railCollider;

        [Header("_______변경 가능 값_______")]
        [Header("레일 UI 확인 가능 범위(캡슐) 반지름")]
        public float detectionRangeRadius = 10f;
        [Header("레일 UI 확인 가능 범위(캡슐) 길이(거리)")]
        public float detectionRangeLength = 2f;
        [Header("레일 타는 중 속도")]
        public float railSpeed = 4f;
        //[Header("레일 탈출 속도")]
        //public
        float escapingRailSpeed = 4f;

        Vector3 offset;

        void Awake()
        {
            track = GetComponent<Cinemachine.CinemachineSmoothPath>();
            offset = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if(isNellaCartActive || isSteadyCartActive)
            {
                CheckCarts();
            }
        }

        private void OnEnable()
        {
            MakeCollider();
        }

        void MakeCollider()
        {
            if (track.m_Waypoints.Length > 1)
            {
                for (int i = 1; i < track.m_Waypoints.Length; i++)
                {
                    Vector3 start = track.m_Waypoints[i - 1].position + offset;
                    Vector3 end = track.m_Waypoints[i].position + offset;
                    float length = Vector3.Distance(start, end);

                    GameObject obj = Instantiate(railCollider, ((start + end) / 2), Quaternion.Euler(Vector3.zero));
                    obj.name = (i - 1).ToString();
                    obj.layer = LayerMask.NameToLayer("Rail");
                    obj.tag = "Rail";
                    obj.transform.parent = gameObject.transform;
                    obj.transform.localScale = new Vector3(0.1f, 0.1f, length);
                    obj.transform.LookAt(end);
                    obj.GetComponentsInChildren<CapsuleCollider>()[1].gameObject.transform.localScale = new Vector3(detectionRangeRadius * 10f, detectionRangeLength * 1f, detectionRangeRadius * 10f);
                }
            }
        }

        public void RideOnRail(Vector3 railStartPos, GameObject startObj, GameObject player)
        {
            SetDestination(player, startObj, CreateCart(railStartPos, startObj, player));
        }

        void SetDestination(GameObject player, GameObject startObj, Cinemachine.CinemachineDollyCart cartSetUp)
        {
            cartSetUp.gameObject.SetActive(true);
            if (cartSetUp.m_Position < 1)
            {
                cartSetUp.m_Speed = railSpeed;
                switch (player.tag)
                {
                    case "Nella":
                        {
                            NellaDestiation = (track.m_Waypoints.Length - 1.005f);
                            player.transform.parent = NellaCart.transform;
                        }
                        break;
                    case "Steady":
                        {
                            SteadyDestiation = (track.m_Waypoints.Length - 1.005f);
                            player.transform.parent = SteadyCart.transform;
                            player.transform.localPosition = Vector3.zero;
                        }
                        break;
                }
                player.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (cartSetUp.m_Position > (track.m_Waypoints.Length - 2f))
            {
                cartSetUp.m_Speed = -railSpeed;
                switch (player.tag)
                {
                    case "Nella":
                        {
                            NellaDestiation = 0.005f;
                            player.transform.parent = NellaCart.transform;
                        }
                        break;
                    case "Steady":
                        {
                            SteadyDestiation = 0.005f;
                            player.transform.parent = SteadyCart.transform;
                        }
                        break;
                }
                player.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else
            {
                Vector3 playerVec = player.transform.forward;
                if (Vector3.Angle(startObj.transform.forward, playerVec) > 90f)
                {
                    cartSetUp.m_Speed = -railSpeed;
                    switch (player.tag)
                    {
                        case "Nella":
                            {
                                NellaDestiation = 0.005f;
                                player.transform.parent = NellaCart.transform;
                            }
                            break;
                        case "Steady":
                            {
                                SteadyDestiation = 0.005f;
                                player.transform.parent = SteadyCart.transform;
                            }
                            break;
                    }
                    player.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                }
                else
                {
                    cartSetUp.m_Speed = railSpeed;
                    switch (player.tag)
                    {
                        case "Nella":
                            {
                                NellaDestiation = (track.m_Waypoints.Length - 1.005f);
                                player.transform.parent = NellaCart.transform;
                            }
                            break;
                        case "Steady":
                            {
                                SteadyDestiation = (track.m_Waypoints.Length - 1.005f);
                                player.transform.parent = SteadyCart.transform;
                            }
                            break;
                    }
                    player.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
            }
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.transform.localPosition = Vector3.zero;

            switch (player.tag)
            {
                case "Nella":
                    {
                        isNellaCartActive = true;   
                    }
                    break;
                case "Steady":
                    {
                        isSteadyCartActive = true;
                    }
                    break;
            }
        }

        Cinemachine.CinemachineDollyCart CreateCart(Vector3 startPos, GameObject startObj, GameObject player)
        {
            if (track.m_Waypoints.Length > 0)
            {
                int startIndex = int.Parse(startObj.name);
                GameObject cart = null;
                player.GetComponent<PlayerController>().characterState.isRiding = true;
                switch (player.tag)
                {
                    case "Nella":
                        Nella = player;
                        cart = NellaCart;
                        break;
                    case "Steady":
                        Steady = player;
                        cart = SteadyCart;
                        break;
                }
                cart.transform.position = startPos;
                float totalLength = Vector3.Distance(track.m_Waypoints[startIndex].position, track.m_Waypoints[startIndex + 1].position);
                float startLength = Vector3.Distance(track.m_Waypoints[startIndex].position + offset, startPos);
                float rate = startLength / totalLength;

                Cinemachine.CinemachineDollyCart cartSetUp = cart.GetComponent<Cinemachine.CinemachineDollyCart>();
                cartSetUp.m_Position = startIndex + rate;

                return cartSetUp;
            }

            return null;
        }

        void CheckCarts()
        {
            if (isNellaCartActive)
            {
                float signOfSpeed = GetSign(NellaCart.GetComponent<Cinemachine.CinemachineDollyCart>().m_Speed);
                if (Nella.GetComponent<PlayerInteractionState>().isRailJumping)
                {
                    NellaCart.GetComponent<Cinemachine.CinemachineDollyCart>().m_Speed = signOfSpeed * railSpeed / 2f;
                }
                else
                {
                    NellaCart.GetComponent<Cinemachine.CinemachineDollyCart>().m_Speed = signOfSpeed * railSpeed;
                }

                if (Mathf.Abs(NellaCart.GetComponent<Cinemachine.CinemachineDollyCart>().m_Position - NellaDestiation) < 0.2f)
                {
                    EscapeRail(Nella, false);
                }
            }

            if (isSteadyCartActive)
            {
                float signOfSpeed = GetSign(SteadyCart.GetComponent<Cinemachine.CinemachineDollyCart>().m_Speed);
                if (Steady.GetComponent<PlayerInteractionState>().isRailJumping)
                {
                    SteadyCart.GetComponent<Cinemachine.CinemachineDollyCart>().m_Speed = signOfSpeed * railSpeed / 2f;
                }
                else
                {
                    SteadyCart.GetComponent<Cinemachine.CinemachineDollyCart>().m_Speed = signOfSpeed * railSpeed;
                }

                if (Mathf.Abs(SteadyCart.GetComponent<Cinemachine.CinemachineDollyCart>().m_Position - SteadyDestiation) < 0.2f)
                {
                    EscapeRail(Steady, false);
                }
            }
        }

        float GetSign(float num)
        {
            if (num < 0)
            {
                return -1f;
            }
            else
            {
                return 1f;
            }


        }

        public void EscapeRail(GameObject player, bool isSwap)
        {
            RailAction rail = player.GetComponent<RailAction>();
            rail.currentRail = null;
            player.transform.parent = null;
            Camera camera;            
            camera = player.GetComponent<CameraController>().mainCam; // 멀티용
            switch (player.tag)
            {
                case "Nella":
                    {
                        NellaCart.SetActive(false);
                        isNellaCartActive = false;

                    }
                    break;
                case "Steady":
                    {
                        SteadyCart.SetActive(false);
                        isSteadyCartActive = false;
                    }
                    break;
            }
            PlayerController playerController = player.GetComponent<PlayerController>();

            Vector3 inertiaVec = camera.transform.forward;
            inertiaVec.y = 0;

            player.transform.LookAt(player.transform.position + inertiaVec);
            playerController.MakeinertiaVec(escapingRailSpeed, inertiaVec.normalized);
            playerController.moveVec = Vector3.up * playerController.jumpSpeed / 2f;
            playerController.characterState.isRiding = false;
            playerController.characterState.IsDashing = false;
            playerController.characterState.IsAirJumping = false;
            playerController.characterState.WasAirDashing = false;
            PlayerInteractionState interactionState = player.GetComponent<PlayerInteractionState>();
            if (!isSwap)
            {
                rail.SetBoolEscapeRail();
            }
            interactionState.isRidingRail = false;
            interactionState.isRailJumping = false;
            interactionState.isRailJumpingUp = false;
            rail.ResetRailSound();
        }

        public void EscapeAll()
        {
            if(NellaCart.activeSelf && Nella != null)
            {
                EscapeRail(Nella, false);
            }

            if (SteadyCart.activeSelf && Steady != null)
            {
                EscapeRail(Steady, false);
            }
        }
    }
}
