using System;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;
using Terrain = _Game.Scripts.BurnMark.Game.Entities.Terrain;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Entities {
    [CreateAssetMenu(menuName = Configs.EntityMenuItem + nameof(TerrainConfig), fileName = nameof(TerrainConfig))]
    public class TerrainConfig : Config {
        [SerializeField] private TerrainData _terrainData;
        public TerrainData TerrainData => _terrainData;
        
        public Func<int, Entity> Create(Vector2Int position) {
            return Terrain.Create(position, this);
        }
    }
}