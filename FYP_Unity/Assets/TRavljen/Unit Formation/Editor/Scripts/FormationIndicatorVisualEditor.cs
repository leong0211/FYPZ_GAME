using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRavljen.UnitFormation.Editor
{
    using UnityEditor;
    using Placement;

    [CustomEditor(typeof(FormationIndicatorVisual))]
    public class FormationIndicatorVisualEditor : Editor
    {
        SerializedProperty unitIndicatorPrefab;
        SerializedProperty hideWithDelay;
        SerializedProperty hideDelay;

        private void OnEnable()
        {
            unitIndicatorPrefab = serializedObject.FindProperty("unitIndicatorPrefab");
            hideWithDelay = serializedObject.FindProperty("hideWithDelay");
            hideDelay = serializedObject.FindProperty("hideDelay");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(unitIndicatorPrefab);

            EditorGUILayout.PropertyField(hideWithDelay);

            if (hideWithDelay.boolValue)
            {
                EditorGUILayout.PropertyField(hideDelay);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

namespace TRavljen.UnitFormation.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(UnitFormation))]
    public class UnitFormationEditor : Editor
    {

        SerializedProperty placeOnGround;
        SerializedProperty maxGroundDistance;
        SerializedProperty noiseEnabled;
        SerializedProperty units;

        EditorSection customSection;

        private void OnEnable()
        {
            placeOnGround = serializedObject.FindProperty("placeOnGround");
            maxGroundDistance = serializedObject.FindProperty("maxGroundDistance");
            noiseEnabled = serializedObject.FindProperty("noiseEnabled");
            units = serializedObject.FindProperty("units");

            customSection = new EditorSection();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(placeOnGround);
            if (placeOnGround.boolValue)
            {
                customSection.BeginSection(null);
                EditorGUILayout.PropertyField(maxGroundDistance);
                customSection.EndSection();
            }

            EditorGUILayout.PropertyField(noiseEnabled);

            EditorGUILayout.PropertyField(units);

            serializedObject.ApplyModifiedProperties();
        }
    }
}