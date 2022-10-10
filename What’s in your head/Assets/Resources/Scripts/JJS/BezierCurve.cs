using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJS
{
    public class BezierCurve : MonoBehaviour
    {
        public GameObject targetObj;
        [Range(0, 1)]
        public float range;

        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
        public Vector3 p4;

        public bool on;
        private void Start()
        {
            targetObj =this.gameObject;
        }

        void Update()
        {
            if(on)
                targetObj.transform.position = BezierCurveUpdate(p1, p2, p3, p4, range);
        }

        public Vector3 BezierCurveUpdate(Vector3 p_1, Vector3 p_2, Vector3 p_3, Vector3 p_4, float value)
        {
            Vector3 A = Vector3.Lerp(p_1, p_2, value);
            Vector3 B = Vector3.Lerp(p_2, p_3, value);
            Vector3 C = Vector3.Lerp(p_3, p_4, value);

            Vector3 D = Vector3.Lerp(A, B, value);
            Vector3 E = Vector3.Lerp(B, C, value);

            Vector3 F = Vector3.Lerp(D, E, value);
            return F;
        }
    }
}
