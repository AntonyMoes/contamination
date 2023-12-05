using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct MoveData : ISame<MoveData> {
        public int Distance;
        public bool CanMove;

        public bool IsSame(ISame<MoveData> other) {
            var o = other.Get();
            return Distance == o.Distance
                && CanMove == o.CanMove;
        }
    }
}