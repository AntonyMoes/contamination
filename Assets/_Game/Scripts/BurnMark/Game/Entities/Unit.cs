﻿using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public static class Unit {
        public static Func<int, Entity> Create(int user, Vector2Int position, HealthData healthData, AttackData attackData, int movementDistance) {
            var ownerComponent = Entity.Add(new OwnerData {
                Owner = user
            });
            var positionComponent = Entity.Add(new PositionData {
                Position = position
            });
            var unitComponent = Entity.Add(new UnitData());
            var healthComponent = Entity.Add(healthData);
            var attackComponent = Entity.Add(attackData);
            var movementComponent = Entity.Add(new MoveData {
                Distance = movementDistance
            });
            return id => new Entity(id, ownerComponent, positionComponent, unitComponent, healthComponent, attackComponent, movementComponent);
        } 
    }
}