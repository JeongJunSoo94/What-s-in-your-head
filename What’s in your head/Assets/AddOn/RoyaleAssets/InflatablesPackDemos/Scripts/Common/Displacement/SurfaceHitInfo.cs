using UnityEngine;

namespace RoyaleAssets.InflatablesGameDemo.Common
{
    public struct SurfaceHitInfo
    {
        public Vector3 point;
        public Vector3 normal;
        public ObstacleSurface other;

        public bool HasValue => other != null;

        public void Init(Vector3 point, Vector3 normal, ObstacleSurface obstacle)
        {
            this.point = point;
            this.normal = normal;
            this.other = obstacle;
        }

        public void Reset()
        {
            other = null;
        }
    }
}
