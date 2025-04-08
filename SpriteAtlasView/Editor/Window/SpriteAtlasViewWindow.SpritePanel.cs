using UnityEditor;
using UnityEngine;

namespace Venus.SpriteAtlasView
{
    public partial class SpriteAtlasViewWindow
    {
        private void DrawSpritePanel()
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginHorizontal();
            bool change = false;
            var beforeSearchText = _searchText;
            var curSearchText = EditorGUILayout.TextField("", beforeSearchText, "SearchTextField");

            if (curSearchText != beforeSearchText)
            {
                _searchText = curSearchText;
                change = true;
            }

            bool clearSearch = GUILayout.Button("", "SearchCancelButton", GUILayout.Width(20));
            if (clearSearch)
            {
                change = true;
                _searchText = "";
                GUIUtility.keyboardControl = 0;
            }

            if (change)
            {
                UpdateShowSprites();
            }
            EditorGUILayout.EndHorizontal();
            
            Rect windowRect = position;
            float padding = 10;
            float nameHeight = padding * 2;
            Event currentEvent = Event.current;
            _spritePanelScrollPos = EditorGUILayout.BeginScrollView(_spritePanelScrollPos, false, true, GUILayout.ExpandWidth(true));

            float texSize = 85;
            int horizontalCount = Mathf.FloorToInt(windowRect.width / (texSize + padding * 0.8f));
            Rect texRect = new Rect(padding, 0, texSize, texSize);
            int curCount = 0;

            for (int i = 0, length = _showSprites.Count; i < length; i++)
            {
                Sprite sprite = _showSprites[i];
                if (sprite == null) continue;

                Texture2D atlasTexture = sprite.texture;
                Rect texCoords = GetSpriteTexCoords(sprite);

                EditorGUI.DrawTextureTransparent(texRect, _spriteDrawBackGround);
                GUI.DrawTextureWithTexCoords(texRect, atlasTexture, texCoords);

                Rect nameRect = new Rect(texRect.x, texRect.y + texSize + padding / 3, texSize, nameHeight);
                GUI.TextField(nameRect, sprite.name);

                SerializedProperty spriteProperty = _componentSerializedObject.FindProperty("m_Sprite");

                Undo.RecordObject(spriteProperty.objectReferenceValue, "Venus.SpriteAtlasView.SetSprite");
                _componentSerializedObject.Update();

                GUI.color = Color.clear;
                bool clickButton = GUI.Button(texRect, "");
                if (clickButton)
                {
                    if (currentEvent.button == 0)
                    {
                        spriteProperty.objectReferenceValue = sprite;
                        _componentSerializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(spriteProperty.objectReferenceValue);
                        currentEvent.Use();
                    }
                    else if (currentEvent.button == 1)
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Open in Sprite Editor"), false, () =>
                        {
                            Object lastSelectedObject = Selection.activeObject;
                            Selection.activeObject = sprite;
                            EditorApplication.ExecuteMenuItem("Window/2D/Sprite Editor");
                            Selection.activeObject = lastSelectedObject;
                        });
                        menu.ShowAsContext();
                        currentEvent.Use();
                    }
                }

                GUI.color = Color.white;

                if (spriteProperty.objectReferenceValue == sprite)
                {
                    SelectBorder(texRect);
                }

                texRect.x += texSize + padding;
                curCount++;

                if (curCount % horizontalCount == 0)
                {
                    EditorGUILayout.Space(texSize + padding + nameHeight);
                    texRect.x = padding;
                    texRect.y += texSize + padding + nameHeight;
                }
            }
            
            EditorGUILayout.Space(texSize + padding + nameHeight);
            EditorGUILayout.EndScrollView();
            
            GUILayout.EndVertical();
        }
    }
}