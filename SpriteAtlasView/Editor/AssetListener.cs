using UnityEditor;
using UnityEngine.U2D;

namespace Venus.SpriteAtlasView
{
    public class AssetListener : AssetPostprocessor
    {
#if UNITY_EDITOR
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var importedPath in importedAssets)
            {
                SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(importedPath);
                if (atlas != null) SpriteAtlasViewData.InitializeDataWithAtlas(atlas);
            }
        }
#endif
    }
}