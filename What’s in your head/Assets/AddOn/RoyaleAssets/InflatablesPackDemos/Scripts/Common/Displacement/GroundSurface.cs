using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Common
{
    public abstract class GroundSurface : MonoBehaviour
    {
        public abstract void UpdateDisplacement(Vector3 point, Vector3 up, ref SurfaceDisplacement otherDisplacement);
    }
}