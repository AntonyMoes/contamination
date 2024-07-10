using System;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public static class Terrain {
        public static Func<int, Entity> Create(Vector2Int position, TerrainData terrainData) {
            var positionComponent = Entity.Add(new PositionData {
                Position = position
            });
            var terrainComponent =  Entity.Add(terrainData);
            var additionalComponents = terrainData.Features.SelectMany(f => f.GetAdditionalComponents());
            var components = new[] { positionComponent, terrainComponent }
                .Concat(additionalComponents)
                .ToArray();
            return id => new Entity(id, components);
        }

        public static bool IsTerrain(IReadOnlyEntity entity, out IReadOnlyComponent<TerrainData> component) {
            component = entity.GetReadOnlyComponent<TerrainData>();
            return component != null;
        }
    }
}