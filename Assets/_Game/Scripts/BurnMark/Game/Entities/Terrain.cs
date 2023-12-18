using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public static class Terrain {
        public static Func<int, Entity> Create(Vector2Int position, TerrainConfig terrainConfig) {
            var positionComponent = Entity.Add(new PositionData {
                Position = position
            });
            var terrainComponent =  Entity.Add(terrainConfig.TerrainData);
            return id => new Entity(id, positionComponent, terrainComponent);
        }

        public static bool IsTerrain(IReadOnlyEntity entity, out IReadOnlyComponent<TerrainData> component) {
            component = entity.GetReadOnlyComponent<TerrainData>();
            return component != null;
        }
    }
}