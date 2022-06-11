using System;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample {
    public class WinCommand : GameCommand {
        public int Winner;
        
        protected override Action<GameDataAPI> PerformReversibleDoOnData(GameDataAPI api) {
            api.EndTurn(true);
            return api => api.UndoEndTurn(true);
        }

        public override string SerializeContents() {
            return Winner.ToString();
        }

        public override void DeserializeContents(string contents) {
            Winner = int.Parse(contents);
        }
    }
}
