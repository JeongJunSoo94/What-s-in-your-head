using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightFlickering))]
public class LightFlickerInspector : Editor
{
    SerializedProperty onStart,
    lightSource,
    fadeEffect,
    fadeSpeed,
    randomizeFlickerings,
    minRandomizeTime,
    maxRandomizeTime,
    flickerings,
    loop,
    lightings,
    buzzSound,
    playBuzzSound,
    changeMaterial,
    bulbObject,
    newMaterial;

    void OnEnable()
    {
        onStart = serializedObject.FindProperty("onStart");
        lightSource = serializedObject.FindProperty("lightSource");
        fadeEffect = serializedObject.FindProperty("fadeEffect");
        fadeSpeed = serializedObject.FindProperty("fadeSpeed");

        randomizeFlickerings = serializedObject.FindProperty("randomizeFlickerings");
        minRandomizeTime = serializedObject.FindProperty("minRandomizeTime");
        maxRandomizeTime = serializedObject.FindProperty("maxRandomizeTime");
        
        flickerings = serializedObject.FindProperty("flickerings");
        loop = serializedObject.FindProperty("loop");
        
        lightings = serializedObject.FindProperty("lightings");
        
        buzzSound = serializedObject.FindProperty("buzzSound");
        playBuzzSound = serializedObject.FindProperty("playBuzzSound");
        
        changeMaterial = serializedObject.FindProperty("changeMaterial");
        bulbObject = serializedObject.FindProperty("bulbObject");
        newMaterial = serializedObject.FindProperty("newMaterial");
    }

    public override void OnInspectorGUI()
    {
        var button = GUILayout.Button("Click for more tools");
        if (button) Application.OpenURL("https://assetstore.unity.com/publishers/39163");
        EditorGUILayout.Space(5);

        LightFlickering script = (LightFlickering) target;

        EditorGUILayout.LabelField("Light Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(onStart, new GUIContent("On Start", "Check whether you want the flickering to start automatically on game start or not"));
        EditorGUILayout.PropertyField(lightSource, new GUIContent("Light Source", "The light source that should flicker"));
        EditorGUILayout.PropertyField(fadeEffect, new GUIContent("Fade Effect", "Make the light fade in and out. Best used with the same light color ~ Lightings property set to 0."));
        EditorGUI.BeginDisabledGroup(script.fadeEffect == false);
            EditorGUILayout.PropertyField(fadeSpeed, new GUIContent("Fade Speed", "The speed of the fade in/out of the light"));
        EditorGUI.EndDisabledGroup ();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Randomizing Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(randomizeFlickerings, new GUIContent("Randomize Flickerings", "Make auto-random flickerings with time generated between two (min, max) floats"));
        EditorGUI.BeginDisabledGroup(script.randomizeFlickerings == false);
            EditorGUILayout.PropertyField(minRandomizeTime, new GUIContent("Min Randomize Time", "Minimum float for randomizing"));
            EditorGUILayout.PropertyField(maxRandomizeTime, new GUIContent("Max Randomize Time", "Maximum float for randomizing"));
        EditorGUI.EndDisabledGroup ();

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(script.randomizeFlickerings == true);
            EditorGUILayout.LabelField("Manual Flickering Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(flickerings, new GUIContent("Flickerings", "Manual flickerings set the amount of flickers and the time to pass before each flicker"));
            EditorGUILayout.PropertyField(loop, new GUIContent("Loop", "Loop the manual flickerings or finish on the 1st pass"));
        EditorGUI.EndDisabledGroup ();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Lightings Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(lightings, new GUIContent("Lightings", "Light color and bulb material of each flicker. Will be read in both manual and random mode. If you want the light color to stay the same as the original light source set this to 0"));

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Audio Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(playBuzzSound, new GUIContent("Play Buzz Sound", "Should an audio source (buzz sound) be played on light on"));
        EditorGUI.BeginDisabledGroup(script.playBuzzSound == false);
            EditorGUILayout.PropertyField(buzzSound, new GUIContent("Buzz Sound", "Audio source of buzz sound to play when light is on"));
        EditorGUI.EndDisabledGroup ();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Material Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(changeMaterial, new GUIContent("Change Material", "Should the material be changed on light off"));
        EditorGUI.BeginDisabledGroup(script.changeMaterial == false);
            EditorGUILayout.PropertyField(bulbObject, new GUIContent("Bulb Object", "The object with mesh renderer that you want to change it's material"));
            EditorGUILayout.PropertyField(newMaterial, new GUIContent("New Material", "The material that will be applied on light off"));
        EditorGUI.EndDisabledGroup ();
        
        serializedObject.ApplyModifiedProperties();
    }
}
