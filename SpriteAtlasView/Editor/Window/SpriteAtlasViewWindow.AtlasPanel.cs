using UnityEditor;
using UnityEngine;

namespace Venus.SpriteAtlasView
{
    public partial class SpriteAtlasViewWindow
    {
        private void DrawAtlasPanel()
        {
            if (_atlasItemSelectedStyle == null)
            {
                _atlasItemSelectedStyle = new GUIStyle("box")
                {
                    normal =
                    {
                        background = MakeTex(2, 2, new Color(0.2f, 0.8f, 0.2f, 0.3f))
                    },
                    padding = new RectOffset(5, 5, 5, 5)
                };
            }

            if (_atlasItemNormalStyle == null)
            {
                _atlasItemNormalStyle = new GUIStyle("box")
                {
                    padding = new RectOffset(5, 5, 5, 5)
                };
            }
            
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
            _atlasPanelScrollPos = EditorGUILayout.BeginScrollView(_atlasPanelScrollPos, "box");
            
            Event evt = Event.current;
            foreach (var atlas in _allSpriteData.Keys)
            {
                bool isSelected = atlas == _selectedSpriteAtlas;
        
                Rect backgroundRect = EditorGUILayout.BeginVertical(isSelected ? _atlasItemSelectedStyle : _atlasItemNormalStyle);
                
                EditorGUILayout.BeginHorizontal();
                bool ping = GUILayout.Button(EditorGUIUtility.IconContent("SpriteAtlas Icon"), GUILayout.Width(25), GUILayout.Height(25));
                if (ping) EditorGUIUtility.PingObject(atlas);
                EditorGUILayout.LabelField(atlas.name, GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
                
                if (evt.type == EventType.MouseDown && evt.button == 0 && backgroundRect.Contains(evt.mousePosition))
                {
                    _selectedSpriteAtlas = atlas;
                    evt.Use();
                }
                
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}