using System;
using _Game.Scripts.NetworkModel.Commands;
using GeneralUtils;

namespace _Game.Scripts.NetworkModel {
    public class InitialCommandGenerator : IInitialCommandGenerator {
        private readonly TurnController _turnController;

        public Event<GameCommand> OnCommandGenerated { get; }

        private Action _triggerInitialCommand;

        public InitialCommandGenerator(GameCommand initialCommand, TurnController turnController) {
            _turnController = turnController;
            OnCommandGenerated = new Event<GameCommand>(out var onCommandGenerated);
            _triggerInitialCommand = () => onCommandGenerated(initialCommand);
        }

        public void TriggerInitialCommand() {
            _triggerInitialCommand?.Invoke();
            _triggerInitialCommand = null;
        }

        public void OnInitialCommandFinished() {
            _turnController.OnInitialCommandFinished();
        }
    }
}
