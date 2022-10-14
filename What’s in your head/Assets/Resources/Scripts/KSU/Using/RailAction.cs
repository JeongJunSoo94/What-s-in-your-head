using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.Utility;

using YC.Camera_;
using YC.Camera_Single;

namespace KSU
{
    public class RailAction : MonoBehaviour
    {
        PlayerController3D playerController;
        CharacterState3D playerState;
        PlayerInteractionState interactionState;

        Camera mainCamera;
        [SerializeField] GameObject rayOrigin;
        public Ray ray;
        RaycastHit _raycastHit;
        LayerMask layerFilterForRail;
        LayerMask layerForRail;

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

        public Vector3 hVision;

        Vector3 endCenter;
        Vector3 endUp;
        Vector3 endDown;
        Vector3 endLeft;
        Vector3 endRight;
        /// </summary>

        void Awake()
        {
            playerController = GetComponent<PlayerController3D>();
            playerState = GetComponent<CharacterState3D>();
            interactionState = GetComponent<PlayerInteractionState>();
            mainCamera = playerController.mainCamera;
            layerForRail = ((1) + (1 << LayerMask.NameToLayer("Interactable")));
            layerFilterForRail = ((-1) - (1 << LayerMask.NameToLayer("Player")));
        }

        void SearchRail()
        {
            if (interactionState.isRailDetected)
            {
                MakeGizmoVecs();
                SearchWithSphereCast();
            }
        }

        public void SearchWithSphereCast()
        {
            //ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            bool isRayChecked = Physics.SphereCast(rayOrigin.transform.position, rangeRadius, mainCamera.transform.forward, out _raycastHit, rangeDistance, layerForRail, QueryTriggerInteraction.Ignore);


            if (isRayChecked)
            {
                RaycastHit hit;
                isRayChecked = Physics.Raycast(rayOrigin.transform.position, mainCamera.transform.forward, out hit, rangeDistance, layerFilterForRail, QueryTriggerInteraction.Ignore);
                if (isRayChecked)
                {
                    if (hit.collider.tag == "Rail")
                    {
                        interactionState.isRailFounded = true;
                        _raycastHit = hit;
                        railStartPosiotion = _raycastHit.point;
                        railStartObject = _raycastHit.collider.gameObject;
                        return;
                    }
                }

                Vector3 direction = (_raycastHit.point - rayOrigin.transform.position).normalized;
                isRayChecked = Physics.Raycast(rayOrigin.transform.position, direction, out hit, rangeDistance, layerFilterForRail, QueryTriggerInteraction.Ignore);

                if (isRayChecked)
                {
                    if (hit.collider.tag == "Rail")
                    {
                        interactionState.isRailFounded = true;
                        _raycastHit = hit;
                        railStartPosiotion = _raycastHit.point;
                        railStartObject = _raycastHit.collider.gameObject;
                        return;
                    }
                }
            }
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
            currentRail.GetComponent<Rail>().EscapeRail(this.gameObject);
        }


        void MakeGizmoVecs()
        {
            hVision = mainCamera.transform.forward;

            startCenter = rayOrigin.transform.position;
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

        private void OnDrawGizmos()
        {
            if (interactionState.isRailFounded)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_raycastHit.point, 1f);
            }

            {
                Gizmos.DrawLine(startUp, endUp);
                Gizmos.DrawLine(startDown, endDown);
                Gizmos.DrawLine(startRight, endRight);
                Gizmos.DrawLine(startLeft, endLeft);

                Gizmos.DrawWireSphere(startCenter, rangeRadius);
                Gizmos.DrawWireSphere(endCenter, rangeRadius);
            }
        }
    }
}
