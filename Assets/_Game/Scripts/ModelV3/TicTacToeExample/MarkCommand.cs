using System;

namespace _Game.Scripts.ModelV3.TicTacToeExample {
    public class MarkCommand : GameCommand {
        private TicTacToeState _state;

        public TicTacToeState.EMark Mark { get; }
        public int Row { get; }
        public int Column { get; }

        public MarkCommand(TicTacToeState.EMark mark, int row, int column) {
            Mark = mark;
            Row = row;
            Column = column;
        }

        public void SetState(TicTacToeState state) {
            _state = state;
        }

        protected override Action PerformReversibleDo() {
            var oldMark = _state.Field[Row][Column];
            _state.Field[Row][Column] = Mark;
            return () => _state.Field[Row][Column] = oldMark;
        }
    }
}
