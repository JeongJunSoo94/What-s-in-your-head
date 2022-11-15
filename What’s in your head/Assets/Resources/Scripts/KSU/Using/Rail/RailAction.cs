using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.Utility;

using YC.Camera_;
using YC.Camera_Single;
using JCW.UI.InGame.Indicator;

namespace KSU
{
    public class RailAction : MonoBehaviour
    {
        Rigidbody playerRigidbody;
        PlayerController playerController;
        PlayerState playerState;
        public PlayerInteractionState interactionState;
        Animator animator;

        public Camera mainCamera;
        [SerializeField] Transform lookAtObj;
        Dictionary <GameObject, int> detectedRail = new();
        RaycastHit _raycastHit;
        [SerializeField] LayerMask layerFilterForRail;
        [SerializeField] LayerMask layerForRail;

        float escapingRailDelayTime = 0.7f;
        public GameObject currentRail;
        public GameObject railStartObject;
        Vector3 railStartPosiotion = Vector3.zero;
        

        [Header("_______변경 가능 값_______")]
        [Header("레일 탐지 범위(캡슐) 반지름")]
        public float rangeRadius = 5f;
        [Header("레일 탐지 범위(캡슐) 길이(거리)")]
        public float rangeDistance = 15f;
        [Header("레일 상호작용키 사용후 레일까지 날아가는 속력(갈아타기 동일)")]
        public float movingToRailSpeed = 20f;
        [Header("레일 상호작용 후 레일 도달 오프셋")]
        public float departingRailOffset = 0.5f;
        [Header("레일 타는 중 점프 높이")]
        public float railJumpHeight = 5f;
        [Header("레일 타는 중 점프 속도(상승 하강 동일)")]
        public float railJumpSpeed = 20f;



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

        void Awake()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            playerController = GetComponent<PlayerController>();
            playerState = GetComponent<PlayerState>();
            interactionState = GetComponent<PlayerInteractionState>();
            animator = GetComponent<Animator>();

            mainCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // 멀티용
            lookAtObj = this.gameObject.GetComponent<CameraController>().lookatBackObj;
            //layerForRail = ((1) + (1 << LayerMask.NameToLayer("Rail")));
            //layerFilterForRail = ((-1) - (1 << LayerMask.NameToLayer("Player")));
        }

