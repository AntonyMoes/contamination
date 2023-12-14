using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public static class ScoutCar {
        public const int Health = 20;
        public const int Armor = 2;

        public const int Damage = 3;
        public const int Attacks = 2;
        public const int ArmorPiercing = 0;
        public const int Range = 2;

        public const int MovementDistance = 3;

        public static Func<int, Entity> Create(int user, Vector2Int position) {
            var healthData = new HealthData {
                Health = Health,
                MaxHealth = Health,
                Armor = Armor
            };
            var attackData = new AttackData {
                Damage = Damage,
                Attacks = Attacks,
                ArmorPiercing = ArmorPiercing,
                Range = Range
            };
            return null;//Unit.Create(user, position, healthData, attackData, MovementDistance);
        }
    }
}