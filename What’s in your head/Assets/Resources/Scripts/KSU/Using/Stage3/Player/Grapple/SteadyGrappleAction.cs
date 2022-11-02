using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using YC.Camera_;
using YC.Camera_Single;


using JCW.UI.InGame.Indicator;
using JCW.UI.InGame;
using KSU.Monster;

namespace KSU
{
    public enum GrappleTargetType { GrappledObject, Monster, Null};
    public class SteadyGrappleAction : MonoBehaviour
    {
        SteadyInteractionState steadyInteractionState;
        PlayerController playerController;
        PlayerState playerState;
        Rigidbody playerRigidbody;
        Animator playerAnimator;

        Camera playerCamera;
        [SerializeField] GameObject lookAtObj;
        RaycastHit _raycastHit;
        [SerializeField] LayerMask layerFilterForGrapple;
        [SerializeField] LayerMask layerForGrapple;

        [SerializeField] GameObject grappleSpawner;
        [SerializeField] GameObject grappleObject;
        SteadyGrapple grapple;
        Transform autoAimPosition;
        [SerializeField] AimUI aimUI;
        GrappleTargetType curTargetType;


        [Header("_______변경 가능 값_______")]
        [Header("플레이어 날아가는 속력")]
        public float grappleMoveSpeed = 10f;
        [Header("갈고리 날아가는 속력")]
        public float grappleSpeed = 10f;
        [Header("갈고리 속력이 빠를땐 이 값을 조금 높이세요")]
        public float grappleDepartOffset = 0.5f;
        [Header("목표지점 도착후 날아가는 속도")]
        public float escapeGrapplePower = 10f;
        [Header("오토 타겟팅 탐지 범위(캡슐) 반지름")]
        public float rangeRadius = 5f;
        [Header("오토 타겟팅 탐지 범위(캡슐) 길이(거리)")]
        public float rangeDistance = 15f;
        [Header("갈고리 투척 최대 거리(rangeDistance + rangeRadius * 2 이상으로)")]
        public float grapplingRange = 30f;
        [Header("오토 타겟팅 탐지 범위 (각도)"), Range(1f, 89f)]
        public float rangeAngle = 30f;

        GameObject grappledTarget;

        Vector3 grappleVec;
        Vector3 targetPosition;


        List<GameObject> grappledObjects = new();

        /// <summary> Gizmo
        Vector3 startCenter;
        Vector3 startUp;
        Vector3 startDown;
        Vector3 startLeft;
        Vector3 startRight;

        Vector3 hVision;

        Vector3 endCenter;
        Vector3 endUp;
        Vector3 endDown;
        Vector3 endLeft;
        Vector3 endRight;
        /// </summary>
        /// 
        PhotonView photonView;


        // Start is called before the first frame update
        void Awake()
        {
            playerController = GetComponent<PlayerController>();
            playerState = GetComponent<PlayerState>();
            steadyInteractionState = GetComponent<SteadyInteractionState>();
            playerRigidbody = GetComponent<Rigidbody>();

            
            playerCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // 멀티용

            grappleObject = Instantiate(grappleObject);
            grappleObject.SetActive(false);
            grapple = grappleObject.GetComponent<SteadyGrapple>();
            grapple.player = this;
            grapple.spawner = grappleSpawner;

            playerAnimator = GetComponent<Animator>();
            photonView = GetComponent<PhotonView>();
        }

        void Update()
        {
            SearchGrappledObject();
            if(photonView.IsMine)
            {
                SendInfoUI();
                SendInfoAImUI();
            }
            
            if (grappleSpawner.transform.parent.gameObject.activeSelf && grappleSpawner.transform.parent.gameObject.activeSelf)
            {
                MakeGizmoVecs();
            }
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
            if (!playerAnimator.GetBool("isShootingGrapple") && grappleSpawner.transform.parent.gameObject.activeSelf && grappleSpawner.activeSelf)
            {
                Debug.Log("검사 시작");
                if (playerState.aim)
                {
                    Debug.Log("에임 중");
                    Vector3 cameraForwardXZ = playerCamera.transform.forward;
                    cameraForwardXZ.y = 0;
                    Vector3 rayOrigin = playerCamera.transform.position;
                    Vector3 rayEnd = (playerCamera.transform.position + playerCamera.transform.forward * (rangeDistance + rangeRadius * 2f));
                    Vector3 direction = (rayEnd - rayOrigin).normalized;
                    bool isRayChecked = Physics.SphereCast(rayOrigin, rangeRadius, direction, out _raycastHit, rangeDistance, layerForGrapple, QueryTriggerInteraction.Ignore);

                    if (isRayChecked)
                    {
                        Debug.Log("있나");
                        direction = (_raycastHit.collider.gameObject.transform.position - rayOrigin).normalized;
                        isRayChecked = Physics.SphereCast(rayOrigin, 0.2f, direction, out _raycastHit, (rangeDistance + rangeRadius * 2f), layerFilterForGrapple, QueryTriggerInteraction.Ignore);
                        if (isRayChecked)
                        {
                            if (_raycastHit.collider.gameObject.layer == LayerMask.NameToLayer("AutoAimedObject"))
                            {
                                if (Vector3.Angle(playerCamera.transform.forward, (_raycastHit.collider.gameObject.transform.position - rayOrigin)) < rangeAngle)
                                {
                                    Debug.Log("오토 타겟");
                                    //aimUI.SetTarget(_raycastHit.collider.gameObject.transform, rangeAngle);
                                    autoAimPosition = _raycastHit.collider.gameObject.transform;
                                    steadyInteractionState.isGrappledObjectFounded = true;
                                    return;
                                }
                            }
                        }
                    }
                }
                //aimUI.SetTarget(null, rangeAngle);
                steadyInteractionState.isGrappledObjectFounded = false;
            }
        }

