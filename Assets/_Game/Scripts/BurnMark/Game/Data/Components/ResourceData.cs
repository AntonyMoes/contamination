using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct ResourceData : ISame<ResourceData> {
        public int Money;
        public int Metal;

        public bool IsSame(ISame<ResourceData> other) {
            var o = other.Get();
            return Money == o.Money
                   && Metal == o.Metal;
        }

        public ResourceData Gain(ResourceGainData gainData) {
            return new ResourceData {
                Money = Money + gainData.Money,
                Metal = Metal + gainData.Metal
            };
        }
    }
}