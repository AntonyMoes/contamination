using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Map {
    [CreateAssetMenu(menuName = Configs.MapMenuItem + nameof(RegularMapConfig), fileName = nameof(RegularMapConfig))]
    public class RegularMapConfig : MapConfig {
        [SerializeField] private List<Vector2Int> _startingPoints = new List<Vector2Int>();
        public override Vector2Int[] StartingPoints => _startingPoints.ToArray();

        [SerializeField] private List<Tile> _terrain = new List<Tile>();

        public override TerrainData?[,] Terrain {
            get {
                var min = _terrain.Aggregate(_terrain.First().Position, (pos, tile) => Vector2Int.Min(pos, tile.Position));
                var max = _terrain.Aggregate(_terrain.First().Position, (pos, tile) => Vector2Int.Max(pos, tile.Position));

                var mapSize = max - min + Vector2Int.one;
                var result = new TerrainData?[mapSize.x, mapSize.y];
                foreach (var position in mapSize.EnumeratePositions()) {
                    var tile = _terrain.FirstOrDefault(t => t.Position == position);
                    result[position.x, position.y] = tile?.Data;
                }

                return result;
            }
        }

        public void Set(Vector2Int position, TerrainData? data) {
            var tile = _terrain.FirstOrDefault(t => t.Position == position);
            if (data is { } terrainData) {
                if (tile != null) {
                    tile.Data = terrainData;
                } else {
                    _terrain.Add(new Tile {
                        Position = position,
                        Data = terrainData
                    });
                }
            } else {
                _terrain.Remove(tile);
            }
        }

        public void SetStartingPoint(Vector2Int position, bool add) {
            if (add) {
                if (!_startingPoints.Contains(position)) {
                    _startingPoints.Add(position);
                }
            } else {
                _startingPoints.Remove(position);
            }
        }

        [Serializable]
        private class Tile {
            public Vector2Int Position;
            public TerrainData Data;
        }
    }
}