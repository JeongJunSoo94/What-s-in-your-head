using RoyaleAssets.InflatablesGameDemo.Common;
using RoyaleAssets.InflatablesGameDemo.Obstacles;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Character
{
    [RequireComponent(typeof(RoyaleCharacterCapsule))]
    public class RoyaleObstacleDetector : MonoBehaviour
    {
        [SerializeField] float threshold = 0.01f;

        private LayerMask collisionMask;

        private RoyaleCharacterCapsule capsule;
        private Collider thisCollider;


        private void Start()
        {
            collisionMask = GameLayers.CharacterMask | GameLayers.DynamicObstaclesMask;

            capsule = GetComponent<RoyaleCharacterCapsule>();
            thisCollider = GetComponent<Collider>();
        }

        public void DetectObstacles(SurfaceHitInfo[] surfaceInfos)
        {
            Vector3 sphereCastOrigin = capsule.Center;
            var halfHeight = (0.5f * capsule.Height - threshold) * capsule.UpDirection;
            Vector3 point0 = sphereCastOrigin - halfHeight;
            Vector3 point1 = sphereCastOrigin + halfHeight;
            float radius = capsule.Radius + threshold;

            thisCollider.enabled = false;
            var colladers = Physics.OverlapCapsule(point0, point1, radius, collisionMask, QueryTriggerInteraction.Ignore);
            thisCollider.enabled = true;

            int obstacleIndex = 0;
            for (int i = 0; obstacleIndex < surfaceInfos.Length && i < colladers.Length; i++)
            {
                var collider = colladers[i];
                var obstacle = collider.GetComponent<ObstacleSurface>();
                if (obstacle != null)
                {
                    var collisionPoint = collider.ClosestPoint(sphereCastOrigin);
                    var normal = (collisionPoint - sphereCastOrigin).normalized;
                    surfaceInfos[obstacleIndex++].Init(collisionPoint, normal, obstacle);
                }
            }

            for (int i = obstacleIndex; i < surfaceInfos.Length; i++)
            {
                surfaceInfos[i].Reset();
            }
        }

        private void OnValidate()
        {
            threshold = Mathf.Max(0f, threshold);
        }
    }
}