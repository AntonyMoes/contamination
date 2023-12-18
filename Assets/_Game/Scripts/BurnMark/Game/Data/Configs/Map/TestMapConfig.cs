using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Map {
    public class TestMapConfig : MapConfig {
        [SerializeField] private TerrainConfig _terrain;
        [SerializeField] private Vector2Int _mapSize;

        [SerializeField] private Vector2Int[] _startingPoints;
        public override Vector2Int[] StartingPoints => _startingPoints;

        public override TerrainConfig[,] Terrain {
            get {
                var result = new TerrainConfig[_mapSize.x, _mapSize.y];
                foreach (var position in _mapSize.EnumeratePositions()) {
                    result[position.x, position.y] = _terrain;
                }

                return result;
            }
        }

#if UNITY_EDITOR
        [MenuItem(Configs.MapMenuItem + nameof(TestMapConfig), false)]
        public static void Create() => Configs.Create<TestMapConfig>();
#endif
    }
}