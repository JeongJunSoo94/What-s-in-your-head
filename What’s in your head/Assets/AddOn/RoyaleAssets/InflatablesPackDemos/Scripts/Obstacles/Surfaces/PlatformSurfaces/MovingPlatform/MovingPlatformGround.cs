using RoyaleAssets.InflatablesGameDemo.Common;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Obstacles
{
    [RequireComponent(typeof(MovingPlatformMover))]
    public class MovingPlatformGround : GroundSurface
    {
        private MovingPlatformMover mover;

        private void Start()
        {
            mover = GetComponent<MovingPlatformMover>();
        }

        public override void UpdateDisplacement(Vector3 point, Vector3 up, ref SurfaceDisplacement otherDisplacement)
        {
            mover.GetDisplacement(out Vector3 platformDeltaPosition, out Quaternion platformDeltaRotation);
            Vector3 localPosition = point - transform.position;
            Vector3 deltaPosition = platformDeltaPosition + platformDeltaRotation * localPosition - localPosition;

            platformDeltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
            angle *= Mathf.Sign(Vector3.Dot(axis, up));

            otherDisplacement.deltaPosition += deltaPosition;
            otherDisplacement.deltaRotation += angle;
        }
    }
}