        public void SendInfoAImUI()
        {
            if(steadyInteractionState.isGrappledObjectFounded)
            {
                aimUI.SetTarget(autoAimPosition, rangeAngle);
            }
            else
            {
                aimUI.SetTarget(null, rangeAngle);
            }
        }

        public void InputFire()
        {
            if (playerState.isOutOfControl || playerState.isStopped)
                return;

            if (grappleSpawner.transform.parent.gameObject.activeSelf   )
            {
                if (!grapple.gameObject.activeSelf)
                {
                    grappleSpawner.SetActive(false);
                    if(GameManager.Instance.isTopView)
                    {
                        ///////////////// 선택지 2: 여기에 마우스 거리 만큼의 위치까지 쏘는 갈고리 발사
                        Vector3 forward = (playerController.playerMouse.point.transform.position - grappleSpawner.transform.position);
                        forward.y = 0;
                        if (forward.magnitude > grapplingRange)
                            forward = forward.normalized * grapplingRange;
                        grapple.InitGrapple(grappleSpawner.transform.position, (grappleSpawner.transform.position + forward), grappleSpeed, grappleDepartOffset);
                    }
                    else if (steadyInteractionState.isGrappledObjectFounded)
                    {
                        // 도착 위치: autoAimPosition
                        grapple.InitGrapple(grappleSpawner.transform.position, autoAimPosition.position, grappleSpeed, grappleDepartOffset);
                    }
                    else
                    {
                        // 도착위치: 화면 중앙에 레이 쏴서 도착하는 곳
                        RaycastHit hit;
                        bool rayCheck = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, grapplingRange,-1,QueryTriggerInteraction.Ignore);
                        if(rayCheck && (hit.distance > Vector3.Distance(transform.position, playerCamera.transform.position)))
                        {
                            grapple.InitGrapple(grappleSpawner.transform.position, hit.point, grappleSpeed, grappleDepartOffset);
                        }
                        else
                        {
                            grapple.InitGrapple(grappleSpawner.transform.position, (playerCamera.transform.position + playerCamera.transform.forward * grapplingRange), grappleSpeed, grappleDepartOffset);
                        }
                        //grapple.InitGrapple(grappleSpawner.transform.position, (playerCamera.transform.position + playerCamera.transform.forward * grapplingRange), grappleSpeed, grappleDepartOffset);

                    }
                }
            }
        }

        public void RecieveGrappleInfo(bool isSuceeded, GameObject targetObj, GrappleTargetType grappleTargetType)
        {
            curTargetType = grappleTargetType;
            steadyInteractionState.isSucceededInGrappling = isSuceeded;

            if (isSuceeded)
            {
                grappledTarget = targetObj;
                //if (targetObj.CompareTag("grappledObjects"))
                //    Hook();
            }
            else
            {
                grappleSpawner.SetActive(true);
            }
        }

        public bool GetWhetherHit(GrappleTargetType grappleTargetType)
        {
            if((grappleTargetType == curTargetType))
            {
                return steadyInteractionState.isSucceededInGrappling;
            }
            return false;
        }

        public void StartGrab()
        {
            steadyInteractionState.isGrabMonster = true;
            playerState.isRiding = true;
            playerController.MoveStop();
            Vector3 lookVec = (autoAimPosition.position - transform.position);
            lookVec.y = 0;
            transform.LookAt(transform.position + lookVec);
        }

        public bool GetWhetherGrappleActived()
        {
            return grapple.gameObject.activeSelf;
        }

