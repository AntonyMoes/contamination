namespace _Game.Scripts.ModelV3.TicTacToeExample {
    public class TicTacToeState {
        public int Size { get; private set; }
        public EMark[][] Field;

        public void Reset(int size) {
            Size = size;
            Field = new EMark[size][];
            for (var i = 0; i < size; i++) {
                Field[i] = new EMark[size];
                for (var j = 0; j < size; j++) {
                    Field[i][j] = EMark.None;
                }
            }
        }

        public enum EMark {
            None,
            X,
            O
        }
    }
}
