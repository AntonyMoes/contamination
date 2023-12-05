using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public class Unit {
        public static Func<int, Entity> Create(int user, Vector2Int position, HealthData healthData, AttackData attackData, int movementDistance) {
            var ownerComponent = Entity.Add(new OwnerData {
                Owner = user
            });
            var positionComponent = Entity.Add(new PositionData {
                Position = position
            });
            var passableComponent = Entity.Add(new PassableData {
                Status = PassableData.PassableStatus.PassableToAlliesNotStoppable
            });
            var healthComponent = Entity.Add(healthData);
            var attackComponent = Entity.Add(attackData);
            var movementComponent = Entity.Add(new MoveData {
                Distance = movementDistance
            });
            return id => new Entity(id, ownerComponent, positionComponent, passableComponent, healthComponent, attackComponent, movementComponent);
        } 
    }
}