using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct ResourceGainData : ISame<ResourceGainData> {
        public int Money;
        public int Metal;

        public bool IsSame(ISame<ResourceGainData> other) {
            var o = other.Get();
            return Money == o.Money
                   && Metal == o.Metal;
        }
    }
}