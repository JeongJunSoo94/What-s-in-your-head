using RoyaleAssets.InflatablesGameDemo.Common;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Obstacles
{
    [RequireComponent(typeof(MovingPlatformMover))]
    public class MovingPlatformObstacle : ObstacleSurface
    {
        private MovingPlatformMover mover;

        private void Start()
        {
            mover = GetComponent<MovingPlatformMover>();
        }

        public override void UpdateDisplacement(ref SurfaceHitInfo surface, ref SurfaceDisplacement thisDisplacement, ref SurfaceDisplacement otherDisplacement)
        {
            mover.GetDisplacement(out Vector3 platformDeltaPosition, out Quaternion platformDeltaRotation);

            if (Vector3.Dot(platformDeltaPosition.normalized, -surface.normal) < 0.2)
            {
                return;
            }

            Vector3 localPosition = surface.point - transform.position;
            Vector3 deltaPosition = platformDeltaPosition + platformDeltaRotation * localPosition - localPosition;

            otherDisplacement.deltaPosition = deltaPosition;
        }
    }
}
