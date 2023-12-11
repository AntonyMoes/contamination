using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct HealthData : ISame<HealthData> {
        public int MaxHealth;
        public int Health;
        public int Armor;

        public bool IsSame(HealthData other) {
            return MaxHealth == other.MaxHealth
                   && Health == other.Health
                   && Armor == other.Armor;
        }
    }
}