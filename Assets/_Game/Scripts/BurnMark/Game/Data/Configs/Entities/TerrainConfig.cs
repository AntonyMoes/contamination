using System;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;
using Terrain = _Game.Scripts.BurnMark.Game.Entities.Terrain;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Entities {
    public class TerrainConfig : Config {
        [SerializeField] private TerrainData _terrainData;
        public TerrainData TerrainData => _terrainData;
        
        public Func<int, Entity> Create(Vector2Int position) {
            return Terrain.Create(position, this);
        }

#if UNITY_EDITOR
        [MenuItem(Configs.EntityMenuItem + nameof(TerrainConfig), false)]
        public static void Create() => Configs.Create<TerrainConfig>();
#endif
    }
}