#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs {
    public static class Configs {
        public const string MenuItem = "Configs/";
        public const string EntityMenuItem = MenuItem + "Entities/";
        public const string ActionMenuItem = MenuItem + "Actions/";
        public const string MapMenuItem = MenuItem + "Map/";
        public const string TerrainMenuItem = MenuItem + "Terrain/";
        private const string DefaultPath = "Assets/_Game/Data/BurnMark/Configs/";

        public static T Create<T>() where T : ScriptableObject {
            var asset = ScriptableObject.CreateInstance<T>();

            var activePath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var path = (string.IsNullOrEmpty(activePath) 
                ? DefaultPath 
                : activePath + "/")
                       + $"{typeof(T).Name}.asset";
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }
    }
}
#endif