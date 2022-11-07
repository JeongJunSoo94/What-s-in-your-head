using RoyaleAssets.InflatablesGameDemo.Common;
using RoyaleAssets.InflatablesGameDemo.Obstacles;
using System;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Character
{
    [RequireComponent(typeof(RoyaleCharacterMover))]
    public class RoyaleCharacterMotor : MonoBehaviour
    {        
        [SerializeField] Parameters parameters = Parameters.Default;

        public Vector3 currentSpeed = Vector3.zero;
        public bool IsGraunded { get; private set; }

        private float nextUngroundedTime = -1f;

        private float GroundClampSpeed => -Mathf.Tan(Mathf.Deg2Rad * mover.maxFloorAngle) * parameters.moveSpeed;
        private readonly ObstacleSurface[] _obstaclesContainer = new ObstacleSurface[5];

        private RoyaleCharacterMover mover;

        private void Start()
        {
            mover = GetComponent<RoyaleCharacterMover>();
        }

        public Vector3 UpdateMovement(Vector3 moveDirection, bool isJump, bool groundDetected, bool isOnFloor)
        {
            Vector3 velocity = parameters.moveSpeed * moveDirection;

            if (IsSafelyGrounded(groundDetected, isOnFloor))
            {
                nextUngroundedTime = Time.time + Parameters.timeBeforeUngrounded;
            }

            IsGraunded = Time.time < nextUngroundedTime;

            if (IsGraunded && isJump)
            {
                currentSpeed += transform.up * parameters.jumpSpeed;
                nextUngroundedTime = -1f;
                IsGraunded = false;
            }

            if (IsGraunded)
            {
                mover.preventMovingUpSteepSlope = true;
                mover.canClimbSteps = true;

                currentSpeed = Vector3.zero;
                velocity += GroundClampSpeed * transform.up;
            }
            else
            {
                mover.preventMovingUpSteepSlope = false;
                mover.canClimbSteps = false;

                currentSpeed += transform.up * parameters.gravity * Time.deltaTime;

                if (currentSpeed.y < Parameters.minVerticalSpeed)
                {
                    currentSpeed.y = Parameters.minVerticalSpeed;
                }

                velocity += currentSpeed;
            }

            RotateTowards(velocity);
            mover.Move(velocity * Time.deltaTime);

            return velocity;
        }

        private bool IsSafelyGrounded(bool groundDetected, bool isOnFloor)
        {
            return groundDetected && isOnFloor && currentSpeed.y < 0.01f;
        }

        private void RotateTowards(Vector3 direction)
        {
            var upDir = Vector3.ProjectOnPlane(direction, transform.up);
            if (upDir.sqrMagnitude < 1E-06f)
            {
                return;
            }

            var toRotation = Quaternion.LookRotation(upDir, transform.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, parameters.rotationSpeed * Time.deltaTime);
        }

        [Serializable]
        public class Parameters
        {
            public const float minVerticalSpeed = -12f;
            public const float timeBeforeUngrounded = 0.1f;

            public readonly static Parameters Default = new Parameters()
            {
                moveSpeed = 5,
                jumpSpeed = 8,
                rotationSpeed = 720,
                gravity = -25
            };

            public float moveSpeed;
            public float jumpSpeed;
            public float rotationSpeed;
            public float gravity;
        }
    }
}