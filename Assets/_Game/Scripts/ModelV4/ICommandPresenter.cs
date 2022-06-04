using GeneralUtils.Processes;

namespace _Game.Scripts.ModelV4 {
    public interface ICommandPresenter : IDataReader {
        public Process PresentCommand(GameCommand generatedCommand);
    }
}
