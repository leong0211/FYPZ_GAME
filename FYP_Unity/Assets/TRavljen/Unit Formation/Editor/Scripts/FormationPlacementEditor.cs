using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRavljen.UnitFormation.Editor
{
    using UnityEditor;
    using TRavljen.UnitFormation.Placement;

    [CustomEditor(typeof(FormationPlacement))]
    public class FormationPlacementEditor : Editor
    {
        SerializedProperty groundLayerMask;
        SerializedProperty raycastMaxDistance;
        SerializedProperty customInput;
        SerializedProperty unitFormation;
        SerializedProperty alwaysCalculatePositions;
        SerializedProperty calculatePositionsIntervalThreshold;
        SerializedProperty placeVisualsOnGround;
        SerializedProperty placementVisuals;

        EditorSection customSection;
        FoldoutSection foldoutSection;

        private void OnEnable()
        {
            groundLayerMask = serializedObject.FindProperty("groundLayerMask");
            raycastMaxDistance = serializedObject.FindProperty("raycastMaxDistance");
            customInput = serializedObject.FindProperty("customInput");
            unitFormation = serializedObject.FindProperty("unitFormation");
            alwaysCalculatePositions = serializedObject.FindProperty("alwaysCalculatePositions");
            calculatePositionsIntervalThreshold = serializedObject.FindProperty("calculatePositionsIntervalThreshold");
            placeVisualsOnGround = serializedObject.FindProperty("placeVisualsOnGround");
            placementVisuals = serializedObject.FindProperty("placementVisuals");
            customSection = new EditorSection();
            foldoutSection = new FoldoutSection();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GeneralSectionGUI();
            VisualsSectionGUI();

            customSection.BeginSection("Focused Unit Formation Group");
            EditorGUILayout.PropertyField(unitFormation);
            customSection.EndSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void VisualsSectionGUI()
        {
            if (foldoutSection.BeginSection("Visuals"))
            {
                customSection.BeginSection(null);
                EditorGUILayout.PropertyField(alwaysCalculatePositions);

                if (alwaysCalculatePositions.boolValue)
                {
                    customSection.BeginSection(null);
                    EditorGUILayout.PropertyField(calculatePositionsIntervalThreshold);
                    EditorGUILayout.PropertyField(placeVisualsOnGround);
                    customSection.EndSection();
                }

                foldoutSection.EndSection();

                EditorGUILayout.PropertyField(placementVisuals);
                customSection.EndSection();
            }
            else
            {
                foldoutSection.EndSection();
            }
        }

        private void GeneralSectionGUI()
        {
            if (foldoutSection.BeginSection("General"))
            {
                customSection.BeginSection(null);
                EditorGUILayout.PropertyField(groundLayerMask);
                EditorGUILayout.PropertyField(raycastMaxDistance);
                customSection.EndSection();

                customSection.BeginSection("Input");

                InputSetupGUI();

                customSection.EndSection();
            }

            foldoutSection.EndSection();
        }

        private void InputSetupGUI()
        {
            var placement = target as FormationPlacement;
            if (IsCustomInputNotSet() && placement.GetComponent<AInputControl>() == null)
            {
                EditorGUILayout.Space();
                if (customInput.objectReferenceValue != null)
                {
                    EditorGUILayout.HelpBox("There is no component which implements 'IInputControl' or subclasses 'AInputControl' on target reference", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox("You can set reference to the game object that contains a component which implements IInputControl." +
                        "\nOr add default input based on your supported input system.", MessageType.Info);
                }
                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(customInput);

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.Label("OR Add provided input component");

                if (GUILayout.Button("Add"))
                {
                    placement.AddDefaultInput();
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();

                EditorGUILayout.HelpBox("Your INPUT is set up properly.\nYou may remove the current one.", MessageType.None);
                EditorGUILayout.Space();

                if (GUILayout.Button("Remove input"))
                {
                    if (customInput.objectReferenceValue != null)
                    {
                        customInput.objectReferenceValue = null;
                    }

                    if (placement.TryGetComponent(out AInputControl control))
                    {
                        DestroyImmediate(control);
                    }
                }

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
        }

        private bool IsCustomInputNotSet()
        {
            // First check null reference
            if (customInput.objectReferenceValue == null)
                return true;

            // Then if it contains valid interface
            return (customInput.objectReferenceValue as GameObject).GetComponent<IInputControl>() == null;
        }
    }
}