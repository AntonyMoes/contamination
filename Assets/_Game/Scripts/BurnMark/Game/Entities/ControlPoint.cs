using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public class ControlPoint {
        public static Func<int, Entity> Create(Vector2Int position) {
            var ownerComponent = Entity.Add(new OwnerData {
                Owner = null
            });
            var positionComponent = Entity.Add(new PositionData {
                Position = position
            });
            var fieldObjectComponent = Entity.Add(new FieldObjectData());
            return id => new Entity(id, ownerComponent, positionComponent, fieldObjectComponent);
        } 
    }
}