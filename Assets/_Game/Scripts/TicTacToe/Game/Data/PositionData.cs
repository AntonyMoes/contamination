using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.TicTacToe.Game.Data {
    public struct PositionData : ISame<PositionData> {
        public int Row;
        public int Column;

        public bool IsSame(PositionData other) {
            return Row == other.Row && Column == other.Column;
        }
    }
}
