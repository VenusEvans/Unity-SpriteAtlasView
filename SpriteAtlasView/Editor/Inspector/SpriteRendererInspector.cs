using System;
using UnityEditor;
using UnityEngine;

namespace Venus.SpriteAtlasView
{
    [CustomEditor(typeof(SpriteRenderer))]
    public class SpriteRendererInspector : Editor
    {
        private Editor _originalEditor;
        
        public override void OnInspectorGUI()
        {
            if (_originalEditor == null)
            {
                _originalEditor = CreateEditor(target, Type.GetType("UnityEditor.SpriteRendererEditor, UnityEditor"));
            }
            _originalEditor.OnInspectorGUI();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Sprite View Window"))
            {
                SpriteAtlasViewWindow.ShowWindow();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}