        public void InitDictionary()
        {
            detectedRail.Clear();
        }
        private void Update()
        {
            if (playerState.isMine)
            {
                SearchWithSphereCast();
                SendInfoUI();
            }
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
            if (interactionState.isMovingToRail)
            {
                MoveToRail();
            }

            if (interactionState.isRidingRail)
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

        public void SearchWithSphereCast()
        {
            if (interactionState.railTriggerDetectionNum > 0)
            {
                if (!interactionState.isRailTriggered && !interactionState.isMoveFromRail)
                {
                    if (playerState.aim)
                    {
                        Vector3 cameraForwardXZ = mainCamera.transform.forward;
                        cameraForwardXZ.y = 0;
                        cameraForwardXZ = cameraForwardXZ.normalized;
                        Vector3 rayOrigin = mainCamera.transform.position + mainCamera.transform.forward * rangeRadius / 2f;
                        //Vector3 rayEnd = (mainCamera.transform.position + mainCamera.transform.forward * (rangeDistance + rangeRadius * 2f));
                        Vector3 direction = mainCamera.transform.forward;
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
                                bool railChecked = false;
                                if (hit.collider.CompareTag("Rail"))
                                {
                                    if ((hit.collider.gameObject.transform.parent.gameObject != currentRail) || interactionState.isRailJumping)
                                    {
                                        interactionState.isRailFounded = true;
                                        _raycastHit = hit;
                                        railStartPosiotion = _raycastHit.point;
                                        railStartObject = _raycastHit.collider.gameObject;
                                        railChecked = true;
                                    }
                                }

                                isRayChecked = Physics.SphereCast(rayOrigin, 1f, direction, out hit, (rangeDistance + rangeRadius * 2f), layerFilterForRail, QueryTriggerInteraction.Ignore);

                                if (isRayChecked)
                                {
                                    if (hit.collider.CompareTag("Rail"))
                                    {
                                        if ((hit.collider.gameObject.transform.parent.gameObject != currentRail) || interactionState.isRailJumping)
                                        {
                                            interactionState.isRailFounded = true;
                                            _raycastHit = hit;
                                            railStartPosiotion = _raycastHit.point;
                                            railStartObject = _raycastHit.collider.gameObject;
                                            railChecked = true;
                                        }
                                    }
                                }
                                if (railChecked)
                                    return;
                            }

                            direction = (_raycastHit.point - rayOrigin).normalized;

                            isRayChecked = Physics.Raycast(rayOrigin, direction, out hit, (rangeDistance + rangeRadius * 2f), layerFilterForRail, QueryTriggerInteraction.Ignore);
                            //Debug.Log("3 RayChecked : " + isRayChecked);
                            if (isRayChecked)
                            {
                                //Debug.Log("3 hit! : " + hit.collider.tag);
                                if (hit.collider.CompareTag("Rail"))
                                {
                                    if ((hit.collider.gameObject.transform.parent.gameObject != currentRail) || interactionState.isRailJumping)
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
                            rayOrigin = (lookAtObj.transform.position + cameraForwardXZ * rangeRadius);
                            direction = -cameraForwardXZ.normalized;

                            isRayChecked = Physics.SphereCast(rayOrigin, rangeRadius, direction, out _raycastHit, rangeRadius * 2f, layerFilterForRail, QueryTriggerInteraction.Ignore);

                            if (isRayChecked)
                            {
                                //Debug.Log("4 hit! : " + _raycastHit.collider.tag);
                                if (_raycastHit.collider.CompareTag("Rail"))
                                {
                                    if ((_raycastHit.collider.gameObject.transform.parent.gameObject != currentRail) || interactionState.isRailJumping)
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
                    else
                    {
                        Vector3 cameraForwardXZ = mainCamera.transform.forward;
                        cameraForwardXZ.y = 0;
                        cameraForwardXZ = cameraForwardXZ.normalized;
                        Vector3 rayOrigin = (lookAtObj.transform.position - mainCamera.transform.forward * rangeRadius);
                        //Vector3 rayEnd = (lookAtObj.transform.position + mainCamera.transform.forward * (rangeDistance + rangeRadius * 2f));
                        Vector3 direction = mainCamera.transform.forward;
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
                                bool railChecked = false;
                                if (hit.collider.CompareTag("Rail"))
                                {
                                    if ((hit.collider.gameObject.transform.parent.gameObject != currentRail) || interactionState.isRailJumping)
                                    {
                                        interactionState.isRailFounded = true;
                                        _raycastHit = hit;
                                        railStartPosiotion = _raycastHit.point;
                                        railStartObject = _raycastHit.collider.gameObject;
                                        railChecked = true;
                                    }
                                }

                                isRayChecked = Physics.SphereCast(rayOrigin, 1f, direction, out hit, (rangeDistance + rangeRadius * 2f), layerFilterForRail, QueryTriggerInteraction.Ignore);

                                if (isRayChecked)
                                {
                                    if (hit.collider.CompareTag("Rail"))
                                    {
                                        if ((hit.collider.gameObject.transform.parent.gameObject != currentRail) || interactionState.isRailJumping)
                                        {
                                            interactionState.isRailFounded = true;
                                            _raycastHit = hit;
                                            railStartPosiotion = _raycastHit.point;
                                            railStartObject = _raycastHit.collider.gameObject;
                                            railChecked = true;
                                        }
                                    }
                                }
                                if (railChecked)
                                    return;
                            }

                            direction = (_raycastHit.point - rayOrigin).normalized;

                            isRayChecked = Physics.Raycast(rayOrigin, direction, out hit, (rangeDistance + rangeRadius * 2f), layerFilterForRail, QueryTriggerInteraction.Ignore);
                            //Debug.Log("3 RayChecked : " + isRayChecked);
                            if (isRayChecked)
                            {
                                if (hit.collider.CompareTag("Rail"))
                                {
                                    if ((hit.collider.gameObject.transform.parent.gameObject != currentRail) || interactionState.isRailJumping)
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
                            rayOrigin = (lookAtObj.transform.position + cameraForwardXZ * rangeRadius);
                            direction = -cameraForwardXZ.normalized;

                            isRayChecked = Physics.SphereCast(rayOrigin, rangeRadius, direction, out _raycastHit, rangeRadius * 2f, layerFilterForRail, QueryTriggerInteraction.Ignore);

                            if (isRayChecked)
                            {
                                if (currentRail != null)
                                {
                                    if (_raycastHit.collider.CompareTag("Rail"))
                                    {
                                        if ((_raycastHit.collider.gameObject.transform.parent.gameObject != currentRail) || interactionState.isRailJumping)
                                        {
                                            interactionState.isRailFounded = true;
                                            railStartPosiotion = _raycastHit.point;
                                            railStartObject = _raycastHit.collider.gameObject;
                                            return;
                                        }
                                    }
                                }
                                else
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
                else
                {
                    interactionState.isRailFounded = false;
                }
            }
            interactionState.isRailFounded = false;
        }

        public bool GetWhetherFoundRail()
        {
            return interactionState.isRailFounded;
        }

        public float StartRailAction()
        {
            if (railStartObject == null)
                return 0f;
            currentRail = railStartObject.transform.parent.gameObject;
            playerController.characterState.isRiding = true;
            playerRigidbody.velocity = Vector3.zero;
            Vector3 lookVec = (railStartPosiotion - transform.position);
            lookVec.y = 0;
            transform.LookAt(transform.position + lookVec);
            interactionState.isMovingToRail = true;
            interactionState.isRailTriggered = true;
            interactionState.isRidingRail = false;
            playerController.SetOffCollider();
            return Vector3.Distance(railStartPosiotion, transform.position)/movingToRailSpeed;
        }

        void MoveToRail()
        {
            if((railStartPosiotion - transform.position).magnitude < departingRailOffset)
            {
                animator.SetBool("isRidingRail", true);
                interactionState.isMovingToRail = false;
                interactionState.isRidingRail = true;
                playerRigidbody.velocity = Vector3.zero;
                railStartObject.transform.parent.gameObject.GetComponent<Rail>().RideOnRail(railStartPosiotion, railStartObject, this.gameObject);
                interactionState.isRailTriggered = false;
                playerController.SetOnCollider();
                return;
            }
            playerRigidbody.velocity = (railStartPosiotion - transform.position).normalized * movingToRailSpeed;
        }

        void RideOnRail()
        {
            
        }
        public bool GetWhetherFailedRiding()
        {
            if (!interactionState.isRidingRail && !interactionState.isMovingToRail)
                return true;
            return false;
        }
        public void EscapeRailAction()
        {
            if(currentRail != null)
            {
                StopCoroutine(nameof(DelayEscape));
                StartCoroutine(nameof(DelayEscape));
                currentRail.GetComponent<Rail>().EscapeRail(this.gameObject, false);
            }
        }

        IEnumerator DelayEscape()
        {
            interactionState.isMoveFromRail = true;
            yield return new WaitForSeconds(escapingRailDelayTime);
            interactionState.isMoveFromRail = false;
        }

        public void SetBoolEscapeRail()
        {
            animator.SetBool("isMoveToRail", false);
            animator.SetBool("isRidingRail", false);
        }

        public float SwapRail()
        {
            if (currentRail != null)
            {
                currentRail.GetComponent<Rail>().EscapeRail(this.gameObject, true);
                return StartRailAction();
            }
            return 1f;
        }

        public void StartRailJump()
        {
            if(!interactionState.isRailJumping)
            {
                animator.SetBool("isRailJump", true);
                interactionState.isRailJumpingUp = true;
                interactionState.isRailJumping = true;
            }
        }

        public void ReSetRailJump()
        {
            animator.SetBool("isRailJump", false);
            interactionState.isRailJumping = false;
        }

        public void JumpOnRail()
        {
            float localPosY = transform.localPosition.y;

            if(interactionState.isRailJumpingUp)
            {
                localPosY += railJumpSpeed * Time.fixedDeltaTime;
                if (localPosY > railJumpHeight)
                {
                    interactionState.isRailJumpingUp = false;
                }
            }
            else
            {
                localPosY -= railJumpSpeed * Time.fixedDeltaTime;
                if (localPosY < departingRailOffset)
                {
                    localPosY = 0;
                    interactionState.isRailJumping = false;
                }
            }

            transform.localPosition = Vector3.up * localPosY;
        }

        void SendInfoUI()
        {
            if (detectedRail.Count > 0)
            {
                if (interactionState.isRailFounded)
                {
                    foreach (var rail in detectedRail)
                    {
                        if (rail.Key != railStartObject.transform.parent.gameObject)
                        {
                            rail.Key.GetComponentInChildren<OneIndicator>().SetUI(false, Vector3.zero);
                        }
                        else
                        {
                            rail.Key.GetComponentInChildren<OneIndicator>().SetUI(true, _raycastHit.point);
                        }
                    }
                }
                else
                {
                    foreach (var rail in detectedRail)
                    {
                        rail.Key.GetComponentInChildren<OneIndicator>().SetUI(false, Vector3.zero);
                    }
                }
            }
        }

        public void SendUIOff()
        {
            if (detectedRail.Count > 0)
            {
                foreach (var rail in detectedRail.Keys)
                {
                    if (rail != railStartObject.transform.parent.gameObject)
                    {
                        rail.GetComponentInChildren<OneIndicator>().SetUI(false, Vector3.zero);
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Rail"))
            {
                //if (detectedRail.ContainsKey(other.transform.parent.transform.parent.gameObject))
                //    return;
                if (detectedRail.Count > 0)
                {
                    //if (detectedRail.ContainsKey(other.transform.parent.transform.parent.gameObject))
                    //    return;
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
                    //bool isNewRail = true;
                    foreach (var rail in detectedRail)
                    {
                        if (rail.Key == other.transform.parent.transform.parent.gameObject)
                        {
                            detectedRail[rail.Key] = rail.Value - 1;
                            if(rail.Value < 2)
                            {
                                rail.Key.GetComponentInChildren<OneIndicator>().SetUI(false, Vector3.zero);
                                detectedRail.Remove(rail.Key);
                            }

                            //isNewRail = false;
                            break;
                        }
                    }

                    //if (isNewRail)
                    //{
                    //    Debug.Log("레일 갯수 오류");
                    //}
                }

                interactionState.railTriggerDetectionNum--;
            }
        }

    }
}
