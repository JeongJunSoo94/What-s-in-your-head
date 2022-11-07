using RoyaleAssets.InflatablesGameDemo.Common;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Obstacles
{
    [RequireComponent(typeof(BalancingPlatformAxesMover))]
    public class BalancingPlatformAxes : GroundSurface
    {
        [SerializeField] Transform balanceAxis = null;

        private BalancingPlatformAxesMover mover;

        private void Start()
        {
            mover = GetComponent<BalancingPlatformAxesMover>();
            balanceAxis.parent = transform.parent;
        }

        public override void UpdateDisplacement(Vector3 hitPoint, Vector3 _, ref SurfaceDisplacement otherDisplacement)
        {
            ApplyDisplacement(hitPoint);
        }

        private void ApplyDisplacement(Vector3 hitPoint)
        {
            var axisForward = balanceAxis.forward;
            var axixsRight = balanceAxis.right;
            var hitVector = hitPoint - balanceAxis.position;

            var projectedVector = Vector3.Project(hitVector, axixsRight);
            var projectedLength = projectedVector.magnitude;

            var angleDelta = -Mathf.Sign(Vector3.Dot(axixsRight, hitVector)) * projectedLength;

            mover.Rotate(angleDelta, axisForward);
        }
    }
}