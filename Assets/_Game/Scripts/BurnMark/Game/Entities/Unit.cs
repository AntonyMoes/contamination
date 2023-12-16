using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public static class Unit {
        public static Func<int, Entity> Create(int user, Vector2Int position, UnitConfig config) {
            var ownerComponent = Entity.Add(new OwnerData {
                Owner = user
            });
            var positionComponent = Entity.Add(new PositionData {
                Position = position
            });
            var unitComponent = Entity.Add(new UnitData {
                Config = config
            });
            var healthComponent = Entity.Add(config.HealthData.WithMaxHealth());
            var attackComponent = Entity.Add(config.AttackData);
            var movementComponent = Entity.Add(config.MoveData);
            return id => new Entity(id, ownerComponent, positionComponent, unitComponent, healthComponent, attackComponent, movementComponent);
        }

        public static bool IsUnit(IReadOnlyEntity entity, out IReadOnlyComponent<UnitData> component) {
            component = entity.GetReadOnlyComponent<UnitData>();
            return component != null;
        }
    }
}