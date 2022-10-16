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

            if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
                mainCamera = this.gameObject.GetComponent<CameraController>().FindCamera(); // 멀티용
            else
                mainCamera = this.gameObject.GetComponent<CameraController_Single>().FindCamera(); // 싱글용
            layerForRail = ((1) + (1 << LayerMask.NameToLayer("Rail")));
            layerFilterForRail = ((-1) - (1 << LayerMask.NameToLayer("Player")));
        }

        private void Update()
        {
            SearchRail();
        }

        void SearchRail()
        {
            //if (interactionState.railTriggerDetectionNum > 0)
            //{
            //    MakeGizmoVecs();
            //    SearchWithSphereCast();
            //}

            MakeGizmoVecs();
            SearchWithSphereCast();
        }

        public void SearchWithSphereCast()
        {
            //ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            bool isRayChecked = Physics.SphereCast(rayOrigin.transform.position, rangeRadius, mainCamera.transform.forward, out _raycastHit, rangeDistance, LayerMask.NameToLayer("Rail"), QueryTriggerInteraction.Ignore);
            
            Debug.Log("1 RayChecked : " + isRayChecked);
            if (isRayChecked)
            {
                Debug.Log("1 hit! : " + _raycastHit.collider.tag);
                RaycastHit hit;
                isRayChecked = Physics.Raycast(rayOrigin.transform.position, mainCamera.transform.forward, out hit, (rangeDistance + rangeRadius) , layerFilterForRail, QueryTriggerInteraction.Ignore);

                Debug.Log("2 RayChecked : " + isRayChecked);
                if (isRayChecked)
                {
                    Debug.Log("2 hit! : " + _raycastHit.collider.tag);
                    if (hit.collider.CompareTag("Rail"))
                    {
                        interactionState.isRailFounded = true;
                        _raycastHit = hit;
                        railStartPosiotion = _raycastHit.point;
                        railStartObject = _raycastHit.collider.gameObject;
                        return;
                    }
                }

                Vector3 direction = Vector3.MoveTowards(rayOrigin.transform.position, _raycastHit.point, 1f);

                isRayChecked = Physics.Raycast(rayOrigin.transform.position, direction, out hit, (rangeDistance + rangeRadius), layerFilterForRail, QueryTriggerInteraction.Ignore);
                Debug.Log("3 RayChecked : " + isRayChecked);
                if (isRayChecked)
                {
                    Debug.Log("3 hit! : " + _raycastHit.collider.tag);
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
            currentRail.GetComponent<Rail>().EscapeRail(this.gameObject);
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.CompareTag("Rail"))
        //    {
        //        interactionState.railTriggerDetectionNum++;
        //    }
        //}
        //private void OnTriggerExit(Collider other)
        //{
        //    if (other.CompareTag("Rail"))
        //    {
        //        interactionState.railTriggerDetectionNum--;
        //    }
        //}

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
    }
}
