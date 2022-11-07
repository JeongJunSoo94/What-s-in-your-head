using RoyaleAssets.InflatablesGameDemo.Common;
using RoyaleAssets.InflatablesGameDemo.Obstacles;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Character
{
    [RequireComponent(typeof(RoyaleCharacterMover))]
    [RequireComponent(typeof(RoyaleCharacterCapsule))]
    public class RoyaleGroundDetector : MonoBehaviour
    {
        [SerializeField]
        private float tolerance = 0.05f;

        private LayerMask collisionMask;

        private RoyaleCharacterMover mover;
        private RoyaleCharacterCapsule capsule;

        private void Start()
        {
            collisionMask = ~GameLayers.CharacterMask;

            capsule = GetComponent<RoyaleCharacterCapsule>();
            mover = GetComponent<RoyaleCharacterMover>();
        }

        public bool DetectGround(out GroundHitInfo groundInfo)
        {
            float radius = capsule.Radius;
            Vector3 sphereCastOrigin = capsule.Center;
            Vector3 upDirection = capsule.UpDirection;
            float minFloorUp = Mathf.Cos(Mathf.Deg2Rad * mover.maxFloorAngle);

            // Count radius
            float maxFloorDistance = 0.5f * capsule.Height + tolerance;
            float maxDistance = maxFloorDistance + tolerance;

            if (!Physics.SphereCast(sphereCastOrigin, radius, -upDirection, out RaycastHit hit, maxDistance, collisionMask, QueryTriggerInteraction.Ignore))
            {
                groundInfo = new GroundHitInfo();
                return false;
            }

            if (Vector3.Dot(hit.normal, upDirection) > minFloorUp)
            {
                if (hit.distance < maxFloorDistance)
                {
                    groundInfo = new GroundHitInfo(hit.point, hit.normal, hit.collider, true);
                    return true;
                }
                else
                {
                    groundInfo = new GroundHitInfo();
                    return false;
                }
            }

            groundInfo = new GroundHitInfo(hit.point, hit.normal, hit.collider, false);
            return true;
        }
    }
}