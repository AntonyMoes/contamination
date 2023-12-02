using System;
using GeneralUtils;

namespace _Game.Scripts.NetworkModel {
    public class InitialCommandGenerator : ICommandGenerator {
        private Action _triggerInitialCommand;

        public InitialCommandGenerator(GameCommand initialCommand) {
            OnCommandGenerated = new Event<GameCommand>(out var onCommandGenerated);
            _triggerInitialCommand = () => onCommandGenerated(initialCommand);
        }

        public void TriggerInitialCommand() {
            _triggerInitialCommand?.Invoke();
            _triggerInitialCommand = null;
        }

        public Event<GameCommand> OnCommandGenerated { get; }
        public void SetReadAPI(IGameReadAPI api) { }
    }
}
