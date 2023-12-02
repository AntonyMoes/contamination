using _Game.Scripts.ModelV4;
using _Game.Scripts.NetworkModel;

namespace _Game.Scripts.TicTacToe.Commands {
    public abstract class TicTacToeCommand : GameCommand {
        protected sealed override void PerformDoOnData(IGameAPI api) {
            PerformDoOnAPI((GameDataAPI) api);
        }

        protected abstract void PerformDoOnAPI(GameDataAPI api);
    }
}