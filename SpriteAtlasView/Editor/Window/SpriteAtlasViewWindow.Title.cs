using UnityEditor;
using UnityEngine;

namespace Venus.SpriteAtlasView
{
    public partial class SpriteAtlasViewWindow
    {
        private bool DrawTitle()
        {
            if (_spriteRenderer == null && _image == null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("No Component Selected!(Image, SpriteRenderer)", _tipsStyle, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndHorizontal();
                return false;
            }
            
            _stringBuilder.Clear();
            Component component = null;
            if (_spriteRenderer != null)
            {
                component = _spriteRenderer;
            }
            else if (_image != null)
            {
                component = _image;
            }
            
            _stringBuilder.Insert(0, $"<color=cyan>{component.name}");
            Transform parent = component.transform.parent;
            while (true)
            {
                if (parent == null) break;
                _stringBuilder.Insert(0, parent.name + "/");
                parent = parent.parent;
            }
               
            _stringBuilder.Insert(0, "Current Select Component: "); 
            _stringBuilder.Append($".{component.GetType().Name}</color>");
                
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(_stringBuilder.ToString(), _selectObjPathStyle, GUILayout.Height(30));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bool refresh = GUILayout.Button("Refresh All", "LargeButton", GUILayout.Width(200));
            if (refresh)
            {
                Init();
                ShowTips("Refresh Success!");
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
            EditorGUILayout.LabelField("Atlas List", _selectObjPathStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("Sprite List", _selectObjPathStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.EndHorizontal();

            return true;
        }
    }
}