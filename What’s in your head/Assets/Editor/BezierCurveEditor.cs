using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace JJS
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BezierCurve))]
    public class BezierCurveEditor : Editor
    {
        private void OnSceneGUI()
        {
            //BezierCurve Generator = (BezierCurve)target;

            //Generator.p1 = Handles.PositionHandle(Generator.p1, Quaternion.identity);
            //Generator.p2 = Handles.PositionHandle(Generator.p2, Quaternion.identity);
            //Generator.p3 = Handles.PositionHandle(Generator.p3, Quaternion.identity);
            //Generator.p4 = Handles.PositionHandle(Generator.p4, Quaternion.identity);

            //Handles.DrawLine(Generator.p1, Generator.p2);
            //Handles.DrawLine(Generator.p3, Generator.p4);

            //int pivotCount = 50;
            //float halfpivotCount = pivotCount*0.5f;
            //for (float i = 0; i < pivotCount; i++)
            //{
            //    float valueBefore = i / pivotCount;
            //    Vector3 before = Generator.BezierCurveUpdate(Generator.p1, Generator.p2, Generator.p3, Generator.p4, valueBefore);
            //    float valueAfter = (i + 1) / pivotCount;
            //    Vector3 after = Generator.BezierCurveUpdate(Generator.p1, Generator.p2, Generator.p3, Generator.p4, valueAfter);

            //    Handles.DrawLine(before, after);
            //}
        }
    }
}
