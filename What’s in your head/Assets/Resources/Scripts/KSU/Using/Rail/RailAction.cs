using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.Utility;

using YC.Camera_;
using YC.Camera_Single;
using JCW.UI.InGame;

namespace KSU
{
    public class RailAction : MonoBehaviour
    {
        Rigidbody playerRigidbody;
        PlayerController playerController;
        PlayerState playerState;
        PlayerInteractionState interactionState;

        public Camera mainCamera;
        [SerializeField] GameObject lookAtObj;
        Dictionary <GameObject, int> detectedRail = new();
        public Ray ray;
        RaycastHit _raycastHit;
        public LayerMask layerFilterForRail;
        public LayerMask layerForRail;

        public float rangeRadius = 5f;
        public float rangeDistance = 5f;

        public GameObject currentRail;
        public GameObject railStartObject;
        public Vector3 railStartPosiotion = Vector3.zero;

        public float movingToRailSpeed = 10f;
        float departingRailOffset = 0.2f;

        public float railJumpHeight = 4f;
        public float railJumpSpeed = 6f;

        public float escapingRopeSpeed = 6f;
        public float escapingRoDelayTime = 1f;



        /// <summary> Gizmo
        Vector3 startCenter;
        Vector3 startUp;
        Vector3 startDown;
        Vector3 startLeft;
        Vector3 startRight;

        //Vector3 startUps;
        //Vector3 startDowns;
        //Vector3 startLefts;
        //Vector3 startRights;

        public Vector3 hVision;

        Vector3 endCenter;
        Vector3 endUp;
        Vector3 endDown;
        Vector3 endLeft;
        Vector3 endRight;

        //Vector3 endUps;
        //Vector3 endDowns;
        //Vector3 endLefts;
        //Vector3 endRights;
        /// </summary>

        void Awake()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            playerController = GetComponent<PlayerController>();
            playerState = GetComponent<PlayerState>();
            interactionState = GetComponent<PlayerInteractionState>();

            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
                mainCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // 멀티용
            else
                mainCamera = this.gameObject.GetComponent<CameraController_Single>().FindCamera(); // 싱글용
            //layerForRail = ((1) + (1 << LayerMask.NameToLayer("Rail")));
            //layerFilterForRail = ((-1) - (1 << LayerMask.NameToLayer("Player")));
        }

        private void Update()
        {
            SearchRail();
            SendInfoUI();
            //if(interactionState.isRailTriggered)
            //{
            //    if (interactionState.isMovingToRail)
            //    {
            //        MoveToRail();
            //    }
            //    else
            //    {
            //        if (!interactionState.isRidingRail)
            //        {
            //            RideOnRail();
            //        }
            //    }
            //}
        }

        private void FixedUpdate()
        {
            if (interactionState.isRailTriggered)
            {
                if (interactionState.isMovingToRail)
                {
                    MoveToRail();
                }
                else
                {
                    if (!interactionState.isRidingRail)
                    {
                        RideOnRail();
                    }
                }
            }

            if(interactionState.isRidingRail)
            {
                if (interactionState.isRailJumping)
                {
                    JumpOnRail();
                }
                else
                {
                    transform.localPosition = Vector3.zero;
                }
            }
        }

        void SearchRail()
        {
            if (interactionState.railTriggerDetectionNum > 0)
            {
                MakeGizmoVecs();
                SearchWithSphereCast();
            }
        }

