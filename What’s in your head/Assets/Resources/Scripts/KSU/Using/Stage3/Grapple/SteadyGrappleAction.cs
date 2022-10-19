using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using YC.Camera_;
using YC.Camera_Single;

using JCW.UI.Options.InputBindings;
using JCW.UI.InGame;

namespace KSU
{
    public class SteadyGrappleAction : MonoBehaviour
    {
        SteadyInteractionState steadyInteractionState;
        PlayerController playerController;
        PlayerState playerState;
        Rigidbody playerRigidbody;

        public float grappleSpeed = 10f;
        public float escapeGrapplePower = 10f;

        Camera playerCamera;
        [SerializeField] GameObject lookAtObj;
        public Ray ray;
        RaycastHit _raycastHit;
        public LayerMask layerFilterForGrapple;
        public LayerMask layerForGrapple;

        public GameObject grappleSpawner;
        public SteadyGrapple grapple;
        public Vector3 autoAimPosition;


        public float rangeRadius = 5f;
        public float rangeDistance = 5f;
        [Range(1f,89f)]
        public float rangeAngle = 30f;
        //public GameObject sphere;

        public GameObject hookableTarget;

        Vector3 grappleVec;
        Vector3 targetPosition;

        public bool isGrappledObjectCollided = false;

        List<GameObject> grappledObjects = new();

        /// <summary> Gizmo
        Vector3 startCenter;
        Vector3 startUp;
        Vector3 startDown;
        Vector3 startLeft;
        Vector3 startRight;

        public Vector3 hVision;

        Vector3 endCenter;
        Vector3 endUp;
        Vector3 endDown;
        Vector3 endLeft;
        Vector3 endRight;
        /// </summary>


        // Start is called before the first frame update
        void Awake()
        {
            playerController = GetComponent<PlayerController>();
            playerState = GetComponent<PlayerState>();
            steadyInteractionState = GetComponent<SteadyInteractionState>();
            playerRigidbody = GetComponent<Rigidbody>();

            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
                playerCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // 멀티용
            else
                playerCamera = this.gameObject.GetComponent<CameraController_Single>().FindCamera(); // 싱글용
        }

        void Update()
        {
            SearchGrappledObject();
            MakeGizmoVecs();
            InputFire();
            MoveToTarget();
        }
        // 스테디 전용 스킬 클래스로
        //void HookOrEscape()
        //{
        //    if (steadyInteractionState.isHookableObjectFounded)
        //    {
        //        if (steadyInteractionState.isRidingHookingRope)
        //        {
        //            if (KeyManager.Instance.GetKeyDown(PlayerAction.Jump))
        //            {
        //                EscapeMoving();
        //            }
        //        }
        //        else
        //        {
        //            if (KeyManager.Instance.GetKeyDown(PlayerAction.Fire))
        //            {
        //                Hook(hookableTarget);
        //            }
        //        }
        //    }
        //}
        public void SearchGrappledObject()
        {
            if (playerState.aim)
            {
                Vector3 cameraForwardXZ = playerCamera.transform.forward;
                cameraForwardXZ.y = 0;
                Vector3 rayOrigin = playerCamera.transform.position;
                Vector3 rayEnd = (playerCamera.transform.position + playerCamera.transform.forward * (rangeDistance + rangeRadius * 2f));
                Vector3 direction = (rayEnd - rayOrigin).normalized;
                bool isRayChecked = Physics.SphereCast(rayOrigin, rangeRadius, direction, out _raycastHit, rangeDistance, layerForGrapple, QueryTriggerInteraction.Ignore);

                if (isRayChecked)
                {
                    direction = (_raycastHit.collider.gameObject.transform.position - rayOrigin).normalized;
                    isRayChecked = Physics.SphereCast(rayOrigin, 0.2f, direction, out _raycastHit, (rangeDistance + rangeRadius * 2f), layerFilterForGrapple, QueryTriggerInteraction.Ignore);
                    if (isRayChecked)
                    {
                        if (_raycastHit.collider.CompareTag("GrappledObject"))
                        {
                            if (Vector3.Angle(playerCamera.transform.forward, (_raycastHit.collider.gameObject.transform.position - rayOrigin)) < rangeAngle)
                            {
                                autoAimPosition = _raycastHit.collider.gameObject.transform.position;
                                steadyInteractionState.isGrappledObjectFounded = true;
                                return;
                            }
                        }
                    }
                }
            }
            steadyInteractionState.isGrappledObjectFounded = false;
        }

