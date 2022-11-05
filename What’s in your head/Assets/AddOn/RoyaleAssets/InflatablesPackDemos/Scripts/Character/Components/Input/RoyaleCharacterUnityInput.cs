using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Character
{
    [RequireComponent(typeof(RoyaleCharacterCapsule))]
    public class RoyaleCharacterUnityInput : RoyaleCharacterInput
    {
        [SerializeField] Transform forwardTransform;

        private RoyaleCharacterCapsule capsule;

        private void Start()
        {
            capsule = GetComponent<RoyaleCharacterCapsule>();
        }

        public override void SetForwardTransform(Transform forward)
        {
            forwardTransform = forward;
        }

        public override Vector3 GetInputMove()
        {
            if (forwardTransform == null)
            {
                return Vector3.zero;
            }

            if(capsule == null)
            {
                capsule = GetComponent<RoyaleCharacterCapsule>();
            }

            var inputVec = GetInputVector();
            var moveMove = CameraRelativeVectorFromInput(inputVec.x, inputVec.z, forwardTransform.forward, capsule.UpDirection);
            return moveMove;
        }

        private Vector3 GetInputVector()
        {
            if (!enabled)
            {
                return Vector3.zero;
            }

            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            return new Vector3(horizontal, 0, vertical);
        }

        public override float GetInputJump()
        {
            if (!enabled)
            {
                return 0;
            }

            var isJump = Input.GetButtonDown("Jump");
            return isJump ? 1 : 0;
        }

        public static Vector3 CameraRelativeVectorFromInput(float x, float y, Vector3 vector, Vector3 up)
        {
            Vector3 forward = Vector3.ProjectOnPlane(vector, up).normalized;
            Vector3 right = Vector3.Cross(up, forward);

            return x * right + y * forward;
        }
    }
}