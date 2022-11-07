using RoyaleAssets.InflatablesGameDemo.Common;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Obstacles
{
    [RequireComponent(typeof(BalancingPlatformCenterMover))]
    public class BalancingPlatformCenter : GroundSurface
    {
        [SerializeField] Transform balanceCenter = null;
        [SerializeField] float centerThreshold = 0.5f;

        private BalancingPlatformCenterMover mover;

        private void Start()
        {
            mover = GetComponent<BalancingPlatformCenterMover>();
            balanceCenter.parent = transform.parent;
        }

        public override void UpdateDisplacement(Vector3 hitPoint, Vector3 _, ref SurfaceDisplacement otherDisplacement)
        {
            ApplyDisplacement(hitPoint);
        }

        private void ApplyDisplacement(Vector3 hitPoint)
        {
            var projectedHitPoint = Vector3.ProjectOnPlane(hitPoint, balanceCenter.up);
            var hitVector = projectedHitPoint - balanceCenter.position;
            var hitLength = hitVector.magnitude;

            if (hitLength < centerThreshold)
            {
                return;
            }

            var upRotation = Vector3.Cross(balanceCenter.up, hitVector);
            mover.Rotate(hitLength, upRotation);
        }
    }
}