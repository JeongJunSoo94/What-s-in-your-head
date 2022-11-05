using RoyaleAssets.InflatablesGameDemo.Common;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Character
{
    [RequireComponent(typeof(RoyaleObstacleDetector))]
    [RequireComponent(typeof(RoyaleCharacterMotor))]
    [RequireComponent(typeof(RoyaleCharacterMover))]
    [RequireComponent(typeof(RoyaleCharacterCapsule))]
    public class RoyaleCharacterDisplacement : MonoBehaviour
    {
        [SerializeField] int maxInteractionCount = 5;

        private RoyaleCharacterMotor motor;
        private RoyaleCharacterMover mover;
        private RoyaleObstacleDetector obstacleDetector;
        private RoyaleCharacterCapsule capsule;

        private SurfaceHitInfo[] surfaceHits;

        private SurfaceDisplacement thisDisplacement;
        private SurfaceDisplacement mergedDisplacement;
        private SurfaceDisplacement otherDisplacement;

        public void Start()
        {
            motor = GetComponent<RoyaleCharacterMotor>();
            mover = GetComponent<RoyaleCharacterMover>();
            obstacleDetector = GetComponent<RoyaleObstacleDetector>();
            capsule = GetComponent<RoyaleCharacterCapsule>();

            surfaceHits = new SurfaceHitInfo[maxInteractionCount];
            for (int i = 0; i < surfaceHits.Length; i++)
            {
                surfaceHits[i] = new SurfaceHitInfo();
            }

            otherDisplacement = new SurfaceDisplacement();
            thisDisplacement = new SurfaceDisplacement();
            mergedDisplacement = new SurfaceDisplacement();
        }

        public void UpdateDisplacement(bool isGraunded, bool groundDetected, GroundHitInfo groundInfo, Vector3 velocity)
        {
            mergedDisplacement.Reset();
            thisDisplacement.Reset();

            // Ground displacement
            GroundSurface[] groundSurfaces = null;
            if (isGraunded)
            {
                if (groundDetected && TryGetGroundSurface(groundInfo.collider, out groundSurfaces))
                {
                    for (int i = 0; i < groundSurfaces.Length; i++)
                    {
                        thisDisplacement.Reset();
                        groundSurfaces[i].UpdateDisplacement(groundInfo.point, capsule.UpDirection, ref thisDisplacement);
                        mergedDisplacement.MergeWith(thisDisplacement);
                    }
                }
            }

            // Obstacles displacement
            obstacleDetector.DetectObstacles(surfaceHits);
            for (int i = 0; i < surfaceHits.Length; i++)
            {
                var obstacleHit = surfaceHits[i];
                if (!obstacleHit.HasValue)
                {
                    break;
                }

                otherDisplacement.Reset();
                otherDisplacement.deltaVelocity = velocity;

                thisDisplacement.Reset();
                obstacleHit.other.UpdateDisplacement(ref obstacleHit, ref otherDisplacement, ref thisDisplacement);
                mergedDisplacement.MergeWith(thisDisplacement);
            }

            ApplySurfaceDisplacement(mergedDisplacement);
        }

        private void ApplySurfaceDisplacement(SurfaceDisplacement displacement)
        {
            motor.currentSpeed += displacement.deltaVelocity;
            mover.Move(displacement.deltaPosition);

            transform.Rotate(0f, displacement.deltaRotation, 0f, Space.Self);
        }

        private bool TryGetGroundSurface(Collider groundCollider, out GroundSurface[] surfaces)
        {
            surfaces = groundCollider.GetComponents<GroundSurface>();
            return surfaces.Length > 0;
        }
    }
}