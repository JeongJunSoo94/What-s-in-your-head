using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS
{
    public class WarterGunData : ScriptableObject
    {
        [Header("BulletInfo")]
        public float speed = 1;

        [Header("WaterGunInfo")]
        public float shootMaxDistance;
        public float shootMinDistrace;
        public float shootCurDistance;
        public float curveHeight = 1f;
        public float curveWidth = 0.5f;
        public float shootSpeed = 0.5f;
        public int bulletCount = 0;
        public LayerMask layerMask;
    }
}
