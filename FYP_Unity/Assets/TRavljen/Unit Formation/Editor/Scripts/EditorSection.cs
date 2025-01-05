using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TRavljen.UnitFormation.Editor
{

    public static class HeaderLabel
    {
        private static GUIStyle boldStyle;

        public static GUIStyle GetStyle()
        {
            if (boldStyle == null)
            {
                var skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
                boldStyle = skin.GetStyle("Label");
                boldStyle.fontStyle = FontStyle.Bold;
                Color textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                boldStyle.normal.textColor = textColor;
                boldStyle.hover.textColor = textColor;
            }

            return boldStyle;
        }
    }

    public class EditorSection
    {

        public void BeginSection(string title)
        {
            if (title != null)
                GUILayout.Label(title, HeaderLabel.GetStyle());

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(12);

            EditorGUILayout.BeginVertical();
        }

        public void EndSection()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }

}