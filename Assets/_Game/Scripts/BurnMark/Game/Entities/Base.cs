﻿using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public static class Base {
        public static Func<int, Entity> Create(int user, Vector2Int position, BaseConfig baseConfig) {
            var ownerComponent = Entity.Add(new OwnerData {
                Owner = user
            });
            var positionComponent = Entity.Add(new PositionData {
                Position = position
            });
            var fieldObjectComponent = Entity.Add(new FieldObjectData {
                Config = baseConfig
            });
            var healthComponent = Entity.Add(baseConfig.HealthData.WithMaxHealth());
            var resourceGainComponent = Entity.Add(baseConfig.ResourceGainData);
            var unitBuilderComponent = Entity.Add(baseConfig.UnitBuilderData);
            return id => new Entity(id, ownerComponent, positionComponent, fieldObjectComponent, healthComponent, resourceGainComponent, unitBuilderComponent);
        }

        public static bool IsBase(IReadOnlyEntity entity) {
            return entity.GetReadOnlyComponent<FieldObjectData>() is { Data: { Config: BaseConfig _ } };
        }
    }
}