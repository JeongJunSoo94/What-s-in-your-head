using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using YC.Camera_;
using YC.Camera_Single;

using JCW.UI.Options.InputBindings;

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

        Camera mainCamera;
        [SerializeField] GameObject lookAtObj;
        public Ray ray;
        RaycastHit _raycastHit;
        public LayerMask layerFilterForGrapple;
        public LayerMask layerForGrapple;

        public GameObject grapplSpawner;
        public Vector3 autoAimPosition;


        public float rangeRadius = 5f;
        public float rangeDistance = 5f;
        public GameObject sphere;

        public GameObject hookableTarget;

        Vector3 grappleVec;
        Vector3 targetPosition;

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
                mainCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // 멀티용
            else
                mainCamera = this.gameObject.GetComponent<CameraController_Single>().FindCamera(); // 싱글용
        }

        void Update()
        {
            SearchGrappledObject();
            MakeGizmoVecs();
            //Fire();
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
                Vector3 cameraForwardXZ = mainCamera.transform.forward;
                cameraForwardXZ.y = 0;
                Vector3 rayOrigin = mainCamera.transform.position;
                Vector3 rayEnd = (mainCamera.transform.position + mainCamera.transform.forward * (rangeDistance + rangeRadius * 2f));
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
                            autoAimPosition = _raycastHit.collider.gameObject.transform.position;
                            steadyInteractionState.isGrappledObjectFounded = true;
                            return;
                        }
                    }
                }
            }
            steadyInteractionState.isGrappledObjectFounded = false;
        }

        void Fire()
        {
            if(KeyManager.Instance.GetKeyDown(PlayerAction.Fire)) //갈고리 날아가는 도중엔 못쏘게 추가해야함
            {
                if (steadyInteractionState.isGrappledObjectFounded)
                {
                    // 도착 위치: autoAimPosition
                }
                else
                {
                    // 도착위치: 화면 중앙에 레이 쏴서 도착하는 곳
                    Vector3 rayOrigin = grapplSpawner.transform.position;
                    Vector3 rayEnd = (mainCamera.transform.position + mainCamera.transform.forward * (rangeDistance + rangeRadius * 2f));
                    Vector3 direction = (rayEnd - rayOrigin).normalized;
                    //bool isRayChecked = Physics.Raycast(rayOrigin, direction, out _raycastHit, rangeDistance + rangeRadius * 2f, layerForGrapple, QueryTriggerInteraction.Ignore);

                }
                // 스테디 그래플 함수안에 init함수 만들어주고 그걸 통해 도착위치 넣어줌

                // 그래플 켜줌
            }
        }

        public void Hook(GameObject targetObj)
        {
            playerController.enabled = false;
            playerState.IsAirJumping = false;
            playerState.WasAirDashing = false;
            playerState.IsGrounded = false;
            targetPosition = targetObj.GetComponent<HookableObject>().GetOffsetPosition();
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
                if (Vector3.Distance(transform.position, targetPosition) < 2f)
                {
                    EscapeMoving();
                }
                playerRigidbody.velocity = grappleVec;
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
        }

        void MakeGizmoVecs()
        {
            if (playerState.aim)
            {
                hVision = mainCamera.transform.forward;

                startCenter = lookAtObj.transform.position;

                startUp = startCenter + mainCamera.transform.up * rangeRadius;
                startDown = startCenter - mainCamera.transform.up * rangeRadius;
                startLeft = startCenter - mainCamera.transform.right * rangeRadius;
                startRight = startCenter + mainCamera.transform.right * rangeRadius;

                endCenter = startCenter + hVision * rangeDistance;

                endUp = endCenter + mainCamera.transform.up * rangeRadius;
                endDown = endCenter - mainCamera.transform.up * rangeRadius;
                endLeft = endCenter - mainCamera.transform.right * rangeRadius;
                endRight = endCenter + mainCamera.transform.right * rangeRadius;
            }
        }

        void SendInfoUI()
        {
            if (grappledObjects.Count > 0)
            {
                
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
            Vector3 rayOrigin = grapplSpawner.transform.position;
            Vector3 rayEnd = (mainCamera.transform.position + mainCamera.transform.forward * (rangeDistance + rangeRadius * 2f));

            sphere.transform.position = rayEnd;

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
            if(other.CompareTag("GrappledObject"))
                {
                grappledObjects.Remove(other.gameObject);
            }
        }
    }
}
