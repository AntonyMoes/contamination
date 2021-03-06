using _Game.Scripts.ModelV4;

namespace _Game.Scripts.TicTacToe.Commands {
    public class WinCommand : GameCommand {
        public int Winner;
        
        protected override void PerformDoOnData(GameDataAPI api) {
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
