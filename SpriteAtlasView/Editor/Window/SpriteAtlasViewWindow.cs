using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Venus.SpriteAtlasView
{
    public partial class SpriteAtlasViewWindow : EditorWindow
    {
        #region Member
        
        private GUIStyle _tipsStyle;
        private GUIStyle _selectObjPathStyle;
        private GUIStyle _atlasItemSelectedStyle;
        private GUIStyle _atlasItemNormalStyle;
        private GameObject _currentActiveGameObject;
        private StringBuilder _stringBuilder = new StringBuilder();
        private GUIContent _tipsContent;
        private Dictionary<SpriteAtlas, List<Sprite>> _allSpriteData;
        private Dictionary<Sprite, Rect> _spriteTexCoords = new Dictionary<Sprite, Rect>();

        private Vector2 _atlasPanelScrollPos;
        private Vector2 _spritePanelScrollPos;
        private SpriteAtlas _selectedSpriteAtlas;
        private Sprite _selectedSprite;

        private Image _image;
        private SpriteRenderer _spriteRenderer;
        
        private Texture2D _spriteDrawBackGround;
        private SerializedObject _componentSerializedObject;
        private List<Sprite> _showSprites = new List<Sprite>();
        
        private string _searchText;
        
        #endregion
        
        [MenuItem("Tools/Venus/SpriteAtlas View")]
        public static void ShowWindow()
        {
            SpriteAtlasViewWindow window = GetWindow<SpriteAtlasViewWindow>("SpriteAtlas View");
            window.minSize = new Vector2(1500, 400);
        }
        
        void OnEnable()
        {
            EditorApplication.update += UpdateWindow;
            InitializeGUIStyle();
            Init();
        }

        void OnDestroy()
        {
            EditorApplication.update -= UpdateWindow;
            SpriteAtlasViewData.Dispose();
        }

        void OnGUI()
        {
            bool next = DrawTitle();
            if (next == false) return;

            EditorGUILayout.BeginHorizontal();
            DrawAtlasPanel();
            DrawSpritePanel();
            EditorGUILayout.EndHorizontal();
        }

        private void Init()
        {
            SpriteAtlasViewData.Initialize();
            _allSpriteData = SpriteAtlasViewData.GetData();
            UpdateActiveGameObject(true);
        }
        
        private (Image, SpriteRenderer) GetComponent()
        {
            Image image = null;
            SpriteRenderer spriteRenderer = null;
            if (_currentActiveGameObject != null)
            {
                image = _currentActiveGameObject.GetComponent<Image>();
                spriteRenderer = _currentActiveGameObject.GetComponent<SpriteRenderer>();
            }
            
            return (image, spriteRenderer);
        }
        
        private void ShowTips(string content, float tweenTime = 0.5f)
        {
            if (_tipsContent == null) _tipsContent = new GUIContent();
            _tipsContent.text = content;
            ShowNotification(_tipsContent, tweenTime);
        }
        
        private void InitializeGUIStyle()
        {
            _tipsStyle = new GUIStyle()
            {
                fontSize = 40,
                alignment = TextAnchor.MiddleCenter,
                normal = new GUIStyleState()
                {
                    textColor = Color.red,
                }
            };
            _selectObjPathStyle = new GUIStyle()
            {
                fontSize = 18,
                alignment = TextAnchor.LowerCenter,
                normal = new GUIStyleState()
                {
                    textColor = Color.white
                },
                richText = true,
            };

            _spriteDrawBackGround = new Texture2D(1, 1);
            _spriteDrawBackGround.SetPixel(0,0, new Color(0, 0, 0, 0));
            _spriteDrawBackGround.Apply();
        }

        private void SelectAtlasProcess()
        {
            (_image, _spriteRenderer) = GetComponent();
            if (_image != null)
            {
                _selectedSprite = _image.sprite;
                _componentSerializedObject = new SerializedObject(_image);
            }
            else if (_spriteRenderer != null)
            {
                _selectedSprite = _spriteRenderer.sprite;
                _componentSerializedObject = new SerializedObject(_spriteRenderer);
            }
            else
            {
                _selectedSprite = null;
                _componentSerializedObject = null;
            }

            _selectedSpriteAtlas = SpriteAtlasViewData.GetSpriteAtlasWithSprite(_selectedSprite);
        }
        
        private Rect GetSpriteTexCoords(Sprite sprite)
        {
            if (_spriteTexCoords.TryGetValue(sprite, out Rect texCoords) == false)
            {
                Rect spriteRectInAtlas = sprite.textureRect;
                Texture2D atlasTexture = sprite.texture;
                texCoords = new Rect(spriteRectInAtlas.x / atlasTexture.width, spriteRectInAtlas.y / atlasTexture.height, spriteRectInAtlas.width / atlasTexture.width, spriteRectInAtlas.height / atlasTexture.height);
                _spriteTexCoords[sprite] = texCoords;
            }

            return texCoords;
        }
        
        private void SelectBorder(Rect rect)
        {
            Texture2D tex = EditorGUIUtility.whiteTexture;
            GUI.color = Color.green;
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), tex);
            GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1f, rect.height), tex);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
            GUI.color = Color.white;
        }
        
        private void UpdateWindow()
        {
            UpdateActiveGameObject();
        }

        private void UpdateActiveGameObject(bool force = false)
        {
            GameObject activeGameObject = Selection.activeGameObject;
            bool refresh = force ? true : activeGameObject != _currentActiveGameObject;
            if (refresh)
            {
                _currentActiveGameObject = activeGameObject;
                SelectAtlasProcess();
                Repaint();
            }
        }
        
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        
        private void UpdateShowSprites()
        {
            _showSprites.Clear();
            
            _allSpriteData.TryGetValue(_selectedSpriteAtlas, out List<Sprite> sprites);
            
            if (sprites == null || sprites.Count <= 0) return;
            
            if (string.IsNullOrEmpty(_searchText))
            {
                _showSprites.AddRange(sprites);
            }
            else
            {
                for (int i = 0, count = sprites.Count; i < count; i++)
                {
                    Sprite sprite = sprites[i];
                    if (sprite == null) continue;
                    string spriteName = sprite.name;
                    if (spriteName.ToLower().Contains(_searchText.ToLower()))
                    {
                        _showSprites.Add(sprite);
                    }
                }
            }
        }
    }
}