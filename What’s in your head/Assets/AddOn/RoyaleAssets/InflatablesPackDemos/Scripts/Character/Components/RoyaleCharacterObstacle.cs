using RoyaleAssets.InflatablesGameDemo.Common;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Character
{
    [RequireComponent(typeof(RoyaleCharacterInput))]
    [RequireComponent(typeof(RoyaleCharacterCapsule))]
    public class RoyaleCharacterObstacle : ObstacleSurface
    {
        [SerializeField] float waight = 5;

        private RoyaleCharacterInput input;
        private RoyaleCharacterCapsule capsule;

        private void Start()
        {
            input = GetComponent<RoyaleCharacterInput>();
            capsule = GetComponent<RoyaleCharacterCapsule>();
        }

        public override void UpdateDisplacement(ref SurfaceHitInfo obstacleHit, ref SurfaceDisplacement thisDisplacement, ref SurfaceDisplacement otherDisplacement)
        {
            var move = input.GetInputMove();
            var forward = Vector3.ProjectOnPlane(obstacleHit.normal, capsule.UpDirection).normalized;

            otherDisplacement.deltaPosition = -waight * move.sqrMagnitude * Time.deltaTime * forward;
        }
    }
}