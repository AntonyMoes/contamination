using _Game.Scripts.ModelV4;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.NetworkModel.Commands;

namespace _Game.Scripts.TicTacToe.Game.Commands {
    public abstract class TicTacToeCommand : StringSerializedGameCommand {
        protected sealed override void PerformDoOnData(IGameAPI api) {
            PerformDoOnAPI((GameDataAPI) api);
        }

        protected abstract void PerformDoOnAPI(GameDataAPI api);
    }
}