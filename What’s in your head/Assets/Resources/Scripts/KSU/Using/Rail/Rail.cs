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

        public float detectionRangeRadius = 10f;
        public float detectionRangeLength = 2f;

        public float railSpeed = 4f;
        public float escapingRailSpeed = 4f;


        void Awake()
        {
            track = GetComponent<Cinemachine.CinemachineSmoothPath>();
        }

        // Update is called once per frame
        void Update()
        {
            CheckCarts();
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
                    Vector3 offset = transform.position;
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

        public void RideOnRail(Vector3 startPos, GameObject startObj, GameObject player)
        {
            SetDestination(player, CreateCart(startPos, startObj, player));
        }

        void SetDestination(GameObject player, Cinemachine.CinemachineDollyCart cartSetUp)
        {
            Vector3 playerVec = cartSetUp.gameObject.transform.position - player.transform.position;
            if (Vector3.Angle(cartSetUp.gameObject.transform.forward, playerVec) > 90f)
            {
                cartSetUp.m_Speed = -railSpeed;
                switch (player.tag)
                {
                    case "Nella":
                        {
                            NellaDestiation = 0.01f;
                            player.transform.parent = NellaCart.transform;
                        }
                        break;
                    case "Steady":
                        {
                            SteadyDestiation = 0.01f;
                            player.transform.parent = SteadyCart.transform;
                        }
                        break;
                }
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                player.transform.localPosition = Vector3.zero;
                player.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else
            {
                cartSetUp.m_Speed = railSpeed;
                switch (player.tag)
                {
                    case "Nella":
                        {
                            NellaDestiation = (track.m_Waypoints.Length - 1.01f);
                            player.transform.parent = NellaCart.transform;
                        }
                        break;
                    case "Steady":
                        {
                            SteadyDestiation = (track.m_Waypoints.Length - 1.01f);
                            player.transform.parent = SteadyCart.transform;
                            player.transform.localPosition = Vector3.zero;
                        }
                        break;
                }
                player.GetComponent<Rigidbody>().velocity = Vector3.zero;
                player.transform.localPosition = Vector3.zero;
                player.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            cartSetUp.gameObject.SetActive(true);
            isNellaCartActive = true;
        }

        Cinemachine.CinemachineDollyCart CreateCart(Vector3 startPos, GameObject startObj, GameObject player)
        {
            if (track.m_Waypoints.Length > 0)
            {
                int startIndex = int.Parse(startObj.name);
                GameObject cart = null;
                player.GetComponent<PlayerController3D>().enabled = false;
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
                float totalLength = Vector3.Distance(track.m_Waypoints[startIndex].position, track.m_Waypoints[startIndex + 1].position);
                float startLength = Vector3.Distance(track.m_Waypoints[startIndex].position, startPos);
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
                Nella.transform.localPosition = Vector3.zero;
                if (Mathf.Abs(NellaCart.GetComponent<Cinemachine.CinemachineDollyCart>().m_Position - NellaDestiation) < 0.2f)
                {
                    EscapeRail(Nella);
                }
            }

            if (isSteadyCartActive)
            {
                Nella.transform.localPosition = Vector3.zero;
                if (Mathf.Abs(SteadyCart.GetComponent<Cinemachine.CinemachineDollyCart>().m_Position - SteadyDestiation) < 0.2f)
                {
                    EscapeRail(Steady);
                }
            }
        }

        public void EscapeRail(GameObject player)
        {
            player.transform.parent = null;
            Camera camera;
            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
                camera = player.GetComponent<CameraController>().mainCam; // 멀티용
            else
                camera = player.GetComponent<CameraController_Single>().mainCam; // 싱글용
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
            PlayerController3D playerController = player.GetComponent<PlayerController3D>();

            Vector3 inertiaVec = camera.transform.forward;
            inertiaVec.y = 0;

            player.transform.LookAt(player.transform.position + inertiaVec);
            playerController.MakeinertiaVec(escapingRailSpeed, inertiaVec.normalized);
            playerController.moveVec = Vector3.up * playerController.jumpSpeed / 2f;
            playerController.enabled = true;
        }
    }
}
