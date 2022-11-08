using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VolumetricLightMesh)), CanEditMultipleObjects]
public class SpotlightMeshEditor : Editor
{
    //private float minVal;
    //private float maxVal;
    //private float maxRange = 10;

    //public override void OnInspectorGUI()
    //{
    //    VolumetricLightMesh vlm = (VolumetricLightMesh)target;

    //    GUIStyle titleStyle = new GUIStyle();
    //    titleStyle.fontSize = 18;
    //    titleStyle.normal.textColor = Color.white;
    //    titleStyle.fontStyle = FontStyle.Bold;
    //    titleStyle.alignment = TextAnchor.MiddleCenter;

    //    GUILayout.Space(10);

    //    GUILayout.Label("Spotlight Mesh", titleStyle);

    //    GUILayout.Space(10);

    //    SerializedProperty lightSource = serializedObject.FindProperty("lightSource");
    //    SerializedProperty quality = serializedObject.FindProperty("quality");
    //    SerializedProperty startOffset = serializedObject.FindProperty("startOffset");
    //    SerializedProperty range = serializedObject.FindProperty("range");
    //    SerializedProperty spotAngle = serializedObject.FindProperty("spotAngle");
    //    SerializedProperty useLightVariables = serializedObject.FindProperty("useLightVariables");

    //    minVal = startOffset.floatValue;
    //    maxVal = range.floatValue;
    //    if (vlm.lightSource != null) maxRange = vlm.lightSource.range;
    //    else maxRange = maxVal + 10;

    //    EditorGUILayout.PropertyField(lightSource);
    //    quality.intValue = EditorGUILayout.IntSlider("Quality", quality.intValue, 3, 100);

    //    GUILayout.Space(10);

    //    EditorGUI.BeginDisabledGroup(useLightVariables.boolValue);
    //    spotAngle.floatValue = EditorGUILayout.Slider("Angle", spotAngle.floatValue, 1, 179);
    //    EditorGUI.EndDisabledGroup();

    //    float startOld = startOffset.floatValue;
    //    float rangeOld = range.floatValue;
    //    if (useLightVariables.boolValue) startOffset.floatValue = EditorGUILayout.Slider("Offset", startOffset.floatValue, 0, maxRange);
    //    else
    //    {
    //        GUILayout.BeginHorizontal();
    //        EditorGUILayout.PropertyField(startOffset, GUIContent.none, GUILayout.MaxWidth(100));
    //        EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, 0, maxRange);
    //        EditorGUILayout.PropertyField(range, GUIContent.none, GUILayout.MaxWidth(100));
    //        GUILayout.EndHorizontal();
    //    }

    //    GUILayout.Space(10);

    //    EditorGUI.BeginDisabledGroup(vlm.lightSource == null);
    //    EditorGUILayout.PropertyField(useLightVariables);
    //    EditorGUI.EndDisabledGroup();

    //    if (startOld == startOffset.floatValue && rangeOld == range.floatValue)
    //    {
    //        startOffset.floatValue = minVal;
    //        range.floatValue = maxVal;
    //    }

    //    GUILayout.Space(10);

    //    if (GUILayout.Button("Generate Mesh")) vlm.GenerateMesh();

    //    if (GUILayout.Button("Export Mesh"))
    //    {
    //        Type projectWindowUtilType = typeof(ProjectWindowUtil);
    //        MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
    //        object obj = getActiveFolderPath.Invoke(null, new object[0]);
    //        string path = obj.ToString();
    //        AssetDatabase.CreateAsset(vlm.GetComponent<MeshFilter>().sharedMesh, $"{path}/VolumetricLightMesh.asset");
    //        Debug.Log($"Mesh saved at: {path}");
    //    }

    //    serializedObject.ApplyModifiedProperties();
    //}

    //[MenuItem("GameObject/Light/Volumetric Spotlight", false, 3)]
    //static void CreateVolumetricLight()
    //{
    //    GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Volumetric Mesh/Prefabs/Volumetric Light.prefab", typeof(GameObject));
    //    GameObject instantiatedObject = Instantiate(prefab);
    //    instantiatedObject.name = "Volumetric Spotlight";
    //    Undo.RegisterCreatedObjectUndo(instantiatedObject, "Create Volumetric Spotlight");
    //}
}