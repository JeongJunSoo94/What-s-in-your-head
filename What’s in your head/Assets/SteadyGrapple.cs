using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class SteadyGrapple : MonoBehaviour
    {
        SteadyInteractionState steadyInteractionState;
        PlayerController playerController;
        PlayerState playerState;
        Rigidbody playerRigidbody;

        public float grappleSpeed = 10f;
        public float escapeGrapplePower = 10f;

        Camera mainCamera;
        [SerializeField] GameObject rayOrigin;
        public Ray ray;
        RaycastHit _raycastHit;
        LayerMask layerFilterForGrapple;
        LayerMask layerForGrapple;

        public float rangeRadius = 5f;
        public float rangeDistance = 5f;

        public GameObject hookableTarget;

        Vector3 grappleVec;
        Vector3 targetPosition;

        // Start is called before the first frame update
        void Awake()
        {
            playerController = GetComponent<PlayerController>();
            playerState = GetComponent<PlayerState>();
            steadyInteractionState = GetComponent<SteadyInteractionState>();
            playerRigidbody = GetComponent<Rigidbody>();
            mainCamera = playerController.mainCamera;

            layerForGrapple = ((1) + (1 << LayerMask.NameToLayer("Interactable")));
            layerFilterForGrapple = ((-1) - (1 << LayerMask.NameToLayer("Player")) - (1 << LayerMask.NameToLayer("Rail")) - (1 << LayerMask.NameToLayer("Interactable")));
        }

        void Update()
        {
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

        public void SearchGrapplingObject()
        {
            //ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            bool rayChecked = Physics.SphereCast(rayOrigin.transform.position, rangeRadius, mainCamera.transform.forward, out _raycastHit, rangeDistance, layerForGrapple, QueryTriggerInteraction.Ignore);

            if (rayChecked)
            {
                RaycastHit hit;
                Vector3 direction = (_raycastHit.collider.gameObject.transform.position - rayOrigin.transform.position).normalized;
                rayChecked = Physics.Raycast(rayOrigin.transform.position, mainCamera.transform.forward, out hit, rangeDistance, layerFilterForGrapple, QueryTriggerInteraction.Ignore);
                if (_raycastHit.collider.tag == "GrapplingObject")
                {
                    hookableTarget = _raycastHit.collider.gameObject;
                    steadyInteractionState.isGrapplingObjectFounded = true;
                    return;
                }
            }

            steadyInteractionState.isGrapplingObjectFounded = false;
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
            steadyInteractionState.isRidingHookingRope = true;
        }

        void MoveToTarget()
        {
            if (steadyInteractionState.isRidingHookingRope)
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
            steadyInteractionState.isRidingHookingRope = false;

            Vector3 inertiaVec = grappleVec;
            float jumpPower = inertiaVec.y;
            inertiaVec.y = 0;

            playerController.MakeinertiaVec(escapeGrapplePower, transform.forward.normalized);
            playerController.moveVec = Vector3.up * jumpPower;
            playerController.enabled = true;
        }
    }
}