        public void SearchWithSphereCast()
        {
            if(!interactionState.isRailTriggered)
            {
                //ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                Vector3 cameraForwardXZ = mainCamera.transform.forward;
                cameraForwardXZ.y = 0;
                Vector3 rayOrigin = (lookAtObj.transform.position - cameraForwardXZ);
                Vector3 rayEnd = (lookAtObj.transform.position + mainCamera.transform.forward * (rangeDistance + rangeRadius * 2f));
                Vector3 direction = (rayEnd - rayOrigin).normalized;
                bool isRayChecked = Physics.SphereCast(rayOrigin, rangeRadius, direction, out _raycastHit, rangeDistance, layerForRail, QueryTriggerInteraction.Ignore);

                //Debug.Log("1 RayChecked : " + isRayChecked);
                if (isRayChecked)
                {
                    //Debug.Log("1 hit! : " + _raycastHit.collider.tag);
                    RaycastHit hit;
                    //isRayChecked = Physics.SphereCast(rayOrigin, 1f, direction, out hit, (rangeDistance + rangeRadius * 2f) , layerFilterForRail, QueryTriggerInteraction.Ignore);
                    isRayChecked = Physics.CapsuleCast(rayOrigin + Vector3.up * rangeRadius, rayOrigin - Vector3.up * rangeRadius, 1f, direction, out hit, (rangeDistance + rangeRadius * 2f), layerFilterForRail, QueryTriggerInteraction.Ignore);

                    //Debug.Log("2 RayChecked : " + isRayChecked);
                    if (isRayChecked)
                    {
                        //Debug.Log("2 hit! : " + hit.collider.tag);
                        if ((hit.collider.gameObject.transform.parent.gameObject != currentRail) || interactionState.isRailJumping)
                        {
                            if (hit.collider.CompareTag("Rail"))
                            {
                                interactionState.isRailFounded = true;
                                _raycastHit = hit;
                                railStartPosiotion = _raycastHit.point;
                                railStartObject = _raycastHit.collider.gameObject;
                                return;
                            }
                        }
                    }

                    direction = (_raycastHit.point - rayOrigin).normalized;

                    isRayChecked = Physics.Raycast(rayOrigin, direction, out hit, (rangeDistance + rangeRadius * 2f), layerFilterForRail, QueryTriggerInteraction.Ignore);
                    //Debug.Log("3 RayChecked : " + isRayChecked);
                    if (isRayChecked)
                    {
                        //Debug.Log("3 hit! : " + hit.collider.tag);
                        if ((hit.collider.gameObject.transform.parent.gameObject != currentRail) || interactionState.isRailJumping)
                        {
                            if (hit.collider.CompareTag("Rail"))
                            {
                                interactionState.isRailFounded = true;
                                _raycastHit = hit;
                                railStartPosiotion = _raycastHit.point;
                                railStartObject = _raycastHit.collider.gameObject;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    rayOrigin = (lookAtObj.transform.position + cameraForwardXZ * 2f);
                    direction = -cameraForwardXZ.normalized;

                    isRayChecked = Physics.SphereCast(rayOrigin, rangeRadius, direction, out _raycastHit, rangeRadius * 2f, layerFilterForRail, QueryTriggerInteraction.Ignore);

                    if (isRayChecked)
                    {
                        //Debug.Log("4 hit! : " + _raycastHit.collider.tag);
                        if ((_raycastHit.collider.gameObject.transform.parent.gameObject != currentRail) || interactionState.isRailJumping)
                        {
                            if (_raycastHit.collider.CompareTag("Rail"))
                            {
                                interactionState.isRailFounded = true;
                                railStartPosiotion = _raycastHit.point;
                                railStartObject = _raycastHit.collider.gameObject;
                                return;
                            }
                        }
                    }
                }

                //Debug.Log("Not Found : " + false);
                interactionState.isRailFounded = false;
                return;
            }
        }

        public void StartRailAction()
        {
            playerState.IsAirJumping = false;
            playerState.WasAirDashing = false;
            playerState.IsGrounded = false;
            currentRail = railStartObject.transform.parent.gameObject;
            interactionState.isMovingToRail = true;
            interactionState.isRailTriggered = true;
            playerController.enabled = false;
        }

        void MoveToRail()
        {
            if((railStartPosiotion - transform.position).magnitude < departingRailOffset)
            {
                interactionState.isMovingToRail = false;
                return;
            }
            playerRigidbody.velocity = (railStartPosiotion - transform.position).normalized * movingToRailSpeed;
        }

        void RideOnRail()
        {
            interactionState.isRidingRail = true;
            railStartObject.transform.parent.gameObject.GetComponent<Rail>().RideOnRail(railStartPosiotion, railStartObject, this.gameObject);
            interactionState.isRailTriggered = false;
        }
        public void EscapeRailAction()
        {
            playerState.IsGrounded = false;
            currentRail.GetComponent<Rail>().EscapeRail(this.gameObject);
        }

        public void SwapRail()
        {
            currentRail.GetComponent<Rail>().EscapeRail(this.gameObject);
            StartRailAction();
        }

        public void StartRailJump()
        {
            interactionState.isRailJumpingUp = true;
            interactionState.isRailJumping = true;
        }
            
        public void JumpOnRail()
        {
            Vector3 localPos = transform.localPosition;

            if(interactionState.isRailJumpingUp)
            {
                localPos.y += railJumpSpeed * Time.fixedDeltaTime;
                if (localPos.y > railJumpHeight)
                {
                    interactionState.isRailJumpingUp = false;
                }
            }
            else
            {
                localPos.y -= railJumpSpeed * Time.fixedDeltaTime;
                if (localPos.y < departingRailOffset)
                {
                    localPos.y = 0;
                    interactionState.isRailJumping = false;
                }
            }

            transform.localPosition = localPos;
        }

        void SendInfoUI()
        {
            if(detectedRail.Count > 0)
            {
                if (interactionState.isRailFounded)
                {
                    railStartObject.transform.parent.gameObject.GetComponentInChildren<TargetIndicator>().SetUI(true, true, _raycastHit.point, mainCamera);
                    foreach (var rail in detectedRail.Keys)
                    {
                        if (rail != railStartObject.transform.parent.gameObject)
                        {
                            rail.GetComponentInChildren<TargetIndicator>().SetUI(false, false, Vector3.zero, mainCamera);
                        }
                    }
                }
                else
                {
                    foreach (var rail in detectedRail.Keys)
                    {
                        rail.GetComponentInChildren<TargetIndicator>().SetUI(false, false, Vector3.zero, mainCamera);
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Rail"))
            {
                if (detectedRail.Count > 0)
                {
                    bool isNewRail = true;
                    foreach (var rail in detectedRail)
                    {
                        if (rail.Key == other.transform.parent.transform.parent.gameObject)
                        {
                            detectedRail[rail.Key] = rail.Value + 1;
                            isNewRail = false;
                            break;
                        }
                    }

                    if (isNewRail)
                    {
                        detectedRail.Add(other.transform.parent.transform.parent.gameObject, 1);
                    }
                }
                else
                {
                    detectedRail.Add(other.transform.parent.transform.parent.gameObject, 1);
                }

                interactionState.railTriggerDetectionNum++;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Rail"))
            {
                if (detectedRail.Count > 0)
                {
                    bool isNewRail = true;
                    foreach (var rail in detectedRail)
                    {
                        if (rail.Key == other.transform.parent.transform.parent.gameObject)
                        {
                            detectedRail[rail.Key] = rail.Value - 1;
                            if(rail.Value < 1)
                            {
                                detectedRail.Remove(rail.Key);
                            }

                            isNewRail = false;
                            break;
                        }
                    }

                    if (isNewRail)
                    {
                        Debug.Log("레일 갯수 오류");
                    }
                }

                interactionState.railTriggerDetectionNum--;
            }
        }

        void MakeGizmoVecs()
        {
            hVision = mainCamera.transform.forward;

            startCenter = lookAtObj.transform.position;
            //startUps = startCenter + mainCamera.transform.up;
            //startDowns = startCenter - mainCamera.transform.up;
            //startLefts = startCenter - mainCamera.transform.right;
            //startRights = startCenter + mainCamera.transform.right;

            startUp = startCenter + mainCamera.transform.up * rangeRadius;
            startDown = startCenter - mainCamera.transform.up * rangeRadius;
            startLeft = startCenter - mainCamera.transform.right * rangeRadius;
            startRight = startCenter + mainCamera.transform.right * rangeRadius;

            endCenter = startCenter + hVision * rangeDistance;
            //endUps = endCenter + mainCamera.transform.up;
            //endDowns = endCenter - mainCamera.transform.up;
            //endLefts = endCenter - mainCamera.transform.right;
            //endRights = endCenter + mainCamera.transform.right;

            endUp = endCenter + mainCamera.transform.up * rangeRadius;
            endDown = endCenter - mainCamera.transform.up * rangeRadius;
            endLeft = endCenter - mainCamera.transform.right * rangeRadius;
            endRight = endCenter + mainCamera.transform.right * rangeRadius;
        }

        private void OnDrawGizmos()
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

            //Gizmos.DrawLine(startUps, endUps);
            //Gizmos.DrawLine(startDowns, endDowns);
            //Gizmos.DrawLine(startRights, endRights);
            //Gizmos.DrawLine(startLefts, endLefts);

            //Gizmos.DrawWireSphere(startCenter, 1f);
            //Gizmos.DrawWireSphere(endCenter, 1f);
            
        }
    }
}
