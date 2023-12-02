using _Game.Scripts.ModelV4;

namespace _Game.Scripts.TicTacToe.Game.Commands {
    public class WinCommand : TicTacToeCommand {
        public int Winner;
        
        protected override void PerformDoOnAPI(GameDataAPI api) {
            api.EndTurn(true);
        }

        protected override string SerializeContents() {
            return Winner.ToString();
        }

        protected override void DeserializeContents(string contents) {
            Winner = int.Parse(contents);
        }
    }
}
