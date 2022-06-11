using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.ModelV4 {
    public interface IDataReader {
        public void SetReadAPI(GameDataReadAPI api);
    }

    public interface ICommandPresenter : IDataReader {
        public Process PresentCommand(GameCommand generatedCommand);
    }

    public interface ICommandSource {
        public Event<GameCommand> OnCommandGenerated { get; }
    }

    public interface ICommandGenerator : ICommandSource, IDataReader { }
}
