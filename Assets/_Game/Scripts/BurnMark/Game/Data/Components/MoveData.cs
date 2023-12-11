using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct MoveData : ISame<MoveData> {
        public int Distance;
        public bool CanMove;

        public bool IsSame(MoveData other) {
            return Distance == other.Distance
                && CanMove == other.CanMove;
        }
    }
}