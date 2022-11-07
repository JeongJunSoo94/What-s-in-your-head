using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Character
{
    public class RoyaleCharacterMover : MonoBehaviour
    {
        private CharacterController characterController;

        public float maxFloorAngle => characterController.slopeLimit;
        public bool preventMovingUpSteepSlope;
        public bool canClimbSteps;

        public bool isGrounded => characterController.isGrounded;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        public void Move(Vector3 motion)
        {
            characterController.Move(motion);
        }
    }
}