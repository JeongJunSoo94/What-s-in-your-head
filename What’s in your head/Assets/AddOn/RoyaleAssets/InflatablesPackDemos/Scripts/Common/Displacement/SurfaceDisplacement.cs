using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Common
{
    public struct SurfaceDisplacement
    {
        public Vector3 deltaPosition;
        public float deltaRotation;

        public Vector3 deltaVelocity;
        public Vector3 deltaAngularVelocity;

        public void MergeWith(SurfaceDisplacement other)
        {
            deltaPosition += other.deltaPosition;
            deltaRotation += other.deltaRotation;
            deltaVelocity += other.deltaVelocity;
            deltaAngularVelocity += other.deltaAngularVelocity;
        }

        public void Reset()
        {
            deltaPosition = Vector3.zero;
            deltaRotation = 0;

            deltaVelocity = Vector3.zero;
            deltaAngularVelocity = Vector3.zero;
        }
    }
}