using RoyaleAssets.InflatablesGameDemo.Common;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Obstacles
{
    public class RigidbodyObstacle : ObstacleSurface
    {
        [SerializeField] float displacementFactor = 10;

        private Rigidbody rb;
        private Vector3 lastPosition;
        private Vector3 deltaPosition;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            lastPosition = transform.position;
        }

        private void Update()
        {
            deltaPosition = transform.position - lastPosition;
            lastPosition = transform.position;
        }

        public override void UpdateDisplacement(ref SurfaceHitInfo hitInfo, ref SurfaceDisplacement thisDisplacement, ref SurfaceDisplacement otherDisplacement)
        {
            var deltaForce = (displacementFactor / rb.mass) * Time.deltaTime * Vector3.ProjectOnPlane(thisDisplacement.deltaVelocity, Vector3.up);
            rb.AddForce(deltaForce, ForceMode.Force);

            otherDisplacement.deltaPosition = rb.mass * deltaPosition;
        }
    }
}
