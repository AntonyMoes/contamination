using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Map {
    public abstract class MapConfig : Config {
        [SerializeField] private string _id;
        public string Id => _id;

        public abstract TerrainConfig[,] Terrain { get; }
        public abstract Vector2Int[] StartingPoints { get; }
    }
}