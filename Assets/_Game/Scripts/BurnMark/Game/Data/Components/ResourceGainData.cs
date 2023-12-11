using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct ResourceGainData : ISame<ResourceGainData> {
        public int Money;
        public int Metal;

        public bool IsSame(ResourceGainData other) {
            return Money == other.Money
                   && Metal == other.Metal;
        }
    }
}