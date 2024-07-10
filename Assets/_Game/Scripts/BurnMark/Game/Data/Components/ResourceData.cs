using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct ResourceData : ISame<ResourceData> {
        // public int Money;
        // public int Metal;
        public Resources Resources;

        public bool IsSame(ResourceData other) {
            return Resources.IsSame(other.Resources);
        }

        public ResourceData Gain(ResourceGainData gainData) {
            return new ResourceData {
                Resources = Resources.Add(gainData.Resources)
            };
        }

        public bool CanPay(Cost cost) {
            return Resources.EnoughFor(cost.Resources);
        }

        public bool TryPay(Cost cost, out ResourceData data) {
            if (!CanPay(cost)) {
                data = this;
                return false;
            }

            data = new ResourceData {
                Resources = Resources.Subtract(cost.Resources)
            };
            return true;
        }
    }
}