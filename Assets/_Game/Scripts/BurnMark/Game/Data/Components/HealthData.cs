﻿using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct HealthData : ISame<HealthData> {
        public int MaxHealth;
        public int Health;
        public int Armor;

        public bool IsSame(ISame<HealthData> other) {
            var o = other.Get();
            return MaxHealth == o.MaxHealth
                   && Health == o.Health
                   && Armor == o.Armor;
        }
    }
}