using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Map {
    [CreateAssetMenu(menuName = Configs.MapMenuItem + nameof(TestMapConfig), fileName = nameof(TestMapConfig))]
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
    }
}