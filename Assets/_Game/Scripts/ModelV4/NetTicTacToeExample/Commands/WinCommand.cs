namespace _Game.Scripts.ModelV4.NetTicTacToeExample.Commands {
    public class WinCommand : GameCommand {
        public int Winner;
        
        protected override void PerformDoOnData(GameDataAPI api) {
            api.EndTurn(true);
        }

        public override string SerializeContents() {
            return Winner.ToString();
        }

        public override void DeserializeContents(string contents) {
            Winner = int.Parse(contents);
        }
    }
}
