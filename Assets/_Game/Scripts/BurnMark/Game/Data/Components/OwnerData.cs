using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct OwnerData : ISame<OwnerData> {
        public int? Owner;

        public bool IsSame(ISame<OwnerData> other) {
            var o = other.Get();
            return Owner == o.Owner;
        }
    }
}