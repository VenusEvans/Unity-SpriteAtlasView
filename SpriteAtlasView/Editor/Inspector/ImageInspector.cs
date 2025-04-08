using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Venus.SpriteAtlasView
{
    [CustomEditor(typeof(Image))]
    public class ImageInspector : ImageEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
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


