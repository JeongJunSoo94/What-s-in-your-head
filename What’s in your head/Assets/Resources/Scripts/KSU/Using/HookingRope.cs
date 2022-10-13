using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class HookingRope : MonoBehaviour
    {
        PlayerInteractionState interactionState;
        PlayerController3D playerController;
        CharacterState3D playerState;
        Rigidbody playerRigidbody;

        public float grappleSpeed = 10f;
        public float escapeGrapplePower = 10f; 
        Vector3 grappleVec;
        Vector3 targetPosition;

        // Start is called before the first frame update
        void Awake()
        {
            playerController = GetComponent<PlayerController3D>();
            playerState = GetComponent<CharacterState3D>();
            interactionState = GetComponent<PlayerInteractionState>();
            playerRigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            MoveToTarget();
        }

        public void Hook(GameObject targetObj)
        {
            playerController.enabled = false;
            playerState.IsAirJumping = false;
            playerState.IsGrounded = false;
            targetPosition = targetObj.GetComponent<HookableObject>().GetOffsetPosition();
            grappleVec = (targetPosition - transform.position).normalized * grappleSpeed;
            Vector3 lookVec = grappleVec;
            lookVec.y = 0;
            transform.LookAt(transform.position + lookVec);
            interactionState.isRidingHookingRope = true;
        }

        void MoveToTarget()
        {
            if(interactionState.isRidingHookingRope)
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
            interactionState.isRidingHookingRope = false;
            
            Vector3 inertiaVec = grappleVec;
            float jumpPower = inertiaVec.y;
            inertiaVec.y = 0;

            playerController.MakeinertiaVec(escapeGrapplePower, transform.forward.normalized);
            playerController.moveVec = Vector3.up * jumpPower;
            playerController.enabled = true;
        }
    }
}
