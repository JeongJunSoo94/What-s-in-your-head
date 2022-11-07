using RoyaleAssets.InflatablesGameDemo.Common;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Character
{
    [RequireComponent(typeof(RoyaleCharacterInput))]
    [RequireComponent(typeof(RoyaleCharacterMotor))]
    [RequireComponent(typeof(RoyaleGroundDetector))]
    public class RoyaleCharacterController : MonoBehaviour
    {
        private RoyaleCharacterInput input;
        private RoyaleCharacterMotor motor;
        private RoyaleGroundDetector groundDetector;

        private RoyaleCharacterDisplacement displacement;
        private RoyaleCharacterAnimator animator;

        private void Start()
        {
            input = GetComponent<RoyaleCharacterInput>();
            motor = GetComponent<RoyaleCharacterMotor>();
            groundDetector = GetComponent<RoyaleGroundDetector>();

            displacement = GetComponent<RoyaleCharacterDisplacement>();
            animator = GetComponent<RoyaleCharacterAnimator>();
        }

        private void Update()
        {
            // Input
            var moveDirection = input ? input.GetInputMove() : Vector3.zero;
            var jump = input ? input.GetInputJump() : 0;

            // GraundDetector
            var groundDetected = groundDetector.DetectGround(out GroundHitInfo groundHitInfo);
            var isGrounded = motor.IsGraunded;

            // Motor
            var velocity = motor.UpdateMovement(moveDirection, jump > 0, groundDetected, groundHitInfo.isOnFloor);

            // Displacement
            if (displacement)
            {
                displacement.UpdateDisplacement(isGrounded, groundDetected, groundHitInfo, velocity);
            }

            // Animate
            if (animator)
            {
                animator.UpdateAnimation(moveDirection, isGrounded);
            }
        }
    }
}