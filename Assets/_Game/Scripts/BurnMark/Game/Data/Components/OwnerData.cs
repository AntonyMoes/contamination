using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct OwnerData : ISame<OwnerData> {
        public int? Owner;

        public bool IsSame(OwnerData other) {
            return Owner == other.Owner;
        }

        public OwnerData WithOwner(int? owner) {
            return new OwnerData {
                Owner = owner
            };
        }
    }
}