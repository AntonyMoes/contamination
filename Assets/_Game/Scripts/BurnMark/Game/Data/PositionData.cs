using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data {
    public struct PositionData : ISame<PositionData> {
        public int Row;
        public int Column;

        public bool IsSame(ISame<PositionData> other) {
            var o = other.Get();
            return Row == o.Row && Column == o.Column;
        }
    }
}
