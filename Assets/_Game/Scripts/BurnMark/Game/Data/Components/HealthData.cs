using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct HealthData : ISame<HealthData> {
        public float MaxHealth;
        public float Health;
        public int Armor;

        public bool IsSame(HealthData other) {
            return MaxHealth == other.MaxHealth
                   && Health == other.Health
                   && Armor == other.Armor;
        }

        public HealthData TakeDamage(float damage) {
            return new HealthData {
                MaxHealth = MaxHealth,
                Health = Mathf.Max(Health - damage, 0f),
                Armor = Armor
            };
        }
    }
}