using _Game.Scripts.NetworkModel.Commands;
using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.NetworkModel {
    public interface IDataReader {
        public void SetReadAPI(IGameReadAPI api);
    }

    public interface ICommandPresenter : IDataReader {
        public Process PresentCommand(GameCommand generatedCommand);
    }

    public interface ICommandSource {
        public Event<GameCommand> OnCommandGenerated { get; }
    }

    public interface ICommandGenerator : ICommandSource, IDataReader { }

    public interface IInitialCommandGenerator : ICommandSource {
        public void OnInitialCommandFinished();
    }
}
