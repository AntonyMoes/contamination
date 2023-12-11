using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct ResourceData : ISame<ResourceData> {
        public int Money;
        public int Metal;

        public bool IsSame(ResourceData other) {
            return Money == other.Money
                   && Metal == other.Metal;
        }

        public ResourceData Gain(ResourceGainData gainData) {
            return new ResourceData {
                Money = Money + gainData.Money,
                Metal = Metal + gainData.Metal
            };
        }
    }
}