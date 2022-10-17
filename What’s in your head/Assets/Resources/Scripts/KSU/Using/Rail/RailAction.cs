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
        PlayerController3D playerController;
        CharacterState3D playerState;
        PlayerInteractionState interactionState;

        Camera mainCamera;
        [SerializeField] GameObject lookAtObj;
        GameObject detectedRail;
        public Ray ray;
        RaycastHit _raycastHit;
        public LayerMask layerFilterForRail;
        public LayerMask layerForRail;

        public float rangeRadius = 5f;
        public float rangeDistance = 5f;

        public GameObject currentRail;
        public GameObject railStartObject;
        public Vector3 railStartPosiotion = Vector3.zero;

        public float escapingRopeSpeed = 6f;
        public float escapingRoDelayTime = 1f;



        /// <summary> Gizmo
        Vector3 startCenter;
        Vector3 startUp;
        Vector3 startDown;
        Vector3 startLeft;
        Vector3 startRight;

        Vector3 startUps;
        Vector3 startDowns;
        Vector3 startLefts;
        Vector3 startRights;

        public Vector3 hVision;

        Vector3 endCenter;
        Vector3 endUp;
        Vector3 endDown;
        Vector3 endLeft;
        Vector3 endRight;

        Vector3 endUps;
        Vector3 endDowns;
        Vector3 endLefts;
        Vector3 endRights;
        /// </summary>

        void Awake()
        {
            playerController = GetComponent<PlayerController3D>();
            playerState = GetComponent<CharacterState3D>();
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
            //ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 cameraForwardXZ = mainCamera.transform.forward;
            cameraForwardXZ.y = 0;
            Vector3 rayOrigin = (lookAtObj.transform.position - cameraForwardXZ);
            Vector3 rayEnd = (lookAtObj.transform.position + mainCamera.transform.forward * (rangeDistance + rangeRadius * 2f));
            Vector3 direction = (rayEnd - rayOrigin).normalized;
            bool isRayChecked = Physics.SphereCast(rayOrigin, rangeRadius, direction, out _raycastHit, rangeDistance, layerForRail, QueryTriggerInteraction.Ignore);
            
            Debug.Log("1 RayChecked : " + isRayChecked);
            if (isRayChecked)
            {
                Debug.Log("1 hit! : " + _raycastHit.collider.tag);
                RaycastHit hit;
                isRayChecked = Physics.SphereCast(rayOrigin, 1f, direction, out hit, (rangeDistance + rangeRadius * 2f) , layerFilterForRail, QueryTriggerInteraction.Ignore);

                Debug.Log("2 RayChecked : " + isRayChecked);
                if (isRayChecked)
                {
                    Debug.Log("2 hit! : " + hit.collider.tag);
                    if (hit.collider.CompareTag("Rail"))
                    {
                        interactionState.isRailFounded = true;
                        _raycastHit = hit;
                        railStartPosiotion = _raycastHit.point;
                        railStartObject = _raycastHit.collider.gameObject;
                        return;
                    }
                }

                direction = (_raycastHit.point - rayOrigin).normalized;

                isRayChecked = Physics.Raycast(rayOrigin, direction, out hit, (rangeDistance + rangeRadius * 2f), layerFilterForRail, QueryTriggerInteraction.Ignore);
                Debug.Log("3 RayChecked : " + isRayChecked);
                if (isRayChecked)
                {
                    Debug.Log("3 hit! : " + hit.collider.tag);
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
            else
            {
                rayOrigin = (lookAtObj.transform.position + cameraForwardXZ * 2f);
                direction = -cameraForwardXZ.normalized;

                isRayChecked = Physics.SphereCast(rayOrigin, rangeRadius, direction, out _raycastHit, rangeRadius * 2f, layerFilterForRail, QueryTriggerInteraction.Ignore);

                if (isRayChecked)
                {
                    Debug.Log("4 hit! : " + _raycastHit.collider.tag);
                    if (_raycastHit.collider.CompareTag("Rail"))
                    {
                        interactionState.isRailFounded = true;
                        railStartPosiotion = _raycastHit.point;
                        railStartObject = _raycastHit.collider.gameObject;
                        return;
                    }
                }
            }

            Debug.Log("Not Found : " + false);
            interactionState.isRailFounded = false;
            return;
        }

        public void StartRailAction()
        {
            playerState.IsAirJumping = false;
            playerState.WasAirDashing = false;
            playerState.IsGrounded = false;
            railStartObject.transform.parent.gameObject.GetComponent<Rail>().RideOnRail(railStartPosiotion, railStartObject, this.gameObject);
        }

        public void EscapeRailAction()
        {
            playerState.IsGrounded = false;
            currentRail.GetComponent<Rail>().EscapeRail(this.gameObject);
        }

        void SendInfoUI()
        {
            if (interactionState.isRailFounded)
            {
                if(interactionState.railTriggerDetectionNum> 0)
                {
                    detectedRail.GetComponentInChildren<TargetIndicator>().SetUI(true, true, _raycastHit.point, mainCamera);
                }
                
            }
            else
            {
                if(detectedRail != null)
                {
                    detectedRail.GetComponentInChildren<TargetIndicator>().SetUI(false, false, Vector3.zero, mainCamera);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Rail"))
            {
                if(interactionState.railTriggerDetectionNum < 1)
                {
                    Debug.Log("할아버지 입장 : " + other.transform.parent.transform.parent.gameObject);
                    detectedRail = other.transform.parent.transform.parent.gameObject;
                }
                interactionState.railTriggerDetectionNum++;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Rail"))
            {
                if (interactionState.railTriggerDetectionNum == 1)
                {
                    Debug.Log("할아버지 퇴장 : " + other.transform.parent.transform.parent.gameObject);
                }
                interactionState.railTriggerDetectionNum--;
            }
        }

        void MakeGizmoVecs()
        {
            hVision = mainCamera.transform.forward;

            startCenter = lookAtObj.transform.position;
            startUps = startCenter + mainCamera.transform.up;
            startDowns = startCenter - mainCamera.transform.up;
            startLefts = startCenter - mainCamera.transform.right;
            startRights = startCenter + mainCamera.transform.right;

            startUp = startCenter + mainCamera.transform.up * rangeRadius;
            startDown = startCenter - mainCamera.transform.up * rangeRadius;
            startLeft = startCenter - mainCamera.transform.right * rangeRadius;
            startRight = startCenter + mainCamera.transform.right * rangeRadius;

            endCenter = startCenter + hVision * rangeDistance;
            endUps = endCenter + mainCamera.transform.up;
            endDowns = endCenter - mainCamera.transform.up;
            endLefts = endCenter - mainCamera.transform.right;
            endRights = endCenter + mainCamera.transform.right;

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

            Gizmos.DrawLine(startUps, endUps);
            Gizmos.DrawLine(startDowns, endDowns);
            Gizmos.DrawLine(startRights, endRights);
            Gizmos.DrawLine(startLefts, endLefts);

            Gizmos.DrawWireSphere(startCenter, 1f);
            Gizmos.DrawWireSphere(endCenter, 1f);
        }
    }
}
