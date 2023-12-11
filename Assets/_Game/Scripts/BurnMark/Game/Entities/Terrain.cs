using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public class Terrain {
        public static Func<int, Entity> Create(Vector2Int position) {
            var positionComponent = Entity.Add(new PositionData {
                Position = position
            });
            var terrainComponent =  Entity.Add(new TerrainData());
            return id => new Entity(id, positionComponent, terrainComponent);
        } 
    }
}