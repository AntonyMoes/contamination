using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.TicTacToe.Game.Data {
    public struct MarkData : ISame<MarkData> {
        public EMark Mark;

        public bool IsSame(MarkData other) {
            return Mark == other.Mark;
        }

        public enum EMark {
            None,
            X,
            O
        }
    }
}
