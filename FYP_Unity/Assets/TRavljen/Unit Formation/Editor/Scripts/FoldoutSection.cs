using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace TRavljen.UnitFormation.Editor
{

    public class FoldoutSection
    {
        private static readonly Dictionary<string, bool> foldoutTitles = new Dictionary<string, bool>();

        public bool BeginSection(string title)
        {
            if (title != null)
            {
                foldoutTitles.TryGetValue(title, out bool foldout);
                foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, title);
                foldoutTitles[title] = foldout;
                return foldout;
            }

            return false;
        }

        public void EndSection()
        {
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

}