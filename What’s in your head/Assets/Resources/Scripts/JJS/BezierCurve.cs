using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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

        private void Start()
        {
            targetObj =this.gameObject;
        }

        void Update()
        {

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

    [CanEditMultipleObjects]
    [CustomEditor(typeof(BezierCurve))]
    public class BezierCurveEditor : Editor
    {
        private void OnSceneGUI()
        {
            BezierCurve Generator = (BezierCurve)target;

            Generator.p1 = Handles.PositionHandle(Generator.p1, Quaternion.identity);
            Generator.p2 = Handles.PositionHandle(Generator.p2, Quaternion.identity);
            Generator.p3 = Handles.PositionHandle(Generator.p3, Quaternion.identity);
            Generator.p4 = Handles.PositionHandle(Generator.p4, Quaternion.identity);

            Handles.DrawLine(Generator.p1, Generator.p2);
            Handles.DrawLine(Generator.p3, Generator.p4);

            int pivotCount = 50;
            for (float i = 0; i < pivotCount; i++)
            {
                float valueBefore = i / pivotCount;
                Vector3 before = Generator.BezierCurveUpdate(Generator.p1, Generator.p2, Generator.p3, Generator.p4, valueBefore);
                float valueAfter = (i + 1) / pivotCount;
                Vector3 after = Generator.BezierCurveUpdate(Generator.p1, Generator.p2, Generator.p3, Generator.p4, valueAfter);

                Handles.DrawLine(before, after);
            }
        }
    }

}
