using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.ModelV4 {
    public interface ICommandGenerator {
        public void SetReadAPI(GameDataReadAPI api);

        public Event<GameCommand> OnCommandGenerated { get; }
        public Process ShowCommand(GameCommand generatedCommand);
    }
}
