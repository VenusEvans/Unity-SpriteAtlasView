using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace Venus.SpriteAtlasView
{
    public static class SpriteAtlasViewData
    {
        private static Dictionary<SpriteAtlas, List<Sprite>> _allSpriteCacheDic = new Dictionary<SpriteAtlas, List<Sprite>>();
        private static Dictionary<Sprite, SpriteAtlas> _spriteWithAtlasCacheDic = new Dictionary<Sprite, SpriteAtlas>();
        
        public static void Initialize()
        {
            _allSpriteCacheDic.Clear();
            _spriteWithAtlasCacheDic.Clear();

            IEnumerable<SpriteAtlas> allAtlas = GetProjectAllAtlas();
            if (allAtlas == null) return;

            foreach (var atlas in allAtlas)
            {
                SaveSpriteAtlasData(atlas);
            }
        }
        
        public static void InitializeDataWithAtlas(SpriteAtlas atlas)
        {
            SaveSpriteAtlasData(atlas);
        }

        public static Dictionary<SpriteAtlas, List<Sprite>> GetData()
        {
            return _allSpriteCacheDic;
        }

        public static void Dispose()
        {
            _allSpriteCacheDic.Clear();
        }

        public static SpriteAtlas GetSpriteAtlasWithSprite(Sprite sprite)
        {
            if (sprite == null) return null;
            return _spriteWithAtlasCacheDic.TryGetValue(sprite, out SpriteAtlas spriteAtlas) ? spriteAtlas : null;
        }
        
        private static void SaveSpriteAtlasData(SpriteAtlas atlas)
        {
            if (atlas == null) return;
            List<Sprite> sprites = new List<Sprite>();
            SerializedProperty packedSpritesProperty = new SerializedObject(atlas).FindProperty("m_PackedSprites");
            for (int j = 0, size = packedSpritesProperty.arraySize; j < size; j++)
            {
                Object obj = packedSpritesProperty.GetArrayElementAtIndex(j).objectReferenceValue;
                if (obj is Sprite sprite)
                {
                    sprites.Add(sprite);
                }
            }
            _allSpriteCacheDic[atlas] = sprites;

            for (int i = 0, length = sprites.Count; i < length; i++)
            {
                Sprite sprite = sprites[i];
                if (sprite == null) continue;
                _spriteWithAtlasCacheDic[sprite] = atlas;
            }
        }
        
        private static IEnumerable<SpriteAtlas> GetProjectAllAtlas()
        {
            IEnumerable<SpriteAtlas> projectAllSpriteAtlas = AssetDatabase.FindAssets("t:SpriteAtlas")
                .Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<SpriteAtlas>);
            return projectAllSpriteAtlas;
        }
    }
}

