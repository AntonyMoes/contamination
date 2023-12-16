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

        public bool CanPay(Cost cost) {
            return Money >= cost.Money
                   && Metal >= cost.Metal;
        }

        public bool TryPay(Cost cost, out ResourceData data) {
            if (!CanPay(cost)) {
                data = this;
                return false;
            }

            data = new ResourceData {
                Money = Money - cost.Money,
                Metal = Metal - cost.Metal
            };
            return true;
        }
    }
}