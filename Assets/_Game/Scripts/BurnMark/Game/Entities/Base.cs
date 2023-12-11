using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public class Base {
        private const int Health = 100;
        private const int Armor = 10;

        private const int MoneyGain = 10;
        private const int MetalGain = 2;

        public static Func<int, Entity> Create(int user, Vector2Int position) {
            var ownerComponent = Entity.Add(new OwnerData {
                Owner = user
            });
            var positionComponent = Entity.Add(new PositionData {
                Position = position
            });
            var fieldObjectComponent = Entity.Add(new FieldObjectData());
            var healthComponent = Entity.Add(new HealthData {
                Health = Health,
                MaxHealth = Health,
                Armor = Armor
            });
            var resourceGainComponent = Entity.Add(new ResourceGainData {
                Money = MoneyGain,
                Metal = MetalGain,
            });
            return id => new Entity(id, ownerComponent, positionComponent, fieldObjectComponent, healthComponent, resourceGainComponent);
        } 
    }
}