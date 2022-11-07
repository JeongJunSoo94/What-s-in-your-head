using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Character
{
    [RequireComponent(typeof(CharacterController))]
    public class RoyaleCharacterCapsule : MonoBehaviour
    {
        private CharacterController controller;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        public float Height => controller.height;
        public float Radius => controller.radius;
        public Vector3 Center => transform.position + transform.TransformVector(controller.center);
        public Vector3 UpDirection => controller.transform.up;
    }
}
