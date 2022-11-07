using RoyaleAssets.InflatablesGameDemo.Common;
using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Obstacles
{
    public class TrampolineSurface : GroundSurface
    {
        [SerializeField] Transform upTransform;
        [SerializeField] float power = 1;

        private void Start()
        {
            if (!upTransform)
            {
                upTransform = transform;
            }
        }

        public override void UpdateDisplacement(Vector3 point, Vector3 up, ref SurfaceDisplacement otherDisplacement)
        {
            otherDisplacement.deltaVelocity += power * upTransform.up;
        }
    }
}