        public void EndGrab()
        {
            steadyInteractionState.isGrabMonster = false;
            playerState.isRiding = false;
            grappleSpawner.SetActive(true);
        }
        public void Hook()
        {
            playerState.isRiding = true;
            playerState.IsAirJumping = false;
            playerState.WasAirDashing = false;
            playerState.IsGrounded = false;
            targetPosition = grappledTarget.GetComponent<GrappledObject>().GetOffsetPosition();
            grappleVec = (targetPosition - transform.position).normalized * grappleMoveSpeed;
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
                    Vector3 directoin = (grappledObject.transform.parent.position - playerCamera.transform.position).normalized;
                    bool rayCheck = false;
                    RaycastHit hit;
                    switch (grappledObject.tag)
                    {
                        case "GrappledObject":
                            {
                                rayCheck = Physics.Raycast(playerCamera.transform.position, directoin, out hit, grappledObject.GetComponentInParent<GrappledObject>().detectingRange * 1.5f, layerFilterForGrapple, QueryTriggerInteraction.Ignore);
                                if (rayCheck)
                                {
                                    if (hit.collider.CompareTag("GrappledObject"))
                                    {
                                        rayCheck = true;
                                    }
                                    else
                                    {
                                        rayCheck = false;
                                    }
                                }
                            }
                            break;
                        case "PoisonSnake":
                            {
                                rayCheck = Physics.Raycast(playerCamera.transform.position, directoin, out hit, grappledObject.GetComponentInParent<DefenseMonster>().detectingUIRange * 1.5f, layerFilterForGrapple, QueryTriggerInteraction.Ignore);
                                if (rayCheck)
                                {
                                    if (hit.collider.CompareTag("PoisonSnake"))
                                    {
                                        rayCheck = true;
                                    }
                                    else
                                    {
                                        rayCheck = false;
                                    }
                                }
                            }
                            break;
                        case "TrippleHeadSnake":
                            {
                                rayCheck = Physics.Raycast(playerCamera.transform.position, directoin, out hit, grappledObject.GetComponentInParent<DefenseMonster>().detectingUIRange * 1.5f, layerFilterForGrapple, QueryTriggerInteraction.Ignore);
                                if (rayCheck)
                                {
                                    if (hit.collider.CompareTag("TrippleHeadSnake"))
                                    {
                                        rayCheck = true;
                                    }
                                    else
                                    {
                                        rayCheck = false;
                                    }
                                }
                            }
                            break;
                    }

                    if (rayCheck)
                    {
                        grappledObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(true);
                    }
                    else
                    {
                        grappledObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(false);
                    }
                }
            }

            //if(steadyInteractionState.isGrappledObjectFounded)
            //{

            //}
        }

        public void EscapeMoving()
        {
            grappledTarget = null;
            steadyInteractionState.isSucceededInGrappling = false;
            grappleSpawner.SetActive(true);
            steadyInteractionState.isGrappling = false;
            Vector3 inertiaVec = grappleVec;
            float jumpPower = inertiaVec.y;
            inertiaVec.y = 0;

            playerController.MakeinertiaVec(escapeGrapplePower, transform.forward.normalized);
            playerController.moveVec = Vector3.up * jumpPower;
            playerController.characterState.isRiding = false;
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

        //private void OnDrawGizmos()
        //{
        //    if (playerState.aim)
        //    {
        //        if (_raycastHit.point != null)
        //        {
        //            Gizmos.color = Color.red;
        //            Gizmos.DrawSphere(_raycastHit.point, 1f);
        //        }

        //        Gizmos.DrawLine(startUp, endUp);
        //        Gizmos.DrawLine(startDown, endDown);
        //        Gizmos.DrawLine(startRight, endRight);
        //        Gizmos.DrawLine(startLeft, endLeft);

        //        Gizmos.DrawWireSphere(startCenter, rangeRadius);
        //        Gizmos.DrawWireSphere(endCenter, rangeRadius);
        //    }
        //    Vector3 rayOrigin = grappleSpawner.transform.position;
        //    Vector3 rayEnd = (playerCamera.transform.position + playerCamera.transform.forward * (rangeDistance + rangeRadius * 2f));

        //    //sphere.transform.position = rayEnd;

        //    Gizmos.DrawLine(rayOrigin, rayEnd);
        //}

        private void OnTriggerEnter(Collider other)
        {

            if ((other.gameObject.layer == LayerMask.NameToLayer("UITriggers")) && (other.CompareTag("GrappledObject") || other.CompareTag("PoisonSnake") || other.CompareTag("TrippleHeadSnake")))
            {
                grappledObjects.Add(other.gameObject);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if ((other.gameObject.layer == LayerMask.NameToLayer("UITriggers")) && (other.CompareTag("GrappledObject") || other.CompareTag("PoisonSnake") || other.CompareTag("TrippleHeadSnake")))
            {
                other.gameObject.transform.parent.gameObject.GetComponentInChildren<OneIndicator>().SetUI(false);
                grappledObjects.Remove(other.gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (steadyInteractionState.isGrappling)
            {
                playerRigidbody.velocity = Vector3.zero;
                grappleVec = Vector3.zero;
                EscapeMoving();
            }
        }
    }
}