        void InputFire()
        {
            if(!grapple.gameObject.activeSelf)
            {
                if (KeyManager.Instance.GetKeyDown(PlayerAction.Fire)) //갈고리 날아가는 도중엔 못쏘게 추가해야함
                {
                    if (steadyInteractionState.isGrappledObjectFounded)
                    {
                        // 도착 위치: autoAimPosition
                        grapple.InitGrapple(grappleSpawner.transform.position, autoAimPosition);
                        grapple.gameObject.SetActive(true);
                    }
                    else
                    {
                        // 도착위치: 화면 중앙에 레이 쏴서 도착하는 곳
                        grapple.InitGrapple(grappleSpawner.transform.position, (playerCamera.transform.position + playerCamera.transform.forward * (rangeDistance + rangeRadius * 2f)));
                        grapple.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void RecieveGrappleInfo(bool isSuceeded, GameObject targetObj)
        {
            if(isSuceeded)
            {
                Hook(targetObj);
            }
        }

        public void Hook(GameObject targetObj)
        {
            playerController.enabled = false;
            playerState.IsAirJumping = false;
            playerState.WasAirDashing = false;
            playerState.IsGrounded = false;
            Debug.Log("targetObj: " + targetObj.name);
            Debug.Log("targetObj.GetComponent<GrappledObject>() : " + targetObj.GetComponent<GrappledObject>());
            Debug.Log("offset: " + targetObj.GetComponent<GrappledObject>().GetOffsetPosition());
            targetPosition = targetObj.GetComponent<GrappledObject>().GetOffsetPosition();
            grappleVec = (targetPosition - transform.position).normalized * grappleSpeed;
            Vector3 lookVec = grappleVec;
            lookVec.y = 0;
            transform.LookAt(transform.position + lookVec);
            steadyInteractionState.isGrappling = true;
        }

        void MoveToTarget()
        {
            if (steadyInteractionState.isGrappling)
            {
                if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
                {
                    EscapeMoving();
                }
                playerRigidbody.velocity = grappleVec;
            }
        }

        void SendInfoUI()
        {
            if (grappledObjects.Count > 0)
            {
                foreach (var grappledObject in grappledObjects)
                {
                    //isMine false면 안보냄 / 현재 널

                    bool rayCheck = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, grappledObject.GetComponent<GrappledObject>().detectingRange * -0.2f, layerFilterForGrapple, QueryTriggerInteraction.Ignore);
                    //grappledObject.GetComponentInChildren<TargetIndicator>().SetUI(rayCheck, playerCamera); // 오버로드 필요
                    // UI상태(bool)가 다르면 신호 struct Obj_Info(bool isUIActive,bool isInteractive, float distance)를 보냄
                }
            }
        }

        public void EscapeMoving()
        {
            steadyInteractionState.isGrappling = false;
            Vector3 inertiaVec = grappleVec;
            float jumpPower = inertiaVec.y;
            inertiaVec.y = 0;

            playerController.MakeinertiaVec(escapeGrapplePower, transform.forward.normalized);
            playerController.moveVec = Vector3.up * jumpPower;
            playerController.enabled = true;
            grapple.gameObject.SetActive(false);
        }

        void MakeGizmoVecs()
        {
            if (playerState.aim)
            {
                hVision = playerCamera.transform.forward;

                startCenter = playerCamera.transform.position;

                startUp = startCenter + playerCamera.transform.up * rangeRadius;
                startDown = startCenter - playerCamera.transform.up * rangeRadius;
                startLeft = startCenter - playerCamera.transform.right * rangeRadius;
                startRight = startCenter + playerCamera.transform.right * rangeRadius;

                endCenter = startCenter + hVision * rangeDistance;

                endUp = endCenter + playerCamera.transform.up * rangeRadius;
                endDown = endCenter - playerCamera.transform.up * rangeRadius;
                endLeft = endCenter - playerCamera.transform.right * rangeRadius;
                endRight = endCenter + playerCamera.transform.right * rangeRadius;
            }
        }

        private void OnDrawGizmos()
        {
            if(playerState.aim)
            {
                if (_raycastHit.point != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(_raycastHit.point, 1f);
                }

                Gizmos.DrawLine(startUp, endUp);
                Gizmos.DrawLine(startDown, endDown);
                Gizmos.DrawLine(startRight, endRight);
                Gizmos.DrawLine(startLeft, endLeft);

                Gizmos.DrawWireSphere(startCenter, rangeRadius);
                Gizmos.DrawWireSphere(endCenter, rangeRadius);
            }
            Vector3 rayOrigin = grappleSpawner.transform.position;
            Vector3 rayEnd = (playerCamera.transform.position + playerCamera.transform.forward * (rangeDistance + rangeRadius * 2f));

            //sphere.transform.position = rayEnd;

            Gizmos.DrawLine(rayOrigin, rayEnd);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("GrappledObject"))
            {
                grappledObjects.Add(other.gameObject);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("GrappledObject"))
            {
                grappledObjects.Remove(other.gameObject);
            }
        }
    }
}
